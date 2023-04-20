using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// String值编辑器
    /// </summary>
    public sealed class StringValueEditor : HTFEditorWindow
    {
        /// <summary>
        /// 打开String值编辑器
        /// </summary>
        /// <param name="parasitifer">宿主窗口</param>
        /// <param name="value">值</param>
        /// <param name="title">窗口显示标题</param>
        /// <param name="editEnd">编辑完成回调</param>
        public static void OpenWindow(EditorWindow parasitifer, string value, string title, HTFAction<string> editEnd)
        {
            StringValueEditor valueEditor = GetWindow<StringValueEditor>();
            valueEditor.titleContent.image = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image;
            valueEditor.titleContent.text = "Value Editor";
            valueEditor._parasitifer = parasitifer;
            valueEditor._value = value;
            valueEditor._title = title;
            valueEditor._editEnd = editEnd;
            valueEditor.Show();
        }

        private EditorWindow _parasitifer;
        private string _value;
        private string _title;
        private HTFAction<string> _editEnd;
        private Vector2 _scroll;

        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.Label(_title);
            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll, "TextField");
            _value = EditorGUILayout.TextArea(_value, EditorGlobalTools.Styles.Label);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sure"))
            {
                _editEnd?.Invoke(_value);
                Close();
            }
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (_parasitifer == null || EditorApplication.isCompiling)
            {
                Close();
            }
        }
    }
}