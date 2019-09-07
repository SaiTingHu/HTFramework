using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    public sealed class ResourceManagerInspector : HTFEditor<ResourceManager>
    {
        private string _assetBundleRootPath;
        private Dictionary<string, AssetBundle> _assetBundles;
        private AssetBundleManifest _assetBundleManifest;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _assetBundleRootPath = (string)Target.GetType().GetField("_assetBundleRootPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target);
            _assetBundles = Target.GetType().GetField("_assetBundles", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, AssetBundle>;
            _assetBundleManifest = Target.GetType().GetField("_assetBundleManifest", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as AssetBundleManifest;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Resource Manager, Manage all resource loading and unloading!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.Mode, out Target.Mode, "Load Mode");
            GUILayout.EndHorizontal();

            if (Target.Mode == ResourceLoadMode.Resource)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Resources Folder View", "LargeButton"))
                {
                    ResourcesFolderViewWindow window = EditorWindow.GetWindow<ResourcesFolderViewWindow>();
                    window.titleContent.text = "Resources Folder View";
                    window.position = new Rect(200, 200, 400, 400);
                    window.Init();
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
            else if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                TextField(Target.AssetBundleManifestName, out Target.AssetBundleManifestName, "Manifest Name");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEditorMode, out Target.IsEditorMode, "Editor Mode");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsCacheAssetBundle, out Target.IsCacheAssetBundle, "Cache AssetBundle");
                GUILayout.EndHorizontal();
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                if (!Target.IsEditorMode)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Root Path: ", GUILayout.Width(100));
                    EditorGUILayout.TextField(_assetBundleRootPath);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Manifest: ", GUILayout.Width(100));
                    EditorGUILayout.ObjectField(_assetBundleManifest, typeof(AssetBundleManifest), false);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("AssetBundles: ", GUILayout.Width(100));
                    GUILayout.Label(_assetBundles.Count.ToString());
                    GUILayout.EndHorizontal();

                    foreach (KeyValuePair<string, AssetBundle> item in _assetBundles)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(item.Key, GUILayout.Width(80));
                        EditorGUILayout.ObjectField(item.Value, typeof(AssetBundle), false);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("No Runtime Data!");
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }
        }
    }
}