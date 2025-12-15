using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ScrollList))]
    internal sealed class ScrollListInspector : UnityEditor.UI.ScrollRectEditor
    {
        private static bool _isFoldoutElement = false;

        private SerializedProperty _elementTemplate;
        private SerializedProperty _scrollDirection;
        private SerializedProperty _stayPos;
        private SerializedProperty _firstPosition;
        private SerializedProperty _rowNumber;
        private SerializedProperty _spacing;
        private SerializedProperty _horizontal;
        private SerializedProperty _vertical;

        protected override void OnEnable()
        {
            base.OnEnable();

            _elementTemplate = serializedObject.FindProperty("ElementTemplate");
            _scrollDirection = serializedObject.FindProperty("ScrollDirection");
            _stayPos = serializedObject.FindProperty("StayPos");
            _firstPosition = serializedObject.FindProperty("FirstPosition");
            _rowNumber = serializedObject.FindProperty("RowNumber");
            _spacing = serializedObject.FindProperty("Spacing");
            _horizontal = serializedObject.FindProperty("m_Horizontal");
            _vertical = serializedObject.FindProperty("m_Vertical");
        }
        public override void OnInspectorGUI()
        {
            _isFoldoutElement = EditorGUILayout.Foldout(_isFoldoutElement, "Element", true);
            if (_isFoldoutElement)
            {
                EditorGUI.indentLevel = 1;

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_elementTemplate);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_scrollDirection);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_stayPos);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_firstPosition);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_rowNumber);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_spacing);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;

                GUILayout.Space(5);

                if (GUI.changed)
                {
                    _horizontal.boolValue = _scrollDirection.intValue == 0;
                    _vertical.boolValue = _scrollDirection.intValue == 1;
                }

                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}