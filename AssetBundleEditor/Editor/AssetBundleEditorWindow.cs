using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework.AssetBundleEditor
{
    public sealed class AssetBundleEditorWindow : EditorWindow
    {
        [MenuItem("HTFramework/AssetBundle/AssetBundle Editor")]
        private static void OpenAssetBundleWindow()
        {
            AssetBundleEditorWindow abEditor = GetWindow<AssetBundleEditorWindow>("AssetBundlesEditor");
            abEditor.position = new Rect(200, 200, 1200, 800);
            abEditor.Init();
            abEditor.Show();
        }

        #region Fields
        private AssetFolderInfo _assetRootFolder;
        private BundleInfo _currentAB = null;
        private AssetFileInfo _currentABFile = null;
        private AssetFileInfo _currentFile;

        private bool _isRename = false;
        private string _renameValue = "";

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

        private Rect _assetPropertyViewRect;
        private Rect _assetPropertyScrollRect;
        private Vector2 _assetPropertyScroll;
        private int _assetPropertyViewHeight = 0;
        private bool _isShowDependencies = false;
        private bool _isShowBeDependencies = false;
        private bool _isShowIndirectBundled = false;

        private bool _hideInvalidAsset = false;
        private bool _hideBundleAsset = false;
        private bool _showOnlyRedundant = false;

        private string _buildPath = "";
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows;
        private string _variant = "";

        private GUIStyle _box;
        private GUIStyle _helpBox;
        private GUIStyle _preButton;
        private GUIStyle _preDropDown;
        private GUIStyle _LRSelect;
        private GUIStyle _prefabLabel;
        private GUIStyle _label;
        private GUIStyle _brokenPrefabLabel;
        private GUIStyle _miniButtonLeft;
        private GUIStyle _miniButtonRight;
        private GUIStyle _oLMinus;
        private GUIContent _redundant;
        #endregion
        
        private void Init()
        {
            _assetRootFolder = AssetBundleEditorUtility.GetFolderInfoByFullPath(Application.dataPath);
            
            AssetBundleEditorUtility.ReadAssetBundleConfig();

            _buildPath = EditorPrefs.GetString(Application.productName + ".AssetBundleEditor.BuildPath", Application.streamingAssetsPath);
            _buildTarget = (BuildTarget)EditorPrefs.GetInt(Application.productName + ".AssetBundleEditor.BuildTarget", 5);
            _variant = EditorPrefs.GetString(Application.productName + ".AssetBundleEditor.Variant", "");

            _box = new GUIStyle("Box");
            _helpBox = new GUIStyle("HelpBox");
            _preButton = new GUIStyle("PreButton");
            _preDropDown = new GUIStyle("PreDropDown");
            _LRSelect = new GUIStyle("LODSliderRangeSelected");
            _prefabLabel = new GUIStyle("PR PrefabLabel");
            _label = new GUIStyle("PR Label");
            _brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
            _miniButtonLeft = new GUIStyle("MiniButtonLeft");
            _miniButtonRight = new GUIStyle("MiniButtonRight");
            _oLMinus = new GUIStyle("OL Minus");
            _redundant = EditorGUIUtility.IconContent("lightMeter/redLight");
            _redundant.text = "Redundant";
        }
        private void Update()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
            }
        }
        private void OnDestroy()
        {
            AssetBundleEditorUtility.ClearData();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        private void OnGUI()
        {
            TitleGUI();
            AssetBundlesGUI();
            CurrentAssetBundlesGUI();
            AssetFolderGUI();
            AssetPropertyGUI();
        }
        private void TitleGUI()
        {
            if (GUI.Button(new Rect(5, 5, 60, 15), "Create", _preButton))
            {
                AssetBundleEditorUtility.GetBundleInfoByName("ab" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            }

            GUI.enabled = (_currentAB != null);

            if (GUI.Button(new Rect(65, 5, 60, 15), "Rename", _preButton))
            {
                _isRename = !_isRename;
            }
            if (GUI.Button(new Rect(125, 5, 60, 15), "Clear", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Clear " + _currentAB + " ？", "Yes", "No"))
                {
                    _currentAB.ClearAsset();
                }
            }
            if (GUI.Button(new Rect(185, 5, 60, 15), "Delete", _preButton))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Delete " + _currentAB + "？this will clear all assets！and this operation cannot be restore!", "Yes", "No"))
                {
                    AssetBundleEditorUtility.DeleteBundleInfoByName(_currentAB.Name);
                    _currentAB = null;
                }
            }

            GUI.enabled = true;

            _hideInvalidAsset = GUI.Toggle(new Rect(265, 5, 100, 15), _hideInvalidAsset, "Hide Invalid");
            _hideBundleAsset = GUI.Toggle(new Rect(365, 5, 100, 15), _hideBundleAsset, "Hide Bundled");
            _showOnlyRedundant = GUI.Toggle(new Rect(465, 5, 200, 15), _showOnlyRedundant, "Show Only Redundant");

            GUI.Label(new Rect(5, 25, 60, 15), "Variant:");
            _variant = EditorGUI.TextField(new Rect(70, 25, 110, 15), _variant);
            if (GUI.Button(new Rect(185, 25, 60, 15), "Apply", _preButton))
            {
                EditorPrefs.SetString(Application.productName + ".AssetBundleEditor.Variant", _variant);
            }

            if (GUI.Button(new Rect(250, 25, 60, 15), "Open", _preButton))
            {
                AssetBundleEditorUtility.OpenFolder(_buildPath);
            }
            if (GUI.Button(new Rect(310, 25, 60, 15), "Browse", _preButton))
            {
                string path = EditorUtility.OpenFolderPanel("Select Path", Application.dataPath, "");
                if (path.Length != 0)
                {
                    _buildPath = path;
                    EditorPrefs.SetString(Application.productName + ".AssetBundleEditor.BuildPath", _buildPath);
                }
            }

            GUI.Label(new Rect(370, 25, 70, 15), "Build Path:");
            GUI.TextField(new Rect(440, 25, 300, 15), _buildPath);

            BuildTarget buildTarget = (BuildTarget)EditorGUI.EnumPopup(new Rect((int)position.width - 205, 5, 150, 15), _buildTarget, _preDropDown);
            if (buildTarget != _buildTarget)
            {
                _buildTarget = buildTarget;
                EditorPrefs.SetInt(Application.productName + ".AssetBundleEditor.BuildTarget", (int)_buildTarget);
            }

            if (GUI.Button(new Rect((int)position.width - 55, 5, 50, 15), "Build", _preButton))
            {
                AssetBundleEditorUtility.BuildAssetBundles();
            }
        }
        private void AssetBundlesGUI()
        {
            _ABViewRect = new Rect(5, 45, 240, (int)position.height / 2 - 40);
            _ABScrollRect = new Rect(5, 45, 240, _ABViewHeight);
            _ABScroll = GUI.BeginScrollView(_ABViewRect, _ABScroll, _ABScrollRect);
            GUI.BeginGroup(_ABScrollRect, _box);

            _ABViewHeight = 5;

            for (int i = 0; i < AssetBundleEditorUtility.BundleInfosList.Count; i++)
            {
                BundleInfo bundle = AssetBundleEditorUtility.BundleInfosList[i];
                string icon = bundle.Count > 0 ? "Prefab Icon" : "GameObject Icon";
                if (_currentAB == bundle)
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
                                if (!AssetBundleEditorUtility.IsExistBundleInfo(_renameValue))
                                {
                                    AssetBundleEditorUtility.RenameBundleInfo(_currentAB.Name, _renameValue);
                                    _renameValue = "";
                                    _isRename = false;
                                }
                                else
                                {
                                    GlobalTools.LogError("Already existed AssetBundle name:" + _renameValue);
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
                        content.text = bundle.Name;
                        GUI.Label(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel);
                    }
                }
                else
                {
                    GUIContent content = EditorGUIUtility.IconContent(icon);
                    content.text = bundle.Name;
                    if (GUI.Button(new Rect(5, _ABViewHeight, 230, 15), content, _prefabLabel))
                    {
                        _currentAB = bundle;
                        _currentABFile = null;
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

            if (_currentAB != null)
            {
                for (int i = 0; i < _currentAB.Count; i++)
                {
                    AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(_currentAB[i]);
                    if (_currentABFile == file)
                    {
                        GUI.Box(new Rect(0, _currentABViewHeight, 240, 15), "", _LRSelect);
                    }
                    GUIContent content = EditorGUIUtility.ObjectContent(null, file.AssetType);
                    content.text = file.Name;
                    if (GUI.Button(new Rect(5, _currentABViewHeight, 205, 15), content, _prefabLabel))
                    {
                        _currentABFile = file;
                        Object obj = AssetDatabase.LoadAssetAtPath(_currentABFile.AssetPath, _currentABFile.AssetType);
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                    if (GUI.Button(new Rect(215, _currentABViewHeight, 20, 15), "", _oLMinus))
                    {
                        _currentAB.RemoveAsset(_currentAB[i]);
                        _currentABFile = null;
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
        private void AssetFolderGUI()
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
        private void AssetGUI(AssetInfoBase asset, int indentation)
        {
            AssetFileInfo fileInfo = asset as AssetFileInfo;
            AssetFolderInfo folderInfo = asset as AssetFolderInfo;

            if (fileInfo != null)
            {
                AssetBundleEditorUtility.IsRedundantFile(fileInfo);
                if (_showOnlyRedundant)
                {
                    if (!fileInfo.IsRedundant)
                    {
                        return;
                    }
                }
                else
                {
                    if ((_hideInvalidAsset && !fileInfo.IsValid) || (_hideBundleAsset && fileInfo.Bundled != ""))
                    {
                        return;
                    }
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(indentation * 20 + 5);
            if (folderInfo != null)
            {
                GUIContent content = EditorGUIUtility.IconContent("Folder Icon");
                content.text = folderInfo.Name;
                folderInfo.IsExpanding = EditorGUILayout.Foldout(folderInfo.IsExpanding, content, true);
            }
            else
            {
                GUI.enabled = fileInfo.IsValid;
                GUI.color = (fileInfo.IsRedundant ? Color.red : Color.white);

                GUILayout.Space(10);
                GUIContent content = EditorGUIUtility.ObjectContent(null, fileInfo.AssetType);
                content.text = fileInfo.Name;
                if (GUILayout.Button(content, _currentFile == fileInfo ? _prefabLabel : _label, GUILayout.Height(20)))
                {
                    _currentFile = fileInfo;
                }
                
                if (fileInfo.Bundled != "")
                {
                    GUILayout.Label("[" + fileInfo.Bundled + "]", _prefabLabel);
                }

                if (fileInfo.IsRedundant)
                {
                    GUILayout.Label(_redundant, _brokenPrefabLabel, GUILayout.Height(20));
                }

                GUI.color = Color.white;
                GUI.enabled = true;
            }
            _assetViewHeight += 20;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (folderInfo != null && folderInfo.IsExpanding)
            {
                folderInfo.ReadChildAsset();
                for (int i = 0; i < folderInfo.ChildAsset.Count; i++)
                {
                    AssetGUI(folderInfo.ChildAsset[i], indentation + 1);
                }
            }
        }
        private void AssetPropertyGUI()
        {
            if (_currentFile != null)
            {
                _currentFile.ReadDependenciesFile();

                GUI.color = (_currentFile.IsRedundant ? Color.red : Color.white);

                _assetPropertyViewRect = new Rect((int)position.width - 420, 50, 400, 400);
                _assetPropertyScrollRect = new Rect((int)position.width - 420, 50, 400, _assetPropertyViewHeight);
                _assetPropertyScroll = GUI.BeginScrollView(_assetPropertyViewRect, _assetPropertyScroll, _assetPropertyScrollRect);
                GUI.BeginGroup(_assetPropertyScrollRect, _helpBox);

                _assetPropertyViewHeight = 5;

                GUIContent content = EditorGUIUtility.ObjectContent(null, _currentFile.AssetType);
                content.text = _currentFile.Name;
                GUI.Label(new Rect(5, _assetPropertyViewHeight, 40, 15), "Asset:");
                if (GUI.Button(new Rect(50, _assetPropertyViewHeight, 280, 15), content, _prefabLabel))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(_currentFile.AssetPath, _currentFile.AssetType);
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                GUI.enabled = (_currentAB != null && _currentFile.Bundled == "");
                if (GUI.Button(new Rect(340, _assetPropertyViewHeight, 50, 15), "Bundle", _preButton))
                {
                    _currentAB.AddAsset(_currentFile.AssetPath);
                    _currentFile = null;
                    return;
                }
                GUI.enabled = true;
                _assetPropertyViewHeight += 20;

                GUI.Label(new Rect(5, _assetPropertyViewHeight, 330, 15), "Path: " + _currentFile.AssetPath);
                if (GUI.Button(new Rect(340, _assetPropertyViewHeight, 50, 15), "Copy", _preButton))
                {
                    GUIUtility.systemCopyBuffer = _currentFile.AssetPath;
                }
                _assetPropertyViewHeight += 20;

                if (_currentFile.Dependencies.Count > 0)
                {
                    _isShowDependencies = EditorGUI.Foldout(new Rect(5, _assetPropertyViewHeight, 390, 15), _isShowDependencies, "Dependencies:", true);
                    _assetPropertyViewHeight += 20;
                    if (_isShowDependencies)
                    {
                        for (int i = 0; i < _currentFile.Dependencies.Count; i++)
                        {
                            AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(_currentFile.Dependencies[i]);
                            content = EditorGUIUtility.ObjectContent(null, file.AssetType);
                            content.text = file.Name;
                            if (GUI.Button(new Rect(45, _assetPropertyViewHeight, 350, 15), content, _prefabLabel))
                            {
                                Object obj = AssetDatabase.LoadAssetAtPath(file.AssetPath, file.AssetType);
                                Selection.activeObject = obj;
                                EditorGUIUtility.PingObject(obj);
                            }
                            _assetPropertyViewHeight += 20;
                        }
                    }
                }

                if (_currentFile.IndirectBundledRelation.Count > 0)
                {
                    _isShowIndirectBundled = EditorGUI.Foldout(new Rect(5, _assetPropertyViewHeight, 390, 15), _isShowIndirectBundled, "Indirect Bundled:", true);
                    _assetPropertyViewHeight += 20;
                    if (_isShowIndirectBundled)
                    {
                        foreach (KeyValuePair<string, string> bundle in _currentFile.IndirectBundledRelation)
                        {
                            AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(bundle.Key);
                            content = EditorGUIUtility.ObjectContent(null, file.AssetType);
                            content.text = file.Name + "  >>  " + bundle.Value;
                            if (GUI.Button(new Rect(45, _assetPropertyViewHeight, 350, 15), content, _prefabLabel))
                            {
                                Object obj = AssetDatabase.LoadAssetAtPath(file.AssetPath, file.AssetType);
                                Selection.activeObject = obj;
                                EditorGUIUtility.PingObject(obj);
                            }
                            _assetPropertyViewHeight += 20;
                        }
                    }
                }

                _assetPropertyViewHeight += 5;
                if (_assetPropertyViewHeight < _assetPropertyViewRect.height)
                {
                    _assetPropertyViewHeight = (int)_assetPropertyViewRect.height;
                }

                GUI.EndGroup();
                GUI.EndScrollView();

                GUI.color = Color.white;
            }
        }
    }
}
