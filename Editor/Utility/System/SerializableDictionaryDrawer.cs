using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    internal sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        private GUIContent _keyGC;
        private GUIContent _valueGC;
        private SerializedProperty _keys;
        private SerializedProperty _values;
        private ReorderableList _list;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObjects.Length > 1)
            {
                EditorGUI.HelpBox(position, "SerializableDictionary cannot be multi-edited.", MessageType.None);
                return;
            }

            InitDrawer(property);

            Rect sub = position;
            sub.Set(position.x, position.y, position.width - 45, 20);
            label.text += $" [{_keys.arraySize}]";
            property.isExpanded = EditorGUI.Foldout(sub, property.isExpanded, label, true);

            sub.Set(position.x + position.width - 45, position.y + 1, 45, 18);
            if (GUI.Button(sub, "Clear"))
            {
                _keys.ClearArray();
                _values.ClearArray();
            }

            if (property.isExpanded)
            {
                bool last = GUI.enabled;
                GUI.enabled = !EditorApplication.isPlaying;

                if (_keys.arraySize == _values.arraySize)
                {
                    sub.Set(position.x, position.y + 20, position.width, position.height - 20);
                    _list.DoList(sub);
                }
                else
                {
                    sub.Set(position.x, position.y + 20, position.width - 45, 20);
                    GUI.color = Color.red;
                    EditorGUI.LabelField(sub, EditorGUIUtility.TrTextContent("The data in the dictionary is incorrect, please clear the data."));
                    GUI.color = Color.white;
                }

                GUI.enabled = last;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded && _keys != null && _values != null)
            {
                if (_keys.arraySize == _values.arraySize)
                {
                    return _list.GetHeight() + 20;
                }
                else
                {
                    return 40;
                }
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        private void InitDrawer(SerializedProperty property)
        {
            if (_list != null)
                return;

            _keyGC = new GUIContent("Key");
            _valueGC = new GUIContent("Value");

            _keys = property.FindPropertyRelative("_keys");
            _values = property.FindPropertyRelative("_values");

            _list = new ReorderableList(property.serializedObject, _keys, false, false, true, true);
            _list.footerHeight = 20;
            _list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect sub = rect;

                Color last = GUI.color;
                if (!EditorApplication.isPlaying)
                {
                    GUI.color = isActive ? last : Color.gray;
                }

                SerializedProperty key = _keys.GetArrayElementAtIndex(index);
                sub.Set(rect.x, rect.y + 2, rect.width, 20);
                EditorGUI.PropertyField(sub, key, _keyGC, true);
                float keyHeight = EditorGUI.GetPropertyHeight(key);

                SerializedProperty value = _values.GetArrayElementAtIndex(index);
                sub.Set(rect.x, rect.y + keyHeight + 4, rect.width, 20);
                if (value.hasVisibleChildren)
                {
                    sub.x += 12;
                    sub.width -= 12;
                }
                EditorGUI.PropertyField(sub, value, _valueGC, true);

                if (!EditorApplication.isPlaying)
                {
                    GUI.color = last;
                }
            };
            _list.drawNoneElementCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, EditorGUIUtility.TrTextContent("Dictionary is Empty"));
            };
            _list.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                    gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                    rect.x += 2;
                    rect.width -= 6;
                    gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
            _list.elementHeightCallback = (int index) =>
            {
                float keyHeight = EditorGUI.GetPropertyHeight(_keys.GetArrayElementAtIndex(index)) + 2;
                float valueHeight = EditorGUI.GetPropertyHeight(_values.GetArrayElementAtIndex(index)) + 2;
                return keyHeight + valueHeight;
            };
            _list.onAddCallback = (list) =>
            {
                int index = _keys.arraySize - 1;
                if (index < 0) index = 0;
                _keys.InsertArrayElementAtIndex(index);
                _values.InsertArrayElementAtIndex(index);
            };
            _list.onRemoveCallback = (list) =>
            {
                _values.DeleteArrayElementAtIndex(list.index);
                _keys.DeleteArrayElementAtIndex(list.index);
            };
        }
    }
}