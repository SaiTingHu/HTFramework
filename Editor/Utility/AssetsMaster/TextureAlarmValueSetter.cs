using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class TextureAlarmValueSetter : HTFEditorWindow
    {
        public static void ShowWindow(AssetsMaster assetsMaster, Vector2 pos)
        {
            TextureAlarmValueSetter window = GetWindow<TextureAlarmValueSetter>();
            window.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            window.titleContent.text = "Texture Alarm Value";
            window._assetsMaster = assetsMaster;
            window.minSize = new Vector2(250, 150);
            window.maxSize = new Vector2(250, 150);
            window.position = new Rect(pos.x, pos.y, 250, 150);
            window.Show();
        }

        private AssetsMaster _assetsMaster;

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
            GUILayout.Label("Mark Width", GUILayout.Width(120));
            _assetsMaster.IsMarkTextureWidth = EditorGUILayout.Toggle(_assetsMaster.IsMarkTextureWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _assetsMaster.IsMarkTextureWidth;
            GUILayout.Space(20);
            GUILayout.Label("Width > ", GUILayout.Width(105));
            _assetsMaster.TextureWidthUpper = EditorGUILayout.IntField(_assetsMaster.TextureWidthUpper);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mark Height", GUILayout.Width(120));
            _assetsMaster.IsMarkTextureHeight = EditorGUILayout.Toggle(_assetsMaster.IsMarkTextureHeight);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _assetsMaster.IsMarkTextureHeight;
            GUILayout.Space(20);
            GUILayout.Label("Height > ", GUILayout.Width(105));
            _assetsMaster.TextureHeightUpper = EditorGUILayout.IntField(_assetsMaster.TextureHeightUpper);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mark Crunched", GUILayout.Width(120));
            _assetsMaster.IsMarkTextureCrunched = EditorGUILayout.Toggle(_assetsMaster.IsMarkTextureCrunched);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mark MipMap", GUILayout.Width(120));
            _assetsMaster.IsMarkTextureMipMap = EditorGUILayout.Toggle(_assetsMaster.IsMarkTextureMipMap);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _assetsMaster.IsMarkTextureMipMap;
            GUILayout.Space(20);
            GUILayout.Label("MipMap Count > ", GUILayout.Width(105));
            _assetsMaster.TextureMipMapUpper = EditorGUILayout.IntField(_assetsMaster.TextureMipMapUpper);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (_assetsMaster == null)
            {
                Close();
            }
        }
    }
}