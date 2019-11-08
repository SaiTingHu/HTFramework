using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HT.Framework
{
    public abstract class HTFEditor<E> : Editor where E : Object
    {
        /// <summary>
        /// 当前目标
        /// </summary>
        protected E Target;

        private GithubURLAttribute _GithubURL;
        private CSDNBlogURLAttribute _CSDNURL;
        private Texture _GithubIcon;
        private Texture _CSDNIcon;
        private Dictionary<string, SerializedProperty> _serializedPropertys = new Dictionary<string, SerializedProperty>();

        /// <summary>
        /// 是否启用运行时调试数据
        /// </summary>
        protected virtual bool IsEnableRuntimeData
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 是否启用基础属性展示
        /// </summary>
        protected virtual bool IsEnableBaseInspectorGUI
        {
            get
            {
                return false;
            }
        }

        private void OnEnable()
        {
            Target = target as E;
            _GithubURL = GetType().GetCustomAttribute<GithubURLAttribute>();
            _CSDNURL = GetType().GetCustomAttribute<CSDNBlogURLAttribute>();
            _GithubIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Github.png");
            _CSDNIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/CSDN.png");
            _serializedPropertys.Clear();

            OnDefaultEnable();

            if (IsEnableRuntimeData && EditorApplication.isPlaying)
            {
                OnRuntimeEnable();
            }
        }
        
        public sealed override void OnInspectorGUI()
        {
            if (_GithubURL != null || _CSDNURL != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (_GithubURL != null)
                {
                    GUI.enabled = !string.IsNullOrEmpty(_GithubURL.URL);
                    if (GUILayout.Button(_GithubIcon, "IconButton", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        Application.OpenURL(_GithubURL.URL);
                    }
                    GUI.enabled = true;
                }

                if (_CSDNURL != null)
                {
                    GUI.enabled = !string.IsNullOrEmpty(_CSDNURL.URL);
                    if (GUILayout.Button(_CSDNIcon, "IconButton", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        Application.OpenURL(_CSDNURL.URL);
                    }
                    GUI.enabled = true;
                }

                GUILayout.EndHorizontal();
            }

            if (IsEnableBaseInspectorGUI)
            {
                base.OnInspectorGUI();
            }

            OnInspectorDefaultGUI();

            if (IsEnableRuntimeData && EditorApplication.isPlaying)
            {
                GUI.backgroundColor = Color.cyan;
                GUI.color = Color.white;

                GUILayout.BeginVertical("Helpbox");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Runtime Data", "BoldLabel");
                GUILayout.EndHorizontal();

                OnInspectorRuntimeGUI();

                GUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 默认 Enable 初始化
        /// </summary>
        protected virtual void OnDefaultEnable()
        { }

        /// <summary>
        /// 运行时 Enable 初始化
        /// </summary>
        protected virtual void OnRuntimeEnable()
        { }

        /// <summary>
        /// 默认 Inspector GUI
        /// </summary>
        protected virtual void OnInspectorDefaultGUI()
        { }

        /// <summary>
        /// 运行时 Inspector GUI
        /// </summary>
        protected virtual void OnInspectorRuntimeGUI()
        { }

        /// <summary>
        /// 标记目标已改变
        /// </summary>
        protected void HasChanged()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorUtility.SetDirty(target);
                Component component = target as Component;
                if (component != null && component.gameObject.scene != null)
                {
                    EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                }
            }
        }

        //可撤销操作、根据改变 SetDirty 的控件
        /// <summary>
        /// 制作一个Button
        /// </summary>
        protected void Button(HTFAction action, string name, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, options))
            {
                Undo.RecordObject(target, "click button");
                action();
                HasChanged();
            }
        }
        /// <summary>
        /// 制作一个Button
        /// </summary>
        protected void Button(HTFAction action, string name, GUIStyle style, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, style, options))
            {
                Undo.RecordObject(target, "click button");
                action();
                HasChanged();
            }
        }
        /// <summary>
        /// 制作一个Toggle
        /// </summary>
        protected void Toggle(bool value, out bool outValue, string name, params GUILayoutOption[] options)
        {
            GUI.color = value ? Color.white : Color.gray;
            bool newValue = EditorGUILayout.Toggle(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个Toggle
        /// </summary>
        protected void Toggle(bool value, out bool outValue, string name, GUIStyle style, params GUILayoutOption[] options)
        {
            GUI.color = value ? Color.white : Color.gray;
            bool newValue = EditorGUILayout.Toggle(name, value, style, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个GUILayout Toggle
        /// </summary>
        protected void GUILayoutToggle(bool value, out bool outValue, string name, params GUILayoutOption[] options)
        {
            GUI.color = value ? Color.white : Color.gray;
            bool newValue = GUILayout.Toggle(value, name, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个GUILayout Toggle
        /// </summary>
        protected void GUILayoutToggle(bool value, out bool outValue, string name, GUIStyle style, params GUILayoutOption[] options)
        {
            GUI.color = value ? Color.white : Color.gray;
            bool newValue = GUILayout.Toggle(value, name, style, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个IntSlider
        /// </summary>
        protected void IntSlider(int value, out int outValue, int leftValue, int rightValue, string name, params GUILayoutOption[] options)
        {
            int newValue = EditorGUILayout.IntSlider(name, value, leftValue, rightValue, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set int value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个FloatSlider
        /// </summary>
        protected void FloatSlider(float value, out float outValue, float leftValue, float rightValue, string name, params GUILayoutOption[] options)
        {
            float newValue = EditorGUILayout.Slider(name, value, leftValue, rightValue, options);
            if (!Mathf.Approximately(value, newValue))
            {
                Undo.RecordObject(target, "Set float value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个IntField
        /// </summary>
        protected void IntField(int value, out int outValue, string name, params GUILayoutOption[] options)
        {
            int newValue = EditorGUILayout.IntField(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set int value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个FloatField
        /// </summary>
        protected void FloatField(float value, out float outValue, string name, params GUILayoutOption[] options)
        {
            float newValue = EditorGUILayout.FloatField(name, value, options);
            if (!Mathf.Approximately(value, newValue))
            {
                Undo.RecordObject(target, "Set float value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个TextField
        /// </summary>
        protected void TextField(string value, out string outValue, string name, params GUILayoutOption[] options)
        {
            GUI.color = value != "" ? Color.white : Color.gray;
            string newValue = EditorGUILayout.TextField(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set string value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个PasswordField
        /// </summary>
        protected void PasswordField(string value, out string outValue, string name, params GUILayoutOption[] options)
        {
            GUI.color = value != "" ? Color.white : Color.gray;
            string newValue = EditorGUILayout.PasswordField(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set string value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个ObjectField
        /// </summary>
        protected void ObjectField<T>(T value, out T outValue, bool allowSceneObjects, string name, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            GUI.color = value ? Color.white : Color.gray;
            T newValue = EditorGUILayout.ObjectField(name, value, typeof(T), allowSceneObjects, options) as T;
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set object value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }
        /// <summary>
        /// 制作一个Vector2Field
        /// </summary>
        protected void Vector2Field(Vector2 value, out Vector2 outValue, string name, params GUILayoutOption[] options)
        {
            Vector2 newValue = EditorGUILayout.Vector2Field(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set vector2 value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个Vector3Field
        /// </summary>
        protected void Vector3Field(Vector3 value, out Vector3 outValue, string name, params GUILayoutOption[] options)
        {
            Vector3 newValue = EditorGUILayout.Vector3Field(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set vector3 value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个枚举下拉按钮
        /// </summary>
        protected void EnumPopup<T>(Enum value, out T outValue, string name, params GUILayoutOption[] options) where T : Enum
        {
            Enum newValue = EditorGUILayout.EnumPopup(name, value, options);
            if (!Equals(value, newValue))
            {
                Undo.RecordObject(target, "Set enum value");
                outValue = (T)newValue;
                HasChanged();
            }
            else
            {
                outValue = (T)value;
            }
        }
        /// <summary>
        /// 制作一个ColorField
        /// </summary>
        protected void ColorField(Color value, out Color outValue, string name, params GUILayoutOption[] options)
        {
            Color newValue = EditorGUILayout.ColorField(name, value, options);
            if (value != newValue)
            {
                Undo.RecordObject(target, "Set color value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个PropertyField
        /// </summary>
        protected void PropertyField(string propertyName, string name, params GUILayoutOption[] options)
        {
            SerializedProperty serializedProperty;
            if (_serializedPropertys.ContainsKey(propertyName))
            {
                serializedProperty = _serializedPropertys[propertyName];
            }
            else
            {
                serializedProperty = serializedObject.FindProperty(propertyName);
                if (serializedProperty != null)
                {
                    _serializedPropertys.Add(propertyName, serializedProperty);
                }
            }

            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name), true, options);
            }
            else
            {
                EditorGUILayout.HelpBox("Property [" + propertyName + "] not found!", MessageType.Error);
            }
        }
        /// <summary>
        /// 制作一个PropertyField
        /// </summary>
        protected void PropertyField(string propertyName, params GUILayoutOption[] options)
        {
            SerializedProperty serializedProperty;
            if (_serializedPropertys.ContainsKey(propertyName))
            {
                serializedProperty = _serializedPropertys[propertyName];
            }
            else
            {
                serializedProperty = serializedObject.FindProperty(propertyName);
                if (serializedProperty != null)
                {
                    _serializedPropertys.Add(propertyName, serializedProperty);
                }
            }

            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, true, options);
            }
            else
            {
                EditorGUILayout.HelpBox("Property [" + propertyName + "] not found!", MessageType.Error);
            }
        }
        /// <summary>
        /// 制作一个PropertyField
        /// </summary>
        protected void PropertyField(string propertyName, string name, bool includeChildren, params GUILayoutOption[] options)
        {
            SerializedProperty serializedProperty;
            if (_serializedPropertys.ContainsKey(propertyName))
            {
                serializedProperty = _serializedPropertys[propertyName];
            }
            else
            {
                serializedProperty = serializedObject.FindProperty(propertyName);
                if (serializedProperty != null)
                {
                    _serializedPropertys.Add(propertyName, serializedProperty);
                }
            }

            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name), includeChildren, options);
            }
            else
            {
                EditorGUILayout.HelpBox("Property [" + propertyName + "] not found!", MessageType.Error);
            }
        }
        /// <summary>
        /// 制作一个PropertyField
        /// </summary>
        protected void PropertyField(string propertyName, bool includeChildren, params GUILayoutOption[] options)
        {
            SerializedProperty serializedProperty;
            if (_serializedPropertys.ContainsKey(propertyName))
            {
                serializedProperty = _serializedPropertys[propertyName];
            }
            else
            {
                serializedProperty = serializedObject.FindProperty(propertyName);
                if (serializedProperty != null)
                {
                    _serializedPropertys.Add(propertyName, serializedProperty);
                }
            }

            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, includeChildren, options);
            }
            else
            {
                EditorGUILayout.HelpBox("Property [" + propertyName + "] not found!", MessageType.Error);
            }
        }
    }
}