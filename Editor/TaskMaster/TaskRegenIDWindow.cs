using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class TaskRegenIDWindow : HTFEditorWindow, ILocalizeWindow
    {
        public static void ShowWindow(TaskEditorWindow taskEditorWindow, TaskContentAsset contentAsset, Language language)
        {
            TaskRegenIDWindow window = GetWindow<TaskRegenIDWindow>();
            window.CurrentLanguage = language;
            window.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            window.titleContent.text = "Regen Task ID";
            window._taskEditorWindow = taskEditorWindow;
            window._contentAsset = contentAsset;
            window.minSize = new Vector2(220, 210);
            window.maxSize = new Vector2(220, 210);
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
            GUILayout.Label(GetWord("Task Content"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("  " + GetWord("ID Name") + ":", GUILayout.Width(90));
            _contentAsset.TaskIDName = EditorGUILayout.TextField(_contentAsset.TaskIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("  " + GetWord("ID Sign") + ":", GUILayout.Width(90));
            GUI.enabled = false;
            _contentAsset.TaskIDSign = EditorGUILayout.IntField(_contentAsset.TaskIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Task Point"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("  " + GetWord("ID Name") + ":", GUILayout.Width(90));
            _contentAsset.TaskPointIDName = EditorGUILayout.TextField(_contentAsset.TaskPointIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("  " + GetWord("ID Sign") + ":", GUILayout.Width(90));
            GUI.enabled = false;
            _contentAsset.TaskPointIDSign = EditorGUILayout.IntField(_contentAsset.TaskPointIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Start Index") + ":", GUILayout.Width(90));
            _startIndex = EditorGUILayout.IntField(_startIndex);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Increment") + ":", GUILayout.Width(90));
            _indexIncrement = EditorGUILayout.IntField(_indexIncrement);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Preview Task") + ":", GUILayout.Width(90));
            GUILayout.Label(_contentAsset.TaskIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Preview Point") + ":", GUILayout.Width(90));
            GUILayout.Label(_contentAsset.TaskPointIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetWord("Regen Task"), EditorGlobalTools.Styles.ButtonLeft))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure regen all task content id？", "Yes", "No"))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        _contentAsset.Content[i].GUID = _contentAsset.TaskIDName + index.ToString();
                        index += _indexIncrement;
                    }
                    _contentAsset.TaskIDSign = index;
                    HasChanged(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button(GetWord("Regen Point"), EditorGlobalTools.Styles.ButtonMid))
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
                    HasChanged(_contentAsset);
                    Close();
                }
            }
            if (GUILayout.Button(GetWord("Cancel"), EditorGlobalTools.Styles.ButtonRight))
            {
                HasChanged(_contentAsset);
                Close();
            }
            GUILayout.EndHorizontal();
        }
        protected override void GenerateWords()
        {
            base.GenerateWords();

            AddWord("任务内容", "Task Content");
            AddWord("身份号名称", "ID Name");
            AddWord("身份号标记", "ID Sign");
            AddWord("任务点", "Task Point");
            AddWord("开始索引", "Start Index");
            AddWord("增加的量", "Increment");
            AddWord("预览任务内容", "Preview Task");
            AddWord("预览任务点", "Preview Point");
            AddWord("生成任务内容", "Regen Task");
            AddWord("生成任务点", "Regen Point");
            AddWord("取消", "Cancel");
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