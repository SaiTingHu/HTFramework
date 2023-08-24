using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 自定义编辑器行为基类
    /// </summary>
    /// <typeparam name="E">自定义的组件</typeparam>
    public abstract class HTFEditor<E> : Editor where E : UObject
    {
        /// <summary>
        /// 当前目标
        /// </summary>
        protected E Target;
        /// <summary>
        /// 当前的所有目标
        /// </summary>
        protected E[] Targets;
        /// <summary>
        /// 复制、粘贴按钮的GUIContent
        /// </summary>
        protected GUIContent CopyPasteGC;
        
        private GithubURLAttribute _GithubURL;
        private GiteeURLAttribute _GiteeURL;
        private CSDNBlogURLAttribute _CSDNURL;
        private GUIContent _GithubGC;
        private GUIContent _GiteeGC;
        private GUIContent _CSDNGC;
        private Dictionary<string, SerializedProperty> _serializedPropertys = new Dictionary<string, SerializedProperty>();

        /// <summary>
        /// 是否启用运行时调试数据
        /// </summary>
        protected virtual bool IsEnableRuntimeData => true;
        /// <summary>
        /// 是否启用基础属性展示
        /// </summary>
        protected virtual bool IsEnableBaseInspectorGUI => false;
        /// <summary>
        /// 是否启用宽模式
        /// </summary>
        protected virtual bool IsWideMode => true;
        /// <summary>
        /// 控件标签的标准宽度
        /// </summary>
        protected virtual float LabelWidth
        {
            get
            {
                return EditorGUIUtility.labelWidth;
            }
        }

        private void OnEnable()
        {
            Target = target as E;
            Targets = targets.ConvertAllAS<E, UObject>();
            CopyPasteGC = new GUIContent();
            CopyPasteGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            CopyPasteGC.tooltip = "Copy or Paste";

            _GithubURL = GetType().GetCustomAttribute<GithubURLAttribute>();
            _GiteeURL = GetType().GetCustomAttribute<GiteeURLAttribute>();
            _CSDNURL = GetType().GetCustomAttribute<CSDNBlogURLAttribute>();
            if (_GithubURL != null)
            {
                _GithubGC = new GUIContent();
                _GithubGC.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Github.png");
                _GithubGC.tooltip = "Github";
            }
            if (_GiteeURL != null)
            {
                _GiteeGC = new GUIContent();
                _GiteeGC.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Gitee.png");
                _GiteeGC.tooltip = "Gitee";
            }
            if (_CSDNURL != null)
            {
                _CSDNGC = new GUIContent();
                _CSDNGC.image = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/CSDN.png");
                _CSDNGC.tooltip = "CSDN";
            }
            _serializedPropertys.Clear();
            
            OnDefaultEnable();

            if (IsEnableRuntimeData && EditorApplication.isPlaying)
            {
                OnRuntimeEnable();
            }
        }
        private void OnDisable()
        {
            _serializedPropertys.Clear();

            OnDefaultDisable();

            if (IsEnableRuntimeData && EditorApplication.isPlaying)
            {
                OnRuntimeDisable();
            }
        }
        public sealed override void OnInspectorGUI()
        {
            if (EditorGUIUtility.wideMode != IsWideMode)
            {
                EditorGUIUtility.wideMode = IsWideMode;
            }

            if (_GithubURL != null || _GiteeURL != null || _CSDNURL != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (_GiteeURL != null)
                {
                    GUI.enabled = !string.IsNullOrEmpty(_GiteeURL.URL);
                    if (GUILayout.Button(_GiteeGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        Application.OpenURL(_GiteeURL.URL);
                    }
                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    GUI.enabled = true;
                }

                if (_GithubURL != null)
                {
                    GUI.enabled = !string.IsNullOrEmpty(_GithubURL.URL);
                    if (GUILayout.Button(_GithubGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        Application.OpenURL(_GithubURL.URL);
                    }
                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    GUI.enabled = true;
                }

                if (_CSDNURL != null)
                {
                    GUI.enabled = !string.IsNullOrEmpty(_CSDNURL.URL);
                    if (GUILayout.Button(_CSDNGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        Application.OpenURL(_CSDNURL.URL);
                    }
                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    GUI.enabled = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            serializedObject.Update();

            if (IsEnableBaseInspectorGUI)
            {
                base.OnInspectorGUI();
            }
            
            OnInspectorDefaultGUI();

            if (IsEnableRuntimeData && EditorApplication.isPlaying)
            {
                GUI.backgroundColor = Color.cyan;
                GUI.color = Color.white;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Runtime Data", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                OnInspectorRuntimeGUI();

                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 默认 Enable
        /// </summary>
        protected virtual void OnDefaultEnable()
        { }
        /// <summary>
        /// 运行时 Enable
        /// </summary>
        protected virtual void OnRuntimeEnable()
        { }
        /// <summary>
        /// 默认 Disable
        /// </summary>
        protected virtual void OnDefaultDisable()
        { }
        /// <summary>
        /// 运行时 Disable
        /// </summary>
        protected virtual void OnRuntimeDisable()
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
        /// <param name="markTarget">是否仅标记单个 target</param>
        protected void HasChanged(bool markTarget = false)
        {
            if (markTarget)
            {
                EditorUtility.SetDirty(target);
            }
            else
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    EditorUtility.SetDirty(targets[i]);
                }
            }

            if (EditorApplication.isPlaying)
                return;

            Component component = target as Component;
            if (component != null && component.gameObject.scene != null)
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }
        /// <summary>
        /// 根据名字获取序列化属性
        /// </summary>
        /// <param name="propertyName">序列化属性名字</param>
        /// <returns>序列化属性</returns>
        protected SerializedProperty GetProperty(string propertyName)
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
            return serializedProperty;
        }

        //可撤销操作、根据改变 SetDirty 的控件
        /// <summary>
        /// 制作一个Button
        /// </summary>
        protected void Button(HTFAction action, string name, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, options))
            {
                Undo.RecordObject(target, "Click button");
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
                Undo.RecordObject(target, "Click button");
                action();
                HasChanged();
            }
        }
        /// <summary>
        /// 制作一个Toggle
        /// </summary>
        protected void Toggle(bool value, out bool outValue, string name, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            bool newValue = EditorGUILayout.Toggle(name, value, options);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个Toggle
        /// </summary>
        protected void Toggle(bool value, out bool outValue, string name, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            bool newValue = EditorGUILayout.Toggle(name, value, style, options);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Set bool value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个IntSlider
        /// </summary>
        protected void IntSlider(int value, out int outValue, int leftValue, int rightValue, string name, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.IntSlider(name, value, leftValue, rightValue, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUILayout.Slider(name, value, leftValue, rightValue, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUILayout.IntField(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUILayout.FloatField(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUILayout.TextField(name, value, options);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Set string value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个PasswordField
        /// </summary>
        protected void PasswordField(string value, out string outValue, string name, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            string newValue = EditorGUILayout.PasswordField(name, value, options);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Set string value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个ObjectField
        /// </summary>
        protected void ObjectField<T>(T value, out T outValue, bool allowSceneObjects, string name, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            EditorGUI.BeginChangeCheck();
            T newValue = EditorGUILayout.ObjectField(name, value, typeof(T), allowSceneObjects, options) as T;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Set object value");
                outValue = newValue;
                HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个Vector2Field
        /// </summary>
        protected void Vector2Field(Vector2 value, out Vector2 outValue, string name, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            Vector2 newValue = EditorGUILayout.Vector2Field(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            Vector3 newValue = EditorGUILayout.Vector3Field(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            Enum newValue = EditorGUILayout.EnumPopup(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
            EditorGUI.BeginChangeCheck();
            Color newValue = EditorGUILayout.ColorField(name, value, options);
            if (EditorGUI.EndChangeCheck())
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
        /// 制作一个序列化属性字段
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="isLine">自动水平布局并占用一行</param>
        /// <param name="options">布局操作</param>
        protected void PropertyField(string propertyName, bool isLine = true, params GUILayoutOption[] options)
        {
            if (isLine) EditorGUILayout.BeginHorizontal();

            SerializedProperty serializedProperty = GetProperty(propertyName);
            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, true, options);
                DrawCopyPaste(serializedProperty);
            }
            else
            {
                EditorGUILayout.HelpBox($"Property [{propertyName}] not found!", MessageType.Error);
            }

            if (isLine) EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 制作一个序列化属性字段
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="name">显示名称</param>
        /// <param name="isLine">自动水平布局并占用一行</param>
        /// <param name="options">布局操作</param>
        protected void PropertyField(string propertyName, string name, bool isLine = true, params GUILayoutOption[] options)
        {
            if (isLine) EditorGUILayout.BeginHorizontal();

            SerializedProperty serializedProperty = GetProperty(propertyName);
            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name), true, options);
                DrawCopyPaste(serializedProperty);
            }
            else
            {
                EditorGUILayout.HelpBox($"Property [{propertyName}] not found!", MessageType.Error);
            }

            if (isLine) EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 制作一个序列化属性字段
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="includeChildren">包含子级</param>
        /// <param name="isLine">自动水平布局并占用一行</param>
        /// <param name="options">布局操作</param>
        protected void PropertyField(string propertyName, bool includeChildren, bool isLine = true, params GUILayoutOption[] options)
        {
            if (isLine) EditorGUILayout.BeginHorizontal();

            SerializedProperty serializedProperty = GetProperty(propertyName);
            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, includeChildren, options);
                DrawCopyPaste(serializedProperty);
            }
            else
            {
                EditorGUILayout.HelpBox($"Property [{propertyName}] not found!", MessageType.Error);
            }

            if (isLine) EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 制作一个序列化属性字段
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="name">显示名称</param>
        /// <param name="includeChildren">包含子级</param>
        /// <param name="isLine">自动水平布局并占用一行</param>
        /// <param name="options">布局操作</param>
        protected void PropertyField(string propertyName, string name, bool includeChildren, bool isLine = true, params GUILayoutOption[] options)
        {
            if (isLine) EditorGUILayout.BeginHorizontal();

            SerializedProperty serializedProperty = GetProperty(propertyName);
            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name), includeChildren, options);
                DrawCopyPaste(serializedProperty);
            }
            else
            {
                EditorGUILayout.HelpBox($"Property [{propertyName}] not found!", MessageType.Error);
            }

            if (isLine) EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 在属性字段的后面绘制复制、粘贴按钮
        /// </summary>
        /// <param name="property">属性</param>
        protected void DrawCopyPaste(SerializedProperty property)
        {
            if (!IsSupportCopyPaste(property))
                return;

            if (GUILayout.Button(CopyPasteGC, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                if (Targets.Length == 1)
                {
                    gm.AddItem(new GUIContent("Copy"), false, () =>
                    {
                        CopyValue(property);
                    });
                    gm.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        PasteValue(property);
                    });
                }
                else
                {
                    gm.AddDisabledItem(new GUIContent("Copy"));
                    gm.AddDisabledItem(new GUIContent("Paste"));
                }
                gm.ShowAsContext();
            }
        }
        /// <summary>
        /// 属性的类型是否支持复制粘贴
        /// </summary>
        private bool IsSupportCopyPaste(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Vector2
                || property.propertyType == SerializedPropertyType.Vector3
                || property.propertyType == SerializedPropertyType.Vector4
                || property.propertyType == SerializedPropertyType.Vector2Int
                || property.propertyType == SerializedPropertyType.Vector3Int
                || property.propertyType == SerializedPropertyType.Quaternion
                || property.propertyType == SerializedPropertyType.Bounds
                || property.propertyType == SerializedPropertyType.BoundsInt
                || (property.propertyType == SerializedPropertyType.Generic && property.hasChildren && property.type == "Location"))
                return true;
            return false;
        }
        /// <summary>
        /// 复制属性的值
        /// </summary>
        private void CopyValue(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                GUIUtility.systemCopyBuffer = property.vector2Value.ToCopyString("F4");
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                GUIUtility.systemCopyBuffer = property.vector3Value.ToCopyString("F4");
            }
            else if (property.propertyType == SerializedPropertyType.Vector4)
            {
                GUIUtility.systemCopyBuffer = property.vector4Value.ToCopyString("F4");
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                GUIUtility.systemCopyBuffer = property.vector2IntValue.ToCopyString();
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                GUIUtility.systemCopyBuffer = property.vector3IntValue.ToCopyString();
            }
            else if (property.propertyType == SerializedPropertyType.Quaternion)
            {
                GUIUtility.systemCopyBuffer = property.quaternionValue.ToCopyString("F4");
            }
            else if (property.propertyType == SerializedPropertyType.Bounds)
            {
                GUIUtility.systemCopyBuffer = property.boundsValue.ToCopyString("F4");
            }
            else if (property.propertyType == SerializedPropertyType.BoundsInt)
            {
                GUIUtility.systemCopyBuffer = property.boundsIntValue.ToCopyString();
            }
            else if (property.propertyType == SerializedPropertyType.Generic && property.hasChildren && property.type == "Location")
            {
                SerializedProperty position = property.FindPropertyRelative("Position");
                SerializedProperty rotation = property.FindPropertyRelative("Rotation");
                SerializedProperty scale = property.FindPropertyRelative("Scale");

                if (position != null && rotation != null && scale != null)
                {
                    Location location = new Location();
                    location.Position = position.vector3Value;
                    location.Rotation = rotation.vector3Value;
                    location.Scale = scale.vector3Value;
                    GUIUtility.systemCopyBuffer = location.LocationToJson();
                }
            }
        }
        /// <summary>
        /// 粘贴值到属性
        /// </summary>
        private void PasteValue(SerializedProperty property)
        {
            if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                return;

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                property.vector2Value = GUIUtility.systemCopyBuffer.ToPasteVector2(Vector3.zero);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                property.vector3Value = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.zero);
            }
            else if (property.propertyType == SerializedPropertyType.Vector4)
            {
                property.vector4Value = GUIUtility.systemCopyBuffer.ToPasteVector4(Vector4.zero);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                property.vector2IntValue = GUIUtility.systemCopyBuffer.ToPasteVector2Int(Vector2Int.zero);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                property.vector3IntValue = GUIUtility.systemCopyBuffer.ToPasteVector3Int(Vector3Int.zero);
            }
            else if (property.propertyType == SerializedPropertyType.Quaternion)
            {
                property.quaternionValue = GUIUtility.systemCopyBuffer.ToPasteQuaternion(Quaternion.identity);
            }
            else if (property.propertyType == SerializedPropertyType.Bounds)
            {
                property.boundsValue = GUIUtility.systemCopyBuffer.ToPasteBounds();
            }
            else if (property.propertyType == SerializedPropertyType.BoundsInt)
            {
                property.boundsIntValue = GUIUtility.systemCopyBuffer.ToPasteBoundsInt();
            }
            else if (property.propertyType == SerializedPropertyType.Generic && property.hasChildren && property.type == "Location")
            {
                SerializedProperty position = property.FindPropertyRelative("Position");
                SerializedProperty rotation = property.FindPropertyRelative("Rotation");
                SerializedProperty scale = property.FindPropertyRelative("Scale");

                if (position != null && rotation != null && scale != null)
                {
                    Location location = GUIUtility.systemCopyBuffer.JsonToLocation();
                    if (location != null)
                    {
                        position.vector3Value = location.Position;
                        rotation.vector3Value = location.Rotation;
                        scale.vector3Value = location.Scale;
                    }
                }
            }
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 创建可脚本化对象，并作为此主对象的子对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>对象实例</returns>
        protected T CreateSubScriptableObject<T>() where T : ScriptableObject
        {
            return CreateSubScriptableObject(typeof(T)) as T;
        }
        /// <summary>
        /// 创建可脚本化对象，并作为此主对象的子对象
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对象实例</returns>
        protected ScriptableObject CreateSubScriptableObject(Type type)
        {
            if (Targets.Length > 1)
                return null;

            ScriptableObject obj = CreateInstance(type);
            obj.name = type.FullName;
            if (Target is Component)
            {
                Component component = Target as Component;
                GameObject prefab = ScriptableToolkit.GetBelongPrefab(component.gameObject);
                if (prefab != null)
                {
                    ScriptableToolkit.SaveSubScriptableObject(obj, prefab);
                    return obj;
                }
                else
                {
                    return obj;
                }
            }
            else
            {
                ScriptableToolkit.SaveSubScriptableObject(obj, Target);
                return obj;
            }
        }
        /// <summary>
        /// 销毁可脚本化对象
        /// </summary>
        /// <param name="obj">对象</param>
        protected void DestroySubScriptableObject(ScriptableObject obj)
        {
            if (Targets.Length > 1)
                return;

            if (obj == null)
                return;

            if (Target is Component)
            {
                Component component = Target as Component;
                GameObject prefab = ScriptableToolkit.GetBelongPrefab(component.gameObject);
                if (prefab != null)
                {
                    ScriptableToolkit.DestroySubScriptableObject(obj, prefab);
                }
                else
                {
                    DestroyImmediate(obj);
                }
            }
            else
            {
                ScriptableToolkit.DestroySubScriptableObject(obj, Target);
            }
        }
    }
}