using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class StepRegenIDWindow : EditorWindow
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
        private string _name = "Step";
        private int _startIndex = 1;
        private int _indexIncrement = 1;
        private int _indexDigit = 3;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID Name:", GUILayout.Width(80));
            _name = EditorGUILayout.TextField(_name);
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
            GUILayout.Label("Digit:", GUILayout.Width(80));
            _indexDigit = EditorGUILayout.IntField(_indexDigit);
            if (_indexDigit < 1) _indexDigit = 1;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview:", GUILayout.Width(80));
            GUILayout.Label(_name + _startIndex.ToString().PadLeft(_indexDigit, '0'));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Regen", "ButtonLeft"))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure regen all step id？", "Yes", "No"))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        _contentAsset.Content[i].GUID = _name + index.ToString().PadLeft(_indexDigit, '0');
                        index += _indexIncrement;
                    }
                    EditorUtility.SetDirty(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button("Cancel", "ButtonRight"))
            {
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
