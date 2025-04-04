﻿using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/88852698")]
    internal sealed class ResourceManagerInspector : InternalModuleInspector<ResourceManager, IResourceHelper>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(ResourceManager.Mode), "Load Mode");

            if (Target.Mode == ResourceLoadMode.Resource)
            {
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Resources Folder View", EditorGlobalTools.Styles.LargeButton))
                {
                    ResourcesFolderViewWindow window = EditorWindow.GetWindow<ResourcesFolderViewWindow>();
                    window.titleContent.image = EditorGUIUtility.IconContent("ViewToolOrbit").image;
                    window.titleContent.text = "Resources Folder View";
                    window.position = new Rect(200, 200, 400, 400);
                    window.Init();
                    window.Show();
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                if (Target.HelperType == "HT.Framework.AddressablesHelper")
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Error: The AddressablesHelper is not capable of Resource mode!");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
            }
            else if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                PropertyField(nameof(ResourceManager.AssetBundleManifestName), "Manifest Name");
                PropertyField(nameof(ResourceManager.IsEditorMode), "Editor Mode");

                if (Target.HelperType == "HT.Framework.AddressablesHelper")
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Error: The AddressablesHelper is not capable of AssetBundle mode!");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
            }
            else if (Target.Mode == ResourceLoadMode.Addressables)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Addressables", GUILayout.Width(LabelWidth));
#if UNITY_ADDRESSABLES_1_20
                GUI.color = Color.cyan;
                EditorGUILayout.LabelField("Installed");
                GUI.color = Color.white;
#else
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Removed");
                GUI.color = Color.white;
#endif
                GUILayout.EndHorizontal();

                if (Target.HelperType == "HT.Framework.DefaultResourceHelper")
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Error: The DefaultResourceHelper is not capable of Addressables mode!");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
            }

            PropertyField(nameof(ResourceManager.IsLogDetail), "Is Log Detail");

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
                return;
            }

            if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                if (!Target.IsEditorMode)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Root Path: ", GUILayout.Width(LabelWidth));
                    EditorGUILayout.TextField(_helper.AssetBundleRootPath);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Manifest: ", GUILayout.Width(LabelWidth));
                    EditorGUILayout.ObjectField(_helper.Manifest, typeof(AssetBundleManifest), false);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("AssetBundles: ", GUILayout.Width(LabelWidth));
                    GUILayout.Label(_helper.AssetBundles.Count.ToString());
                    GUILayout.EndHorizontal();

                    foreach (var item in _helper.AssetBundles)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(item.Key, GUILayout.Width(LabelWidth - 20));
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