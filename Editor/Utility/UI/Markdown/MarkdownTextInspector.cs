using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MarkdownText))]
    internal sealed class MarkdownTextInspector : UnityEditor.UI.TextEditor
    {
        private static bool _isFoldout = false;

        private MarkdownText _markdownText;
        private SerializedProperty _isParseInAwake;
        private SerializedProperty _isHyperlinkUnderline;
        private SerializedProperty _tableRowHeight;
        private SerializedProperty _spriteAssets;
        private SerializedProperty _tableTemplate;
        private SerializedProperty _onClickHyperlink;
        private SerializedProperty _onClickEmbedTexture;
        private GUIContent _isParseInAwakeGC;
        private GUIContent _isHyperlinkUnderlineGC;

        protected override void OnEnable()
        {
            base.OnEnable();

            _markdownText = target as MarkdownText;
            _isParseInAwake = serializedObject.FindProperty("IsParseInAwake");
            _isHyperlinkUnderline = serializedObject.FindProperty("IsHyperlinkUnderline");
            _tableRowHeight = serializedObject.FindProperty("TableRowHeight");
            _spriteAssets = serializedObject.FindProperty("SpriteAssets");
            _tableTemplate = serializedObject.FindProperty("TableTemplate");
            _onClickHyperlink = serializedObject.FindProperty("OnClickHyperlink");
            _onClickEmbedTexture = serializedObject.FindProperty("OnClickEmbedTexture");
            _isParseInAwakeGC = new GUIContent("Parse In Awake");
            _isHyperlinkUnderlineGC = new GUIContent("Hyperlink Underline");

            if (!EditorApplication.isPlaying)
            {
                if (_markdownText.TableTemplate == null)
                {
                    GameObject temp = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HTFramework/RunTime/Utility/UI/Markdown/MarkdownTable.prefab");
                    _markdownText.TableTemplate = temp.GetComponent<MarkdownTable>();
                    EditorUtility.SetDirty(_markdownText);
                }
            }
        }
        public override void OnInspectorGUI()
        {
            _isFoldout = EditorGUILayout.Foldout(_isFoldout, "Markdown", true);
            if (_isFoldout)
            {
                EditorGUI.indentLevel = 1;

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_isParseInAwake, _isParseInAwakeGC);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_isHyperlinkUnderline, _isHyperlinkUnderlineGC);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_tableRowHeight);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_spriteAssets);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_tableTemplate);
                GUILayout.EndHorizontal();

                if (EditorApplication.isPlaying)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Raw Text");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.TextArea(_markdownText.RawText, GUILayout.Height(60));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Pure Text");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.TextArea(_markdownText.PureText, GUILayout.Height(60));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_onClickHyperlink);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_onClickEmbedTexture);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;

                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}