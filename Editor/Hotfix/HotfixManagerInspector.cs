using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(HotfixManager))]
    public sealed class HotfixManagerInspector : HTFEditor<HotfixManager>
    {
        private static readonly string SourceDllPath = "/Library/ScriptAssemblies/Hotfix.dll";
        private static readonly string AssetsDllPath = "/Assets/Hotfix/Hotfix.dll.bytes";

        private bool _hotfixIsCreated = false;
        private string _hotfixDirectory = "/Hotfix/";
        private string _hotfixEnvironmentPath = "/Hotfix/Environment/HotfixEnvironment.cs";
        private string _hotfixAssemblyDefinitionPath = "/Hotfix/Hotfix.asmdef";

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _hotfixIsCreated = false;
            string hotfixDirectory = Application.dataPath + _hotfixDirectory;
            string hotfixEnvironmentPath = Application.dataPath + _hotfixEnvironmentPath;
            string hotfixAssemblyDefinitionPath = Application.dataPath + _hotfixAssemblyDefinitionPath;
            if (Directory.Exists(hotfixDirectory))
            {
                if (File.Exists(hotfixEnvironmentPath))
                {
                    if (File.Exists(hotfixAssemblyDefinitionPath))
                    {
                        _hotfixIsCreated = true;
                    }
                }
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Hotfix manager, the hot update in this game!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableHotfix, out Target.IsEnableHotfix, "Is Enable Hotfix");
            GUILayout.EndHorizontal();

            if (Target.IsEnableHotfix)
            {
                #region HotfixDll
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                GUILayout.Label("HotfixDll AssetBundleName");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                TextField(Target.HotfixDllAssetBundleName, out Target.HotfixDllAssetBundleName);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("HotfixDll AssetsPath");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                TextField(Target.HotfixDllAssetsPath, out Target.HotfixDllAssetsPath);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                #endregion

                #region HotfixWizard
                if (_hotfixIsCreated)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Hotfix environment is Created!", MessageType.Info);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("box");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Hotfix Directory");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.TextField("Assets" + _hotfixDirectory);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Correct Hotfix Environment", "LargeButton"))
                    {
                        SetHotfixAssemblyDefinition(Application.dataPath + _hotfixAssemblyDefinitionPath);
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create Hotfix Environment", "LargeButton"))
                    {
                        CreateHotfixEnvironment();
                        _hotfixIsCreated = true;
                    }
                    GUILayout.EndHorizontal();
                }
                #endregion
            }
        }

        private void CreateHotfixEnvironment()
        {
            string hotfixDirectory = Application.dataPath + _hotfixDirectory;
            string hotfixEnvironmentPath = Application.dataPath + _hotfixEnvironmentPath;
            string hotfixAssemblyDefinitionPath = Application.dataPath + _hotfixAssemblyDefinitionPath;
            if (!Directory.Exists(hotfixDirectory))
            {
                Directory.CreateDirectory(hotfixDirectory);
            }
            if (!Directory.Exists(hotfixDirectory + "Environment/"))
            {
                Directory.CreateDirectory(hotfixDirectory + "Environment/");
            }
            if (!File.Exists(hotfixEnvironmentPath))
            {
                CreateHotfixEnvironment(hotfixEnvironmentPath);
            }
            if (!File.Exists(hotfixAssemblyDefinitionPath))
            {
                CreateHotfixAssemblyDefinition(hotfixAssemblyDefinitionPath);
            }
        }
        private void SetHotfixAssemblyDefinition(string filePath)
        {
            string contentOld = File.ReadAllText(filePath);
            JsonData json = GlobalTools.StringToJson(contentOld);
            json["name"] = "Hotfix";
            json["includePlatforms"] = new JsonData();
            json["includePlatforms"].Add("Editor");
            json["references"] = new JsonData();
            json["references"].Add("HTFramework.RunTime");
            string contentNew = GlobalTools.JsonToString(json);

            if (contentOld != contentNew)
            {
                File.WriteAllText(filePath, contentNew);
                AssetDatabase.Refresh();
                AssemblyDefinitionImporter importer = AssetImporter.GetAtPath("Assets" + _hotfixAssemblyDefinitionPath) as AssemblyDefinitionImporter;
                importer.SaveAndReimport();
            }
        }
        private void CreateHotfixAssemblyDefinition(string filePath)
        {
            JsonData json = new JsonData();
            json["name"] = "Hotfix";
            json["includePlatforms"] = new JsonData();
            json["includePlatforms"].Add("Editor");
            json["references"] = new JsonData();
            json["references"].Add("HTFramework.RunTime");

            File.WriteAllText(filePath, GlobalTools.JsonToString(json));
            AssetDatabase.Refresh();
            AssemblyDefinitionImporter importer = AssetImporter.GetAtPath("Assets" + _hotfixAssemblyDefinitionPath) as AssemblyDefinitionImporter;
            importer.SaveAndReimport();
        }
        private void CreateHotfixEnvironment(string filePath)
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/HotfixEnvironmentTemplate.txt", typeof(TextAsset)) as TextAsset;
            if (asset)
            {
                string code = asset.text;
                File.AppendAllText(filePath, code);
                asset = null;
                AssetDatabase.Refresh();
            }
        }

        [InitializeOnLoadMethod]
        private static void CopyHotfixDll()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                string sourcePath = GlobalTools.GetDirectorySameLevelOfAssets(SourceDllPath);
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, GlobalTools.GetDirectorySameLevelOfAssets(AssetsDllPath), true);
                    AssetDatabase.Refresh();
                    GlobalTools.LogInfo("更新：Assets/Hotfix/Hotfix.dll");
                }
            }
        }
    }
}
