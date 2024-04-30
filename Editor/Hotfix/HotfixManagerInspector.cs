using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(HotfixManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/90479971")]
    internal sealed class HotfixManagerInspector : InternalModuleInspector<HotfixManager, IHotfixHelper>
    {
        private static readonly string SourceDllPath = "Library/ScriptAssemblies/Hotfix.dll";
        private static readonly string AssetsDllPath = "Assets/Hotfix/Hotfix.dll.bytes";

        [InitializeOnLoadMethod]
        private static void CopyHotfixDll()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                string sourcePath = PathToolkit.ProjectPath + SourceDllPath;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, PathToolkit.ProjectPath + AssetsDllPath, true);
                    AssetDatabase.Refresh();
                    Log.Info("已更新：Assets/Hotfix/Hotfix.dll");
                }
            }
        }

        private bool _hotfixIsCreated = false;
        private string _hotfixDirectory = "/Hotfix/";
        private string _hotfixEnvironmentPath = "/Hotfix/Environment/HotfixEnvironment.cs";
        private string _hotfixAssemblyDefinitionPath = "/Hotfix/Hotfix.asmdef";

        protected override string Intro => "Hotfix manager, help you implement basic hot fixes in your game!";

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

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(HotfixManager.IsEnableHotfix), "Enable Hotfix");

            if (Target.IsEnableHotfix)
            {
                #region HotfixDll
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                GUILayout.BeginHorizontal();
                GUILayout.Label("HotfixDll AssetBundleName");
                GUILayout.EndHorizontal();

                PropertyField(nameof(HotfixManager.HotfixDllAssetBundleName), "");
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("HotfixDll AssetsPath");
                GUILayout.EndHorizontal();

                PropertyField(nameof(HotfixManager.HotfixDllAssetsPath), "");
                
                GUILayout.EndVertical();
                #endregion

                #region HotfixWizard
                if (_hotfixIsCreated)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Hotfix environment is Created!", MessageType.Info);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Hotfix Directory");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.TextField("Assets" + _hotfixDirectory);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button("Correct Hotfix Environment", EditorGlobalTools.Styles.LargeButton))
                    {
                        SetHotfixAssemblyDefinition(Application.dataPath + _hotfixAssemblyDefinitionPath);
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Create Hotfix Environment", EditorGlobalTools.Styles.LargeButton))
                    {
                        CreateHotfixEnvironment();
                        _hotfixIsCreated = true;
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
                #endregion
            }

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
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
            JsonData json = JsonToolkit.StringToJson(contentOld);
            json["name"] = "Hotfix";
            json["includePlatforms"] = new JsonData();
            json["includePlatforms"].Add("Editor");
            json["references"] = new JsonData();
            json["references"].Add("HTFramework.RunTime");
            json["autoReferenced"] = false;
            string contentNew = JsonToolkit.JsonToString(json);

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
            json["autoReferenced"] = false;

            File.WriteAllText(filePath, JsonToolkit.JsonToString(json));
            AssetDatabase.Refresh();
            AssemblyDefinitionImporter importer = AssetImporter.GetAtPath("Assets" + _hotfixAssemblyDefinitionPath) as AssemblyDefinitionImporter;
            importer.SaveAndReimport();
        }
        private void CreateHotfixEnvironment(string filePath)
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath(EditorPrefsTable.ScriptTemplateFolder + "HotfixEnvironmentTemplate.txt", typeof(TextAsset)) as TextAsset;
            if (asset)
            {
                string code = asset.text;
                File.AppendAllText(filePath, code, Encoding.UTF8);
                AssetDatabase.Refresh();
            }
        }
    }
}