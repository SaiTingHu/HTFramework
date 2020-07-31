using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class ProjectWizard : HTFEditorWindow
    {
        private List<Folder> _rootFolders = new List<Folder>();
        private Folder _mainSceneFolder;
        private string _mainSceneName;
        private Vector2 _scrollFolderGUI;

        protected override bool IsEnableTitleGUI => false;

        private void OnEnable()
        {
            InitFolder();

            _mainSceneName = "Main";
        }
        private void InitFolder()
        {
            Folder pluginsfolder = new Folder(null, "Plugins");
            Folder sourcefolder = new Folder(null, "Source", "Animations", "Fonts", "Images", "Materials", "Models", "Prefabs", "Shaders", "Sounds");
            Folder scriptsfolder = new Folder(sourcefolder, "Scripts", "Entity", "Event", "FSM", "UI", "Procedure");

            _rootFolders.Clear();
            _rootFolders.Add(pluginsfolder);
            _rootFolders.Add(sourcefolder);

            _mainSceneFolder = sourcefolder;
        }
        
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            FolderGUI();
            SetupGUI();
        }
        private void FolderGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Generate Project Folder", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("TextField");
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
            GUILayout.BeginHorizontal();
            GUILayout.Label("Original Setup", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("TextField");

            GUI.color = _mainSceneFolder != null ? Color.white : Color.gray;

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
                    ChooseFolderMenu(gm, _rootFolders[i]);
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            
            GUI.color = Color.white;

            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate", EditorGlobalTools.Styles.LargeButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to generate by project wizard？", "Yes", "No"))
                {
                    Generate();
                }
            }
            GUILayout.EndHorizontal();
        }
        private void ChooseFolderMenu(GenericMenu gm, Folder folder)
        {
            gm.AddItem(new GUIContent(folder.Path.Replace("/", "\\")), _mainSceneFolder == folder, () =>
            {
                _mainSceneFolder = folder;
            });

            for (int i = 0; i < folder.SubFolders.Count; i++)
            {
                ChooseFolderMenu(gm, folder.SubFolders[i]);
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
                SaveScene();
            }
        }
        private void CreateFolder(Folder folder)
        {
            if (!AssetDatabase.IsValidFolder(folder.Path))
            {
                AssetDatabase.CreateFolder(folder.Parent != null ? folder.Parent.Path : "Assets", folder.Name);
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
            }
            return false;
        }
        private void SaveScene()
        {
            Main main = FindObjectOfType<Main>();
            EditorSceneManager.MarkSceneDirty(main.gameObject.scene);
            EditorSceneManager.SaveScene(main.gameObject.scene, _mainSceneFolder.Path + "/" + _mainSceneName + ".unity");
            AssetDatabase.Refresh();
        }

        public class Folder
        {
            public Folder Parent;
            public List<Folder> SubFolders = new List<Folder>();
            public string Name;
            public string Path;
            public bool IsExpansion = true;
            
            public Folder(Folder parent, string name, params string[] subs)
            {
                Parent = parent;
                Name = name;
                Path = Parent == null ? ("Assets/" + Name) : (Parent.Path + "/" + Name);

                if (Parent != null)
                {
                    Parent.SubFolders.Add(this);
                }
                for (int i = 0; i < subs.Length; i++)
                {
                    Folder folder = new Folder(this, subs[i]);
                }
            }
        }
    }
}