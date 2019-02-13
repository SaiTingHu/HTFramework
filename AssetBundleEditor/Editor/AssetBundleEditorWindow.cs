using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace HT.Framework.AssetBundleEditor
{
    public sealed class AssetBundleEditorWindow : EditorWindow
    {
        [MenuItem("HTFramework/AssetBundle/AssetBundle Editor")]
        private static void OpenAssetBundleWindow()
        {
            AssetBundleEditorWindow abEditor = GetWindow<AssetBundleEditorWindow>("AssetBundles");
            abEditor.Init();
            abEditor.Show();
        }

        #region fields
        private FolderInfo _assetRootFolder;
        private Dictionary<string, FileInfo> _validFilesCache;
        private AssetBundleInfo _assetBundleInfo;
        private FileInfo _currentFile;
        private int _currentAB = -1;
        private int _currentABAsset = -1;
        private bool _isRename = false;
        private string _renameValue = "";
        private string _variant = "";

        private Rect _ABViewRect;
        private Rect _ABScrollRect;
        private Vector2 _ABScroll;
        private int _ABViewHeight = 0;

        private Rect _currentABViewRect;
        private Rect _currentABScrollRect;
        private Vector2 _currentABScroll;
        private int _currentABViewHeight = 0;

        private Rect _assetViewRect;
        private Rect _assetScrollRect;
        private Vector2 _assetScroll;
        private int _assetViewHeight = 0;

        private Rect _dependenciesViewRect;
        private Rect _dependenciesScrollRect;
        private Vector2 _dependenciesScroll;
        private int _dependenciesViewHeight = 0;

        private bool _hideInvalidAsset = false;
        private bool _hideBundleAsset = false;

        private string _buildPath = "";
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;

        private GUIStyle _box = new GUIStyle("Box");
        private GUIStyle _preButton = new GUIStyle("PreButton");
        private GUIStyle _preDropDown = new GUIStyle("PreDropDown");
        private GUIStyle _LRSelect = new GUIStyle("LODSliderRangeSelected");
        private GUIStyle _prefabLabel = new GUIStyle("PR PrefabLabel");
        private GUIStyle _brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
        private GUIStyle _miniButtonLeft = new GUIStyle("MiniButtonLeft");
        private GUIStyle _miniButtonRight = new GUIStyle("MiniButtonRight");
        private GUIStyle _oLMinus = new GUIStyle("OL Minus");
        #endregion

        private void Init()
        {
            _assetRootFolder = new FolderInfo(Application.dataPath, "Assets", "");
            _validFilesCache = new Dictionary<string, FileInfo>();
            AssetBundleTool.ReadAssetsInChildren(_assetRootFolder, _validFilesCache);

            AssetBundleTool.ReadAssetsDependencies(_validFilesCache);

            _assetBundleInfo = new AssetBundleInfo();
            AssetBundleTool.ReadAssetBundleConfig(_assetBundleInfo, _validFilesCache);

            _buildPath = EditorPrefs.GetString("BuildPath", "");
            _buildTarget = (BuildTarget)EditorPrefs.GetInt("BuildTarget", 5);
            
            Resources.UnloadUnusedAssets();
        }
        private bool SearchEmptyFolder(FolderInfo folderInfo)
        {
            bool empty = true;
            if (folderInfo.ChildAssetInfo.Count > 0)
            {
                bool emptyCache;
                for (int i = 0; i < folderInfo.ChildAssetInfo.Count; i++)
                {
                    if (folderInfo.ChildAssetInfo[i] is FolderInfo)
                    {
                        FolderInfo folder = folderInfo.ChildAssetInfo[i] as FolderInfo;
                        emptyCache = SearchEmptyFolder(folder);
                        if (empty)
                        {
                            empty = emptyCache;
                        }
                    }
                    else if (folderInfo.ChildAssetInfo[i] is FileInfo)
                    {
                        FileInfo file = folderInfo.ChildAssetInfo[i] as FileInfo;
                        emptyCache = (_hideInvalidAsset && !file.IsValid) || (_hideBundleAsset && file.Bundled != null);
                        if (empty)
                        {
                            empty = emptyCache;
                        }
                    }
                }
            }
            folderInfo.IsEmpty = empty;
            return empty;
        }

        private void OnGUI()
        {
            TitleGUI();
            AssetBundlesGUI();
            CurrentAssetBundlesGUI();
            AssetsGUI();
            DependenciesGUI();
        }
        private void Update()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
                Resources.UnloadUnusedAssets();
            }
        }
        private void TitleGUI()
        {
            if (GUI.Button(new Rect(5, 5, 60, 15), "Create", _preButton))
            {
                string variant = (_variant == "" ? "" : ("." + _variant));
                BundleInfo build = new BundleInfo("ab" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + variant);
                _assetBundleInfo.AddBundle(build);
            }
            GUI.enabled = _currentAB != -1;
            if (GUI.Button(new Rect(65, 5, 60, 15), "Rename", _preButton))
            {
                _isRename = !_isRename;
            }
            if (GUI.Button(new Rect(125, 5, 60, 15), "Clear", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Clear " + _assetBundleInfo[_currentAB].Name + " ？", "Yes", "No"))
                {
                    _assetBundleInfo[_currentAB].ClearAsset();
                }
            }
            if (GUI.Button(new Rect(185, 5, 60, 15), "Delete", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Delete " + _assetBundleInfo[_currentAB].Name + "？This will clear all assets！", "Yes", "No"))
                {
                    _assetBundleInfo.DeleteBundle(_currentAB);
                    _currentAB = -1;
                }
            }
            if (GUI.Button(new Rect(250, 5, 100, 15), "Add Assets", _preButton))
            {
                List<FileInfo> assets = _validFilesCache.GetCheckedAssets();
                for (int i = 0; i < assets.Count; i++)
                {
                    _assetBundleInfo[_currentAB].AddAsset(assets[i]);
                    assets[i].IsChecked = false;
                }
            }
            GUI.enabled = true;
            if (GUI.Button(new Rect(355, 5, 100, 15), "Create ABs", _preButton))
            {
                List<FileInfo> assets = _validFilesCache.GetCheckedAssets();
                for (int i = 0; i < assets.Count; i++)
                {
                    string variant = (_variant == "" ? "" : ("." + _variant));
                    BundleInfo build = new BundleInfo(assets[i].Name.Replace(assets[i].Extension, "") + variant);
                    build.AddAsset(assets[i]);
                    assets[i].IsChecked = false;
                    _assetBundleInfo.AddBundle(build);
                }
            }
            if (GUI.Button(new Rect(460, 5, 100, 15), "Clear ABs", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Delete all ABs？Please operate with caution！", "Yes", "No"))
                {
                    _assetBundleInfo.ClearBundle();
                    _currentAB = -1;
                }
            }

            bool hideInvalidAsset = GUI.Toggle(new Rect(565, 5, 100, 15), _hideInvalidAsset, "Hide Invalid");
            bool hideBundleAsset = GUI.Toggle(new Rect(665, 5, 100, 15), _hideBundleAsset, "Hide Bundled");

            if(hideInvalidAsset != _hideInvalidAsset || hideBundleAsset != _hideBundleAsset)
            {
                _hideInvalidAsset = hideInvalidAsset;
                _hideBundleAsset = hideBundleAsset;
                SearchEmptyFolder(_assetRootFolder);
            }

            GUI.Label(new Rect(5, 25, 60, 15), "Variant:");
            _variant = EditorGUI.TextField(new Rect(70, 25, 110, 15), _variant);
            if (GUI.Button(new Rect(185, 25, 60, 15), "Apply", _preButton))
            {
                for (int i = 0; i < _assetBundleInfo.Count; i++)
                {
                    string variant = (_variant == "" ? "" : ("." + _variant));
                    string abName = _assetBundleInfo[i].Name.Split('.')[0];
                    _assetBundleInfo[i].RenameBundle(abName + variant);
                }
            }

            if (GUI.Button(new Rect(250, 25, 60, 15), "Open", _preButton))
            {
                AssetBundleTool.OpenFolder(_buildPath);
            }
            if (GUI.Button(new Rect(310, 25, 60, 15), "Browse", _preButton))
            {
                string path = EditorUtility.OpenFolderPanel("Select Path", Application.dataPath, "");
                if (path.Length != 0)
                {
                    _buildPath = path;
                    EditorPrefs.SetString("BuildPath", _buildPath);
                }
            }

            GUI.Label(new Rect(370, 25, 70, 15), "Build Path:");
            _buildPath = GUI.TextField(new Rect(440, 25, 300, 15), _buildPath);

            BuildTarget buildTarget = (BuildTarget)EditorGUI.EnumPopup(new Rect((int)position.width - 205, 5, 150, 15), _buildTarget, _preDropDown);
            if (buildTarget != _buildTarget)
            {
                _buildTarget = buildTarget;
                EditorPrefs.SetInt("BuildTarget", (int)_buildTarget);
            }

            if (GUI.Button(new Rect((int)position.width - 55, 5, 50, 15), "Build", _preButton))
            {
                AssetBundleTool.BuildAssetBundles();
            }
        }
        private void AssetBundlesGUI()
        {
            _ABViewRect = new Rect(5, 45, 240, (int)position.height / 2 - 40);
            _ABScrollRect = new Rect(5, 45, 240, _ABViewHeight);
            _ABScroll = GUI.BeginScrollView(_ABViewRect, _ABScroll, _ABScrollRect);
            GUI.BeginGroup(_ABScrollRect, _box);

            _ABViewHeight = 5;

            for (int i = 0; i < _assetBundleInfo.Count; i++)
            {
                string icon = _assetBundleInfo[i].Count > 0 ? "PrefabNormal Icon" : "Prefab Icon";
                if (_currentAB == i)
                {
                    GUI.Box(new Rect(0, _ABViewHeight, 240, 15), "", _LRSelect);

                    if (_isRename)
                    {
                        GUIContent content = EditorGUIUtility.IconContent(icon);
                        content.text = "";
                        GUI.Label(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel);
                        _renameValue = EditorGUI.TextField(new Rect(40, _ABViewHeight, 140, 15), _renameValue);
                        if (GUI.Button(new Rect(180, _ABViewHeight, 30, 15), "OK", _miniButtonLeft))
                        {
                            if (_renameValue != "")
                            {
                                if (!_assetBundleInfo.IsExistBundle(_renameValue))
                                {
                                    _assetBundleInfo[_currentAB].RenameBundle(_renameValue);
                                    _renameValue = "";
                                    _isRename = false;
                                }
                                else
                                {
                                    Debug.LogError("Already existed name:" + _renameValue);
                                }
                            }
                        }
                        if (GUI.Button(new Rect(210, _ABViewHeight, 30, 15), "NO", _miniButtonRight))
                        {
                            _isRename = false;
                        }
                    }
                    else
                    {
                        GUIContent content = EditorGUIUtility.IconContent(icon);
                        content.text = _assetBundleInfo[i].Name;
                        GUI.Label(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel);
                    }
                }
                else
                {
                    GUIContent content = EditorGUIUtility.IconContent(icon);
                    content.text = _assetBundleInfo[i].Name;
                    if (GUI.Button(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel))
                    {
                        _currentAB = i;
                        _currentABAsset = -1;
                        _isRename = false;
                    }
                }
                _ABViewHeight += 20;
            }

            _ABViewHeight += 5;
            if (_ABViewHeight < _ABViewRect.height)
            {
                _ABViewHeight = (int)_ABViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void CurrentAssetBundlesGUI()
        {
            _currentABViewRect = new Rect(5, (int)position.height / 2 + 10, 240, (int)position.height / 2 - 15);
            _currentABScrollRect = new Rect(5, (int)position.height / 2 + 10, 240, _currentABViewHeight);
            _currentABScroll = GUI.BeginScrollView(_currentABViewRect, _currentABScroll, _currentABScrollRect);
            GUI.BeginGroup(_currentABScrollRect, _box);

            _currentABViewHeight = 5;

            if (_currentAB != -1)
            {
                BundleInfo build = _assetBundleInfo[_currentAB];
                for (int i = 0; i < build.Count; i++)
                {
                    if (_currentABAsset == i)
                    {
                        GUI.Box(new Rect(0, _currentABViewHeight, 240, 15), "", _LRSelect);
                    }
                    GUIContent content = EditorGUIUtility.ObjectContent(null, build[i].AssetType);
                    content.text = build[i].Name;
                    if (GUI.Button(new Rect(5, _currentABViewHeight, 205, 15), content, _prefabLabel))
                    {
                        _currentABAsset = i;
                        Object obj = AssetDatabase.LoadAssetAtPath(build[_currentABAsset].Path, build[_currentABAsset].AssetType);
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                    if (GUI.Button(new Rect(215, _currentABViewHeight, 20, 15), "", _oLMinus))
                    {
                        build.RemoveAsset(build[i]);
                        _currentABAsset = -1;
                    }
                    _currentABViewHeight += 20;
                }
            }

            _currentABViewHeight += 5;
            if (_currentABViewHeight < _currentABViewRect.height)
            {
                _currentABViewHeight = (int)_currentABViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void AssetsGUI()
        {
            _assetViewRect = new Rect(250, 45, (int)position.width - 255, (int)position.height - 50);
            _assetScrollRect = new Rect(250, 45, (int)position.width - 255, _assetViewHeight);
            _assetScroll = GUI.BeginScrollView(_assetViewRect, _assetScroll, _assetScrollRect);
            GUI.BeginGroup(_assetScrollRect, _box);

            _assetViewHeight = 5;

            AssetGUI(_assetRootFolder, 0);

            _assetViewHeight += 20;
            if (_assetViewHeight < _assetViewRect.height)
            {
                _assetViewHeight = (int)_assetViewRect.height;
            }

            GUI.EndGroup();
            GUI.EndScrollView();
        }
        private void AssetGUI(AssetInfo asset, int indentation)
        {
            FileInfo fileInfo = asset as FileInfo;
            FolderInfo folderInfo = asset as FolderInfo;

            if (folderInfo != null)
            {
                if (folderInfo.IsEmpty)
                {
                    return;
                }
            }
            else
            {
                if ((_hideInvalidAsset && !fileInfo.IsValid) || (_hideBundleAsset && fileInfo.Bundled != null))
                {
                    return;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(indentation * 20 + 5);
            if (folderInfo != null)
            {
                if (GUILayout.Toggle(folderInfo.IsChecked, "", GUILayout.Width(20)) != folderInfo.IsChecked)
                {
                    AssetBundleTool.ChangeCheckedInChildren(folderInfo, !folderInfo.IsChecked);
                }

                GUIContent content = EditorGUIUtility.IconContent("Folder Icon");
                content.text = folderInfo.Name;
                folderInfo.IsExpanding = EditorGUILayout.Foldout(folderInfo.IsExpanding, content, true);
            }
            else
            {
                GUI.enabled = (fileInfo.IsValid && fileInfo.Bundled == null);
                if (GUILayout.Toggle(fileInfo.IsChecked, "", GUILayout.Width(20)) != fileInfo.IsChecked)
                {
                    AssetBundleTool.ChangeCheckedInChildren(fileInfo, !fileInfo.IsChecked);
                }

                GUILayout.Space(10);
                GUIContent content = EditorGUIUtility.ObjectContent(null, fileInfo.AssetType);
                content.text = fileInfo.Name;
                GUILayout.Label(content, GUILayout.Height(20));
                GUI.enabled = true;

                if (fileInfo.Bundled != null)
                {
                    GUILayout.Label("[" + fileInfo.Bundled + "]", _prefabLabel);
                }

                fileInfo.IsRedundantFile();
                if(fileInfo.IsRedundant)
                {
                    GUIContent redLight = EditorGUIUtility.IconContent("lightMeter/redLight");
                    redLight.text = "Redundant";
                    if (GUILayout.Button(redLight, _brokenPrefabLabel, GUILayout.Height(20)))
                    {
                        if (_currentFile != fileInfo)
                            _currentFile = fileInfo;
                        else
                            _currentFile = null;
                    }
                }
            }
            _assetViewHeight += 20;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (folderInfo != null && folderInfo.IsExpanding)
            {
                for (int i = 0; i < folderInfo.ChildAssetInfo.Count; i++)
                {
                    AssetGUI(folderInfo.ChildAssetInfo[i], indentation + 1);
                }
            }
        }
        private void DependenciesGUI()
        {
            if (_currentFile != null)
            {
                if (!_currentFile.IsRedundant)
                {
                    _currentFile = null;
                    return;
                }

                _dependenciesViewRect = new Rect((int)position.width - 220, 50, 200, 200);
                _dependenciesScrollRect = new Rect((int)position.width - 220, 50, 200, _dependenciesViewHeight);
                _dependenciesScroll = GUI.BeginScrollView(_dependenciesViewRect, _dependenciesScroll, _dependenciesScrollRect);
                GUI.BeginGroup(_dependenciesScrollRect, _box);

                _dependenciesViewHeight = 5;

                GUIContent content = EditorGUIUtility.ObjectContent(null, _currentFile.AssetType);
                content.text = _currentFile.Name;
                GUI.Label(new Rect(5, _dependenciesViewHeight, 35, 15), "Asset");
                if (GUI.Button(new Rect(45, _dependenciesViewHeight, 150, 15), content, _prefabLabel))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(_currentFile.Path, _currentFile.AssetType);
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                _dependenciesViewHeight += 20;

                if (_currentFile.Dependencies.Count > 0)
                {
                    GUI.Label(new Rect(5, _dependenciesViewHeight, 190, 15), "Dependencies");
                    _dependenciesViewHeight += 20;
                    for (int i = 0; i < _currentFile.Dependencies.Count; i++)
                    {
                        content = EditorGUIUtility.ObjectContent(null, _currentFile.Dependencies[i].AssetType);
                        content.text = _currentFile.Dependencies[i].Name;
                        if (GUI.Button(new Rect(45, _dependenciesViewHeight, 150, 15), content, _prefabLabel))
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath(_currentFile.Dependencies[i].Path, _currentFile.Dependencies[i].AssetType);
                            Selection.activeObject = obj;
                            EditorGUIUtility.PingObject(obj);
                        }
                        _dependenciesViewHeight += 20;
                    }
                }

                if (_currentFile.BeDependencies.Count > 0)
                {
                    GUI.Label(new Rect(5, _dependenciesViewHeight, 190, 15), "Be Dependencies");
                    _dependenciesViewHeight += 20;
                    for (int i = 0; i < _currentFile.BeDependencies.Count; i++)
                    {
                        content = EditorGUIUtility.ObjectContent(null, _currentFile.BeDependencies[i].AssetType);
                        content.text = _currentFile.BeDependencies[i].Name;
                        if (GUI.Button(new Rect(45, _dependenciesViewHeight, 150, 15), content, _prefabLabel))
                        {
                            Object obj = AssetDatabase.LoadAssetAtPath(_currentFile.BeDependencies[i].Path, _currentFile.BeDependencies[i].AssetType);
                            Selection.activeObject = obj;
                            EditorGUIUtility.PingObject(obj);
                        }
                        _dependenciesViewHeight += 20;
                    }
                }

                if (_currentFile.IndirectBundled.Count > 0)
                {
                    GUI.Label(new Rect(5, _dependenciesViewHeight, 190, 15), "Indirect Bundled");
                    _dependenciesViewHeight += 20;
                    foreach (KeyValuePair<BundleInfo, int> bundle in _currentFile.IndirectBundled)
                    {
                        GUI.Label(new Rect(45, _dependenciesViewHeight, 150, 15), bundle.Key.Name + " >> " + bundle.Value, _prefabLabel);
                        _dependenciesViewHeight += 20;
                    }
                }

                _dependenciesViewHeight += 5;
                if (_dependenciesViewHeight < _dependenciesViewRect.height)
                {
                    _dependenciesViewHeight = (int)_dependenciesViewRect.height;
                }

                GUI.EndGroup();
                GUI.EndScrollView();
            }
        }
    }
}
