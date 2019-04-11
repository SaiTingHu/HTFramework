using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class TextureImportSettingsBatch : EditorWindow
    {
        private static readonly string[] _supportedFormat = new string[] { ".jpg", ".JPG", ".png", ".PNG" };

        private TextureImporterType _importerType = TextureImporterType.Default;
        private string _packingTag = "";
        private bool _generateMipMap = false;
        private bool _useCrunchCompre = false;
        private int _number = 0;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("设置选中Texture或选中路径下的所有Texture属性", MessageType.Info);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Texture Type：", GUILayout.Width(150));
            _importerType = (TextureImporterType)EditorGUILayout.EnumPopup(_importerType);
            EditorGUILayout.EndHorizontal();

            if (_importerType == TextureImporterType.Sprite)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Packing Tag：", GUILayout.Width(150));
                _packingTag = EditorGUILayout.TextField(_packingTag);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Generate Mip Map：", GUILayout.Width(150));
            _generateMipMap = EditorGUILayout.Toggle(_generateMipMap);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Use Crunch Compre：", GUILayout.Width(150));
            _useCrunchCompre = EditorGUILayout.Toggle(_useCrunchCompre);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set"))
            {
                _number = 0;
                Object[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
                for (int i = 0; i < textures.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(textures[i]);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    textureImporter.textureType = _importerType;
                    textureImporter.mipmapEnabled = _generateMipMap;
                    textureImporter.crunchedCompression = _useCrunchCompre;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        textureImporter.spritePackingTag = _packingTag;
                    }
                    AssetDatabase.ImportAsset(path);
                    _number += 1;
                }
                AssetDatabase.Refresh();
                GlobalTools.LogInfo("设置完成！共设置了 " + _number + " 张 Texture！");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Only Set Property"))
            {
                _number = 0;
                Object[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
                for (int i = 0; i < textures.Length; i++)
                {
                    string path = AssetDatabase.GetAssetPath(textures[i]);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    textureImporter.mipmapEnabled = _generateMipMap;
                    textureImporter.crunchedCompression = _useCrunchCompre;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        textureImporter.spritePackingTag = _packingTag;
                    }
                    AssetDatabase.ImportAsset(path);
                    _number += 1;
                }
                AssetDatabase.Refresh();
                GlobalTools.LogInfo("设置完成！共设置了 " + _number + " 张 Texture！");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
