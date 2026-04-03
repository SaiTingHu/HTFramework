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
        private SerializedProperty _hyperlinkColor;
        private SerializedProperty _isUseUnderlineEmphasize;
        private SerializedProperty _textureMinSize;
        private SerializedProperty _textureMaxSize;
        private SerializedProperty _tableRowHeight;
        private SerializedProperty _spriteAssets;
        private SerializedProperty _tableTemplate;
        private SerializedProperty _onClickHyperlink;
        private SerializedProperty _onClickEmbedTexture;
        private SerializedProperty _onClickTable;
        private GUIContent _isParseInAwakeGC;
        private GUIContent _isHyperlinkUnderlineGC;
        private GUIContent _isUseUnderlineEmphasizeGC;

        protected override void OnEnable()
        {
            base.OnEnable();

            _markdownText = target as MarkdownText;
            _isParseInAwake = serializedObject.FindProperty("IsParseInAwake");
            _isHyperlinkUnderline = serializedObject.FindProperty("IsHyperlinkUnderline");
            _hyperlinkColor = serializedObject.FindProperty("HyperlinkColor");
            _isUseUnderlineEmphasize = serializedObject.FindProperty("IsUseUnderlineEmphasize");
            _textureMinSize = serializedObject.FindProperty("TextureMinSize");
            _textureMaxSize = serializedObject.FindProperty("TextureMaxSize");
            _tableRowHeight = serializedObject.FindProperty("TableRowHeight");
            _spriteAssets = serializedObject.FindProperty("SpriteAssets");
            _tableTemplate = serializedObject.FindProperty("TableTemplate");
            _onClickHyperlink = serializedObject.FindProperty("OnClickHyperlink");
            _onClickEmbedTexture = serializedObject.FindProperty("OnClickEmbedTexture");
            _onClickTable = serializedObject.FindProperty("OnClickTable");
            _isParseInAwakeGC = new GUIContent("Parse In Awake");
            _isHyperlinkUnderlineGC = new GUIContent("Hyperlink Underline");
            _isUseUnderlineEmphasizeGC = new GUIContent("Use _ Emphasize");

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
                EditorGUILayout.PropertyField(_hyperlinkColor);
                if (GUILayout.Button("Preset", GUILayout.Width(50)))
                {
                    GenericMenu gm = new GenericMenu();
                    GenarateColorMenuItem(gm, "red", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "green", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "blue", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "white", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "black", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "yellow", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "cyan", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "magenta", _hyperlinkColor);
                    GenarateColorMenuItem(gm, "gray", _hyperlinkColor);
                    gm.ShowAsContext();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_isUseUnderlineEmphasize, _isUseUnderlineEmphasizeGC);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_textureMinSize);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_textureMaxSize);
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

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_onClickTable);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel = 0;

                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }

        private void GenarateColorMenuItem(GenericMenu genericMenu, string color, SerializedProperty property)
        {
            genericMenu.AddItem(new GUIContent(color), property.stringValue == color, () =>
            {
                property.stringValue = color;
                serializedObject.ApplyModifiedProperties();
            });
        }
    }
}