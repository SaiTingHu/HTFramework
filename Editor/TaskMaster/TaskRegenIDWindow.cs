using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class TaskRegenIDWindow : HTFEditorWindow
    {
        public static void ShowWindow(TaskEditorWindow taskEditorWindow, TaskContentAsset contentAsset)
        {
            TaskRegenIDWindow window = GetWindow<TaskRegenIDWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            window.titleContent.text = "Regen Task ID";
            window._taskEditorWindow = taskEditorWindow;
            window._contentAsset = contentAsset;
            window.minSize = new Vector2(220, 160);
            window.maxSize = new Vector2(220, 160);
            window.position = new Rect(taskEditorWindow.position.x + 50, taskEditorWindow.position.y + 50, 200, 110);
            window.Show();
        }

        private TaskEditorWindow _taskEditorWindow;
        private TaskContentAsset _contentAsset;
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
            GUILayout.Label("Task ID Name:", GUILayout.Width(90));
            _contentAsset.TaskIDName = EditorGUILayout.TextField(_contentAsset.TaskIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Task ID Sign:", GUILayout.Width(90));
            GUI.enabled = false;
            _contentAsset.TaskIDSign = EditorGUILayout.IntField(_contentAsset.TaskIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Point ID Name:", GUILayout.Width(90));
            _contentAsset.TaskPointIDName = EditorGUILayout.TextField(_contentAsset.TaskPointIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Point ID Sign:", GUILayout.Width(90));
            GUI.enabled = false;
            _contentAsset.TaskPointIDSign = EditorGUILayout.IntField(_contentAsset.TaskPointIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Start Index:", GUILayout.Width(90));
            _startIndex = EditorGUILayout.IntField(_startIndex);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Increment:", GUILayout.Width(90));
            _indexIncrement = EditorGUILayout.IntField(_indexIncrement);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview Task:", GUILayout.Width(90));
            GUILayout.Label(_contentAsset.TaskIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview Point:", GUILayout.Width(90));
            GUILayout.Label(_contentAsset.TaskPointIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Regen Task", EditorGlobalTools.Styles.ButtonLeft))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure regen all task id？", "Yes", "No"))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        _contentAsset.Content[i].GUID = _contentAsset.TaskIDName + index.ToString();
                        index += _indexIncrement;
                    }
                    _contentAsset.TaskIDSign = index;
                    EditorUtility.SetDirty(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button("Regen Point", EditorGlobalTools.Styles.ButtonMid))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure regen all task point id？", "Yes", "No"))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        for (int j = 0; j < _contentAsset.Content[i].Points.Count; j++)
                        {
                            TaskPointBase taskPoint = _contentAsset.Content[i].Points[j];
                            taskPoint.GUID = _contentAsset.TaskPointIDName + index.ToString();
                            index += _indexIncrement;
                        }
                    }
                    _contentAsset.TaskPointIDSign = index;
                    EditorUtility.SetDirty(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button("Cancel", EditorGlobalTools.Styles.ButtonRight))
            {
                EditorUtility.SetDirty(_contentAsset);
                Close();
            }
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (_taskEditorWindow == null || _contentAsset == null)
            {
                Close();
            }
        }
    }
}