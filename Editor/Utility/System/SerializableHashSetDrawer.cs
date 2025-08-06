using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomPropertyDrawer(typeof(SerializableHashSet<>))]
    internal sealed class SerializableHashSetDrawer : PropertyDrawer
    {
        private SerializedProperty _values;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObjects.Length > 1)
            {
                EditorGUI.HelpBox(position, "SerializableHashSet cannot be multi-edited.", MessageType.None);
                return;
            }

            InitDrawer(property);

            bool last = GUI.enabled;
            GUI.enabled = !EditorApplication.isPlaying;

            EditorGUI.PropertyField(position, _values, label, true);

            GUI.enabled = last;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_values != null)
            {
                return EditorGUI.GetPropertyHeight(_values);
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        private void InitDrawer(SerializedProperty property)
        {
            if (_values != null)
                return;

            _values = property.FindPropertyRelative("_values");
        }
    }
}