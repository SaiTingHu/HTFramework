using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class ProjectWizard : HTFEditorWindow
    {
        private List<Folder> _rootFolders = new List<Folder>();
        private Folder _mainSceneFolder;
        private Folder _procedureFolder;
        private string _mainSceneName = "Main";
        private bool _isCreateInitialProcedure = false;
        private string _initialProcedure = "InitialProcedure";
        private Vector2 _scrollFolderGUI;

        protected override bool IsEnableTitleGUI => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            InitFolder();
        }
        private void InitFolder()
        {
            Folder pluginsfolder = new Folder(null, "Plugins");
            Folder sourcefolder = new Folder(null, "Source", "Animations", "Fonts", "Images", "Materials", "Models", "Prefabs", "Shaders", "Sounds");
            Folder scriptsfolder = new Folder(sourcefolder, "Scripts", "Entity", "Event", "FSM", "UI");
            Folder procedureFolder = new Folder(scriptsfolder, "Procedure");

            _rootFolders.Clear();
            _rootFolders.Add(pluginsfolder);
            _rootFolders.Add(sourcefolder);

            _mainSceneFolder = sourcefolder;
            _procedureFolder = procedureFolder;

            for (int i = 0; i < _rootFolders.Count; i++)
            {
                SetDefaultGuide(_rootFolders[i]);
            }
        }
        
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            FolderGUI();

            GUILayout.Space(10);

            SetupGUI();

            GUILayout.Space(10);

            GenerateGUI();
        }
        private void FolderGUI()
        {
            GUILayout.BeginVertical("Generate Project Folder", "Window");
            _scrollFolderGUI = GUILayout.BeginScrollView(_scrollFolderGUI);

            for (int i = 0; i < _rootFolders.Count; i++)
            {
                DrawFolder(_rootFolders[i], 0);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void DrawFolder(Folder folder, int level)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20 * level);
            GUIContent content = EditorGUIUtility.IconContent("Folder Icon");
            content.text = folder.Name;
            folder.IsExpansion = EditorGUILayout.Foldout(folder.IsExpansion, content, true);
            GUILayout.Space(60);
            GUI.color = folder.IsCreateGuide ? Color.white : Color.gray;
            folder.IsCreateGuide = GUILayout.Toggle(folder.IsCreateGuide, "Guide");
            if (folder.IsCreateGuide)
            {
                folder.GuideContent = EditorGUILayout.TextField(folder.GuideContent, GUILayout.Width(200));
            }
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (folder.IsExpansion)
            {
                for (int i = 0; i < folder.SubFolders.Count; i++)
                {
                    DrawFolder(folder.SubFolders[i], level + 1);
                }
            }
        }
        private void SetupGUI()
        {
            GUILayout.BeginVertical("Original Setup", "Window");
            
            GUILayout.BeginHorizontal();
            GUIContent content = EditorGUIUtility.IconContent("SceneAsset Icon");
            content.text = "Main Scene";
            GUILayout.Label(content, GUILayout.Height(18), GUILayout.Width(120));
            _mainSceneName = EditorGUILayout.TextField(_mainSceneName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            content = EditorGUIUtility.IconContent("Folder Icon");
            content.text = _mainSceneFolder != null ? _mainSceneFolder.Path : "<None>";
            if (GUILayout.Button(content, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _rootFolders.Count; i++)
                {
                    ChooseMainSceneFolderMenu(gm, _rootFolders[i]);
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUI.color = _isCreateInitialProcedure ? Color.white : Color.gray;
            _isCreateInitialProcedure = GUILayout.Toggle(_isCreateInitialProcedure, "Initial Procedure", GUILayout.Width(120));
            GUI.color = Color.white;
            GUI.enabled = _isCreateInitialProcedure;
            _initialProcedure = EditorGUILayout.TextField(_initialProcedure);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(125);
            content.text = _procedureFolder != null ? _procedureFolder.Path : "<None>";
            if (GUILayout.Button(content, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _rootFolders.Count; i++)
                {
                    ChooseProcedureFolderMenu(gm, _rootFolders[i]);
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
        private void ChooseMainSceneFolderMenu(GenericMenu gm, Folder folder)
        {
            gm.AddItem(new GUIContent(folder.Path.Replace("/", "\\")), _mainSceneFolder == folder, () =>
            {
                _mainSceneFolder = folder;
            });

            for (int i = 0; i < folder.SubFolders.Count; i++)
            {
                ChooseMainSceneFolderMenu(gm, folder.SubFolders[i]);
            }
        }
        private void ChooseProcedureFolderMenu(GenericMenu gm, Folder folder)
        {
            gm.AddItem(new GUIContent(folder.Path.Replace("/", "\\")), _procedureFolder == folder, () =>
            {
                _procedureFolder = folder;
            });

            for (int i = 0; i < folder.SubFolders.Count; i++)
            {
                ChooseProcedureFolderMenu(gm, folder.SubFolders[i]);
            }
        }
        private void GenerateGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = _mainSceneFolder != null && _mainSceneName != "";
            if (GUILayout.Button("Generate", EditorGlobalTools.Styles.LargeButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to generate by project wizard？", "Yes", "No"))
                {
                    Generate();
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void SetDefaultGuide(Folder folder)
        {
            if (folder.SubFolders.Count > 0)
            {
                for (int i = 0; i < folder.SubFolders.Count; i++)
                {
                    SetDefaultGuide(folder.SubFolders[i]);
                }
            }
            else
            {
                folder.SetGuide(true, string.Format("Put the {0} in this folder", folder.Name));
            }
        }
        private void Generate()
        {
            for (int i = 0; i < _rootFolders.Count; i++)
            {
                CreateFolder(_rootFolders[i]);
            }

            if (CreateMainScene())
            {
                CreateInitialProcedure();
                SaveScene();
            }
        }
        private void CreateFolder(Folder folder)
        {
            if (!AssetDatabase.IsValidFolder(folder.Path))
            {
                AssetDatabase.CreateFolder(folder.Parent != null ? folder.Parent.Path : "Assets", folder.Name);

                if (folder.IsCreateGuide)
                {
                    File.AppendAllText(folder.FullPath + "/Guide.txt", folder.GuideContent, Encoding.UTF8);
                }
            }

            for (int i = 0; i < folder.SubFolders.Count; i++)
            {
                CreateFolder(folder.SubFolders[i]);
            }
        }
        private bool CreateMainScene()
        {
            if (_mainSceneFolder != null && _mainSceneName != "")
            {
                Main main = FindObjectOfType<Main>();
                if (main == null)
                {
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>("Assets/HTFramework/HTFramework.prefab");
                    if (asset)
                    {
                        GameObject mainObj = PrefabUtility.InstantiatePrefab(asset) as GameObject;
                        mainObj.name = "HTFramework";
                        mainObj.transform.localPosition = Vector3.zero;
                        mainObj.transform.localRotation = Quaternion.identity;
                        mainObj.transform.localScale = Vector3.one;
                        return true;
                    }
                    else
                    {
                        Log.Error("新建框架主环境失败，丢失主预制体：Assets/HTFramework/HTFramework.prefab");
                    }
                }
                else
                {
                    Log.Error("已存在框架主环境，项目生成向导将不会再自动创建主场景！");
                }
            }
            return false;
        }
        private void CreateInitialProcedure()
        {
            if (_isCreateInitialProcedure)
            {
                if (_procedureFolder != null && _initialProcedure != "")
                {
                    string path = _procedureFolder.FullPath + "/" + _initialProcedure + ".cs";
                    if (!File.Exists(path))
                    {
                        TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/ProcedureTemplate.txt", typeof(TextAsset)) as TextAsset;
                        if (asset)
                        {
                            string code = asset.text;
                            code = code.Replace("新建流程", "初始流程（运行 " + _mainSceneName + " 场景会首先进入此流程）");
                            code = code.Replace("#SCRIPTNAME#", _initialProcedure);
                            File.AppendAllText(path, code, Encoding.UTF8);
                            asset = null;

                            Main main = FindObjectOfType<Main>();
                            ProcedureManager procedure = main.GetComponentByChild<ProcedureManager>("Procedure");
                            procedure.ActivatedProcedures.Add(_initialProcedure);
                            procedure.DefaultProcedure = _initialProcedure;
                        }
                    }
                    else
                    {
                        Log.Error("新建初始流程失败，已存在脚本 " + path);
                    }
                }
            }
        }
        private void SaveScene()
        {
            Main main = FindObjectOfType<Main>();
            EditorSceneManager.MarkSceneDirty(main.gameObject.scene);
            EditorSceneManager.SaveScene(main.gameObject.scene, _mainSceneFolder.Path + "/" + _mainSceneName + ".unity");
            AssetDatabase.Refresh();
            Close();
        }
        
        private class Folder
        {
            public Folder Parent;
            public List<Folder> SubFolders;
            public string Name;
            public string Path;
            public string FullPath;
            public bool IsCreateGuide = false;
            public string GuideContent;
            public bool IsExpansion = true;
            
            public Folder(Folder parent, string name, params string[] subs)
            {
                Parent = parent;
                SubFolders = new List<Folder>();
                Name = name;
                Path = Parent == null ? ("Assets/" + Name) : (Parent.Path + "/" + Name);
                FullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + Path;

                if (Parent != null)
                {
                    Parent.SubFolders.Add(this);
                }
                for (int i = 0; i < subs.Length; i++)
                {
                    Folder folder = new Folder(this, subs[i]);
                }
            }

            public void SetGuide(bool isCreate, string content)
            {
                IsCreateGuide = isCreate;
                GuideContent = content;
            }
        }
    }
}