using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class StepRegenIDWindow : HTFEditorWindow, ILocalizeWindow
    {
        public static void ShowWindow(StepEditorWindow stepEditorWindow, StepContentAsset contentAsset, Language language)
        {
            StepRegenIDWindow window = GetWindow<StepRegenIDWindow>();
            window.CurrentLanguage = language;
            window.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
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
            GUILayout.Label(GetWord("ID Name") + ":", GUILayout.Width(80));
            _contentAsset.StepIDName = EditorGUILayout.TextField(_contentAsset.StepIDName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("ID Sign") + ":", GUILayout.Width(80));
            GUI.enabled = false;
            _contentAsset.StepIDSign = EditorGUILayout.IntField(_contentAsset.StepIDSign);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Start Index") + ":", GUILayout.Width(80));
            _startIndex = EditorGUILayout.IntField(_startIndex);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Increment") + ":", GUILayout.Width(80));
            _indexIncrement = EditorGUILayout.IntField(_indexIncrement);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(GetWord("Preview") + ":", GUILayout.Width(80));
            GUILayout.Label(_contentAsset.StepIDName + _startIndex.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetWord("Regen"), EditorGlobalTools.Styles.ButtonLeft))
            {
                string prompt = CurrentLanguage == Language.English ? "Are you sure regen all step id？" : "你确定要重新生成所有步骤的身份号吗？";
                if (EditorUtility.DisplayDialog(GetWord("Prompt"), prompt, GetWord("Yes"), GetWord("No")))
                {
                    int index = _startIndex;
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        _contentAsset.Content[i].GUID = _contentAsset.StepIDName + index.ToString();
                        index += _indexIncrement;
                    }
                    _contentAsset.StepIDSign = index;
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

            AddWord("身份号名称", "ID Name");
            AddWord("身份号标记", "ID Sign");
            AddWord("开始索引", "Start Index");
            AddWord("增加的量", "Increment");
            AddWord("预览", "Preview");
            AddWord("重新生成", "Regen");
            AddWord("提示", "Prompt");
            AddWord("是的", "Yes");
            AddWord("不", "No");
            AddWord("取消", "Cancel");
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