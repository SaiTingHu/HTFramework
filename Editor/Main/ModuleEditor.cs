using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    public abstract class ModuleEditor : Editor
    {
        protected void HasChanged()
        {
            EditorUtility.SetDirty(target);
            Component component = target as Component;
            if (component != null && component.gameObject.scene != null)
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }

        //可撤销操作、根据改变SetDirty的控件
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
        protected void IntField(int value, out int outValue, params GUILayoutOption[] options)
        {
            int newValue = EditorGUILayout.IntField(value, options);
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
        /// 制作一个FloatField
        /// </summary>
        protected void FloatField(float value, out float outValue, params GUILayoutOption[] options)
        {
            float newValue = EditorGUILayout.FloatField(value, options);
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
        protected void TextField(string value, out string outValue, params GUILayoutOption[] options)
        {
            string newValue = EditorGUILayout.TextField(value, options);
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
        }
        /// <summary>
        /// 制作一个PasswordField
        /// </summary>
        protected void PasswordField(string value, out string outValue, params GUILayoutOption[] options)
        {
            string newValue = EditorGUILayout.PasswordField(value, options);
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
        }
        /// <summary>
        /// 制作一个ObjectField
        /// </summary>
        protected void ObjectField<T>(T value, out T outValue, bool allowSceneObjects, string name, params GUILayoutOption[] options) where T : Object
        {
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
    }
}
