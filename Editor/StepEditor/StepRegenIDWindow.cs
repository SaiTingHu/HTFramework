using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class StepRegenIDWindow : HTFEditorWindow
    {
        public static void ShowWindow(StepEditorWindow stepEditorWindow, StepContentAsset contentAsset)
        {
            StepRegenIDWindow window = GetWindow<StepRegenIDWindow>();
            window.titleContent.text = "Regen Step ID";
            window._stepEditorWindow = stepEditorWindow;
            window._contentAsset = contentAsset;
            window.minSize = new Vector2(200, 110);
            window.maxSize = new Vector2(200, 110);
            window.position = new Rect(stepEditorWindow.position.x + 50, stepEditorWindow.position.y + 50, 200, 110);
            window.Show();
        }

        private StepEditorWindow _stepEditorWindow;
        private StepContentAsset _contentAsset;
        private int _startIndex = 1;
        private int _indexIncrement = 1;

        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID Name:", GUILayout.Width(80));
            _contentAsset.StepIDName = EditorGUILayout.TextField(_contentAsset.StepIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ID Sign:", GUILayout.Width(80));
            GUI.enabled = false;
            _contentAsset.StepIDSign = EditorGUILayout.IntField(_contentAsset.StepIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Start Index:", GUILayout.Width(80));
            _startIndex = EditorGUILayout.IntField(_startIndex);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Increment:", GUILayout.Width(80));
            _indexIncrement = EditorGUILayout.IntField(_indexIncrement);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview:", GUILayout.Width(80));
            GUILayout.Label(_contentAsset.StepIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Regen", "ButtonLeft"))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure regen all step id？", "Yes", "No"))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        _contentAsset.Content[i].GUID = _contentAsset.StepIDName + index.ToString();
                        index += _indexIncrement;
                    }
                    _contentAsset.StepIDSign = index;
                    EditorUtility.SetDirty(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button("Cancel", "ButtonRight"))
            {
                EditorUtility.SetDirty(_contentAsset);
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            if (_stepEditorWindow == null || _contentAsset == null)
            {
                Close();
            }
        }
    }
}
