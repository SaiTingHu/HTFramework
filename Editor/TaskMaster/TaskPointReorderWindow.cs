using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务点重新排序窗口
    /// </summary>
    internal sealed class TaskPointReorderWindow : HTFEditorWindow
    {
        public static void ShowWindow(TaskContentBase taskContent)
        {
            TaskPointReorderWindow window = GetWindow<TaskPointReorderWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("AlphabeticalSorting").image;
            window.titleContent.text = "Reorder Task Point";
            window._taskContent = taskContent;
            window.minSize = new Vector2(250, 300);
            window.maxSize = new Vector2(250, 300);
            window.OnInit();
            window.Show();
        }

        private TaskContentBase _taskContent;
        private ReorderableList _taskPointsList;
        private Vector2 _scroll;

        protected override bool IsEnableTitleGUI => false;

        private void OnInit()
        {
            _taskPointsList = new ReorderableList(_taskContent.Points, typeof(TaskPointBase), true, false, false, false);
            _taskPointsList.headerHeight = 2;
            _taskPointsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < _taskContent.Points.Count)
                {
                    Rect subrect = rect;
                    subrect.Set(rect.x, rect.y + 2, rect.width, 16);
                    GUI.Label(subrect, $"{index + 1}.{_taskContent.Points[index].Name}");
                }
            };
            _taskPointsList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
            _taskPointsList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
            {
                if (oldIndex < newIndex)
                {
                    for (int i = 0; i < _taskContent.Depends.Count; i++)
                    {
                        TaskDepend depend = _taskContent.Depends[i];

                        if (depend.OriginalPoint == oldIndex) depend.OriginalPoint = newIndex;
                        else if (depend.OriginalPoint > oldIndex && depend.OriginalPoint <= newIndex) depend.OriginalPoint -= 1;

                        if (depend.DependPoint == oldIndex) depend.DependPoint = newIndex;
                        else if (depend.DependPoint > oldIndex && depend.DependPoint <= newIndex) depend.DependPoint -= 1;
                    }
                }
                else if (oldIndex > newIndex)
                {
                    for (int i = 0; i < _taskContent.Depends.Count; i++)
                    {
                        TaskDepend depend = _taskContent.Depends[i];

                        if (depend.OriginalPoint == oldIndex) depend.OriginalPoint = newIndex;
                        else if (depend.OriginalPoint < oldIndex && depend.OriginalPoint >= newIndex) depend.OriginalPoint += 1;

                        if (depend.DependPoint == oldIndex) depend.DependPoint = newIndex;
                        else if (depend.DependPoint < oldIndex && depend.DependPoint >= newIndex) depend.DependPoint += 1;
                    }
                }
                HasChanged(_taskContent);
            };
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll);
            _taskPointsList.DoLayoutList();
            GUILayout.EndScrollView();
        }
        private void Update()
        {
            if (_taskContent == null || _taskPointsList == null)
            {
                Close();
            }
        }
    }
}