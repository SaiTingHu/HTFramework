using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class StepEditorWindow : EditorWindow
    {
        public static void ShowWindow(StepContentAsset contentAsset)
        {
            StepEditorWindow window = GetWindow<StepEditorWindow>();
            window.titleContent.text = "Step Editor";
            window._contentAsset = contentAsset;
            window._currentStep = -1;
            window._currentOperation = -1;
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            window._isMinimize = false;
            window.Show();
        }

        private StepContentAsset _contentAsset;
        private int _currentStep = -1;
        private int _currentOperation = -1;
        private StepContent _currentStepObj;
        private StepOperation _currentOperationObj;
        private Vector2 _mouseDownPos;
        private Texture _background;
        private bool _isMinimize = false;
        private Rect _recordedPosition;

        private StepListShowType _stepListShowType = StepListShowType.Name;
        private bool _showAncillary = true;
        private Rect _stepListRect;
        private Vector2 _stepListScroll = Vector3.zero;
        private Vector2 _stepContentScroll = Vector3.zero;
        private Vector2 _stepOperationScroll = Vector3.zero;
        private string _stepListFilter = "";
        private float _stepListGUIWidth = 200;

        private Rect _stepContentRect;
        private float _operationButtonWidth = 150;
        private float _operationButtonHeight = 40;
        private bool _stepContentDragging = false;
        private bool _stepContentGUIRepaint = false;

        private Rect _splitterRect;
        private float _splitterWidth = 5;
        private bool _splitterDragging = false;
        private bool _isMoveTo = false;
        private int _moveToIndex = 1;
        
        private CameraTarget _ct;
        private MousePosition _mp;
        private MouseRotation _mr;
        private Transform _player;

        private Type _baseType = typeof(StepHelper);
        private Dictionary<string, string> _helpers = new Dictionary<string, string>();

        private void OnEnable()
        {
            SelectStepContent(_currentStep);
            SelectStepOperation(_currentOperation);

            _background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/StepEditor/Texture/background.png");

            _ct = FindObjectOfType<CameraTarget>();
            _mp = FindObjectOfType<MousePosition>();
            _mr = FindObjectOfType<MouseRotation>();
            _player = null;

            _helpers.Clear();
            string[] helpers = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < helpers.Length; i++)
            {
                if (helpers[i].EndsWith(".cs"))
                {
                    string className = helpers[i].Substring(helpers[i].LastIndexOf("/") + 1).Replace(".cs", "");
                    if (!_helpers.ContainsKey(className))
                    {
                        _helpers.Add(className, helpers[i]);
                    }
                }
            }
        }

        private void Update()
        {
            if (_contentAsset == null)
            {
                Close();
            }
        }

        private void OnGUI()
        {
            if (_contentAsset == null)
            {
                Close();
                return;
            }

            if (_isMinimize)
            {
                MinimizeGUI();
            }
            else
            {
                GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), _background, new Rect(0, 0, position.width / 50, position.height / 50));

                StepContentRemovableGUI();

                GUILayout.BeginHorizontal("Toolbar");
                TitleGUI();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                StepListGUI();
                SplitterGUI();
                StepContentFixedGUI();
                GUILayout.EndHorizontal();

                MouseControl();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_contentAsset);
            }
        }

        /// <summary>
        /// 最小化后的GUI
        /// </summary>
        private void MinimizeGUI()
        {
            GUILayout.BeginHorizontal("Toolbar");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Maximize", "Toolbarbutton"))
            {
                MaximizeWindow();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Step Count:" + _contentAsset.Content.Count);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 标题GUI
        /// </summary>
        private void TitleGUI()
        {
            if (GUILayout.Button(_contentAsset.name, "Toolbarbutton"))
            {
                Selection.activeObject = _contentAsset;
                EditorGUIUtility.PingObject(_contentAsset);
            }
            if (GUILayout.Button("Clear Unused GUID", "Toolbarbutton"))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure clear unused GUID [StepTarget] in the current opened scene？", "Yes", "No"))
                {
                    HashSet<string> usedTargets = new HashSet<string>();
                    for (int i = 0; i < _contentAsset.Content.Count; i++)
                    {
                        StepContent content = _contentAsset.Content[i];
                        if (!usedTargets.Contains(content.TargetGUID) && content.TargetGUID != "<None>")
                        {
                            usedTargets.Add(content.TargetGUID);
                        }

                        for (int j = 0; j < content.Operations.Count; j++)
                        {
                            StepOperation operation = content.Operations[j];
                            if (!usedTargets.Contains(operation.TargetGUID) && operation.TargetGUID != "<None>")
                            {
                                usedTargets.Add(operation.TargetGUID);
                            }
                        }
                    }

                    GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                    foreach (GameObject rootObj in rootObjs)
                    {
                        StepTarget[] targets = rootObj.transform.GetComponentsInChildren<StepTarget>(true);
                        foreach (StepTarget target in targets)
                        {
                            if (!usedTargets.Contains(target.GUID))
                            {
                                DestroyImmediate(target);
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Regen Step ID", "Toolbarbutton"))
            {
                StepRegenIDWindow.ShowWindow(this, _contentAsset);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Browse Ease Type", "Toolbarbutton"))
            {
                Application.OpenURL(@"http://robertpenner.com/easing/easing_demo.html");
            }
            if (GUILayout.Button("Minimize", "Toolbarbutton"))
            {
                MinimizeWindow();
            }
        }
        /// <summary>
        /// 步骤列表GUI
        /// </summary>
        private void StepListGUI()
        {
            GUILayout.BeginVertical("PreBackground", GUILayout.Width(_stepListGUIWidth));

            #region 筛选步骤
            GUILayout.BeginHorizontal("Icon.OutlineBorder");
            GUILayout.Label("Step Content List", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Show Type:");
            _stepListShowType = (StepListShowType)EditorGUILayout.EnumPopup(_stepListShowType, GUILayout.Width(100));
            if (_stepListShowType == StepListShowType.Name)
            {
                _showAncillary = GUILayout.Toggle(_showAncillary, "Ancillary");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _stepListFilter = EditorGUILayout.TextField("", _stepListFilter, "SearchTextField");
            if (GUILayout.Button("", _stepListFilter != "" ? "SearchCancelButton" : "SearchCancelButtonEmpty"))
            {
                _stepListFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
            #endregion

            #region 步骤列表
            _stepListScroll = GUILayout.BeginScrollView(_stepListScroll);
            for (int i = 0; i < _contentAsset.Content.Count; i++)
            {
                if (StepFilter(_contentAsset.Content[i]))
                {
                    if (_showAncillary && _contentAsset.Content[i].Ancillary != "")
                    {
                        GUILayout.BeginHorizontal();
                        GUI.color = Color.yellow;
                        GUILayout.Label("【" + _contentAsset.Content[i].Ancillary + "】", GUILayout.Height(16));
                        GUI.color = Color.white;
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    GUI.color = (_currentStep == i ? Color.cyan : Color.white);
                    GUIContent content = EditorGUIUtility.IconContent("Avatar Icon");
                    content.text = i + "." + StepShowName(_contentAsset.Content[i]);
                    content.tooltip = _contentAsset.Content[i].Prompt;
                    GUILayout.Label(content, GUILayout.Height(16));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-->", "Minibutton"))
                    {
                        SelectStepContent(i);
                        SelectStepOperation(-1);
                        GUI.FocusControl(null);

                        if (_currentStepObj.Target && Selection.activeGameObject != _currentStepObj.Target)
                        {
                            Selection.activeGameObject = _currentStepObj.Target;
                            EditorGUIUtility.PingObject(_currentStepObj.Target);
                        }
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            #endregion

            GUILayout.FlexibleSpace();

            #region 移动到
            if (_isMoveTo)
            {
                GUILayout.BeginHorizontal();
                GUI.enabled = (_currentStep != -1);
                GUILayout.Label("Move To: ");
                _moveToIndex = EditorGUILayout.IntField(_moveToIndex);
                if (GUILayout.Button("Sure", "MiniButtonLeft"))
                {
                    if (_moveToIndex >= 0 && _moveToIndex <= _contentAsset.Content.Count - 1)
                    {
                        _contentAsset.Content.RemoveAt(_currentStep);
                        _currentStep = _moveToIndex;
                        _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                    }
                    else
                    {
                        GlobalTools.LogError("当前要移动到的索引不存在！");
                    }
                    _isMoveTo = false;
                }
                if (GUILayout.Button("Cancel", "MiniButtonRight"))
                {
                    _isMoveTo = false;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            #endregion

            #region 添加、移动、克隆、删除步骤
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", "ButtonLeft"))
            {
                StepContent content = new StepContent();
                content.GUID = Guid.NewGuid().ToString();
                content.EnterAnchor = new Vector2(position.width / 2, position.height / 2);
                _contentAsset.Content.Add(content);
                SelectStepContent(_contentAsset.Content.Count - 1);
                SelectStepOperation(-1);
            }
            GUI.enabled = (_currentStep != -1);
            if (GUILayout.Button("Move Up", "ButtonMid"))
            {
                if (_currentStep > 0)
                {
                    _contentAsset.Content.RemoveAt(_currentStep);
                    _currentStep -= 1;
                    _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                }
            }
            if (GUILayout.Button("Move Down", "ButtonMid"))
            {
                if (_currentStep < _contentAsset.Content.Count - 1)
                {
                    _contentAsset.Content.RemoveAt(_currentStep);
                    _currentStep += 1;
                    _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                }
            }
            if (GUILayout.Button("Move To", "ButtonMid"))
            {
                _isMoveTo = !_isMoveTo;
            }
            if (GUILayout.Button("Clone", "ButtonMid"))
            {
                _contentAsset.Content.Add(_currentStepObj.Clone());
                SelectStepContent(_contentAsset.Content.Count - 1);
                SelectStepOperation(-1);
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete", "ButtonRight"))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure delete step " + _contentAsset.Content[_currentStep].Name + "？", "Yes", "No"))
                {
                    _contentAsset.Content.RemoveAt(_currentStep);
                    SelectStepContent(-1);
                    SelectStepOperation(-1);
                }
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();
            _stepListRect = GUILayoutUtility.GetLastRect();
        }
        /// <summary>
        /// 分割线GUI
        /// </summary>
        private void SplitterGUI()
        {
            GUILayout.Box("", "PreVerticalScrollbarThumb", GUILayout.Width(_splitterWidth), GUILayout.MaxWidth(_splitterWidth), GUILayout.MinWidth(_splitterWidth), GUILayout.ExpandHeight(true));
            _splitterRect = GUILayoutUtility.GetLastRect();
        }
        /// <summary>
        /// 步骤内容GUI（固定内容）
        /// </summary>
        private void StepContentFixedGUI()
        {
            GUILayout.BeginVertical();

            if (_currentStep == -1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Please select a Step Content!", "BoldLabel");
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                #region 步骤的属性
                GUILayout.BeginVertical("HelpBox", GUILayout.Width(205), GUILayout.Height(420));

                GUILayout.BeginHorizontal("Icon.OutlineBorder");
                GUILayout.Label("Step Content Properties", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                _stepContentScroll = GUILayout.BeginScrollView(_stepContentScroll);

                GUILayout.BeginVertical("Tooltip");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.Width(50));
                _currentStepObj.Name = EditorGUILayout.TextField(_currentStepObj.Name, GUILayout.Width(130));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("GUID:", GUILayout.Width(50));
                _currentStepObj.GUID = EditorGUILayout.TextField(_currentStepObj.GUID, GUILayout.Width(130));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUI.enabled = !_currentStepObj.Instant;
                GUILayout.Label("Elapse Time:", GUILayout.Width(80));
                _currentStepObj.ElapseTime = EditorGUILayout.FloatField(_currentStepObj.ElapseTime, GUILayout.Width(40));
                GUI.enabled = true;
                _currentStepObj.Instant = GUILayout.Toggle(_currentStepObj.Instant, "Instant", GUILayout.Width(60));
                GUILayout.EndHorizontal();

                #region 步骤目标物体丢失，根据目标GUID重新搜寻
                if (!_currentStepObj.Target)
                {
                    if (_currentStepObj.TargetGUID != "<None>")
                    {
                        _currentStepObj.Target = GameObject.Find(_currentStepObj.TargetPath);
                        if (!_currentStepObj.Target)
                        {
                            StepTarget[] targets = FindObjectsOfType<StepTarget>();
                            foreach (StepTarget target in targets)
                            {
                                if (target.GUID == _currentStepObj.TargetGUID)
                                {
                                    _currentStepObj.Target = target.gameObject;
                                    _currentStepObj.TargetPath = target.transform.FullName();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            StepTarget target = _currentStepObj.Target.GetComponent<StepTarget>();
                            if (!target)
                            {
                                target = _currentStepObj.Target.AddComponent<StepTarget>();
                                target.GUID = _currentStepObj.TargetGUID;
                            }
                        }
                    }
                }
                #endregion

                GUILayout.BeginHorizontal();
                GUILayout.Label("Target:", GUILayout.Width(50));
                GUI.color = _currentStepObj.Target ? Color.white : Color.gray;
                GameObject contentObj = EditorGUILayout.ObjectField(_currentStepObj.Target, typeof(GameObject), true, GUILayout.Width(130)) as GameObject;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("GUID: " + _currentStepObj.TargetGUID, GUILayout.Width(140));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear", "Minibutton", GUILayout.Width(40)))
                {
                    contentObj = _currentStepObj.Target = null;
                    _currentStepObj.TargetGUID = "<None>";
                    _currentStepObj.TargetPath = "<None>";
                    GUI.FocusControl(null);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();

                GUILayout.Space(5);
                
                #region 步骤目标改变
                if (contentObj != _currentStepObj.Target)
                {
                    if (contentObj)
                    {
                        StepTarget target = contentObj.GetComponent<StepTarget>();
                        if (!target)
                        {
                            target = contentObj.AddComponent<StepTarget>();
                        }
                        if (target.GUID == "<None>")
                        {
                            target.GUID = Guid.NewGuid().ToString();
                        }
                        _currentStepObj.Target = contentObj;
                        _currentStepObj.TargetGUID = target.GUID;
                        _currentStepObj.TargetPath = contentObj.transform.FullName();
                    }
                }
                #endregion

                GUILayout.BeginVertical("Tooltip");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Prompt:", GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _currentStepObj.Prompt = EditorGUILayout.TextArea(_currentStepObj.Prompt, GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Ancillary:", GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _currentStepObj.Ancillary = EditorGUILayout.TextArea(_currentStepObj.Ancillary, GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Operation: " + _currentStepObj.Operations.Count, GUILayout.Width(130));
                if (GUILayout.Button("Find", "MiniPopup", GUILayout.Width(50)))
                {
                    GUI.FocusControl(null);
                    GenericMenu gm = new GenericMenu();
                    for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(_currentStepObj.Operations[j].Name), _currentOperation == j, () =>
                        {
                            SelectStepOperation(j);

                            Vector2 direction = (new Vector2(position.width / 2, position.height / 2) - _currentStepObj.Operations[j].Anchor);
                            for (int m = 0; m < _currentStepObj.Operations.Count; m++)
                            {
                                _currentStepObj.Operations[m].Anchor += direction;
                            }
                            _currentStepObj.EnterAnchor += direction;
                        });
                    }
                    gm.ShowAsContext();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Trigger:", GUILayout.Width(50));
                _currentStepObj.Trigger = (StepTrigger)EditorGUILayout.EnumPopup(_currentStepObj.Trigger, GUILayout.Width(130));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Initial Mode:", GUILayout.Width(80));
                _currentStepObj.InitialMode = (ControlMode)EditorGUILayout.EnumPopup(_currentStepObj.InitialMode, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Best View:", GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                GUI.enabled = _mr;
                if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(40)))
                {
                    _currentStepObj.BestView = new Vector3(_mr.X, _mr.Y, _mr.Distance);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _currentStepObj.BestView = EditorGUILayout.Vector3Field("", _currentStepObj.BestView, GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("View Offset:", GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                GUI.enabled = _ct && _currentStepObj.Target;
                if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(40)))
                {
                    _currentStepObj.ViewOffset = _ct.transform.position - _currentStepObj.Target.transform.position;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                _currentStepObj.ViewOffset = EditorGUILayout.Vector3Field("", _currentStepObj.ViewOffset, GUILayout.Width(180));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Best Pos:", GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                GUI.enabled = _player && _currentStepObj.Target;
                if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(40)))
                {
                    _currentStepObj.BestPos = _player.transform.position;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _currentStepObj.BestPos = EditorGUILayout.Vector3Field("", _currentStepObj.BestPos, GUILayout.Width(180));
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Helper:", GUILayout.Width(50));
                if (GUILayout.Button(_currentStepObj.Helper, "MiniPopup", GUILayout.Width(100)))
                {
                    List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                    GenericMenu gm = new GenericMenu();
                    gm.AddItem(new GUIContent("<Create>"), false, () =>
                    {
                        string directory = EditorPrefs.GetString(EditorPrefsTable.Script_Helper_Directory, Application.dataPath);
                        string path = EditorUtility.SaveFilePanel("新建 Helper 类", directory, "NewHelper", "cs");
                        if (path != "")
                        {
                            string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                            if (!_helpers.ContainsKey(className))
                            {
                                TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/HelperTemplate.txt", typeof(TextAsset)) as TextAsset;
                                if (asset)
                                {
                                    string code = asset.text;
                                    code = code.Replace("#SCRIPTNAME#", className);
                                    code = code.Replace("#HELPERNAME#", className);
                                    File.AppendAllText(path, code);
                                    _currentStepObj.Helper = className;
                                    asset = null;
                                    AssetDatabase.Refresh();
                                    EditorPrefs.SetString(EditorPrefsTable.Script_Helper_Directory, path.Substring(0, path.LastIndexOf("/")));
                                }
                            }
                            else
                            {
                                GlobalTools.LogError("新建Helper失败，已存在类型 " + className);
                            }
                        }
                    });
                    gm.AddItem(new GUIContent("<None>"), _currentStepObj.Helper == "<None>", () =>
                    {
                        _currentStepObj.Helper = "<None>";
                    });
                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(_baseType))
                        {
                            CustomHelperAttribute helper = type.GetCustomAttribute<CustomHelperAttribute>();
                            if (helper != null)
                            {
                                gm.AddItem(new GUIContent(helper.Name), _currentStepObj.Helper == type.FullName, () =>
                                {
                                    _currentStepObj.Helper = type.FullName;
                                });
                            }
                        }
                    }
                    gm.ShowAsContext();
                }
                GUI.enabled = _currentStepObj.Helper != "<None>";
                if (GUILayout.Button("Edit", "MiniButton", GUILayout.Width(30)))
                {
                    if (_helpers.ContainsKey(_currentStepObj.Helper))
                    {
                        UnityEngine.Object classFile = AssetDatabase.LoadAssetAtPath(_helpers[_currentStepObj.Helper], typeof(TextAsset));
                        if (classFile)
                            AssetDatabase.OpenAsset(classFile);
                        else
                            GlobalTools.LogError("没有找到 " + _currentStepObj.Helper + " 脚本文件！");
                    }
                    else
                    {
                        GlobalTools.LogError("没有找到 " + _currentStepObj.Helper + " 脚本文件！");
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Parameter:", GUILayout.Width(80));
                if (GUILayout.Button("Edit Parameter", "MiniButton"))
                {
                    StepParameterWindow.ShowWindow(this, _contentAsset, _currentStepObj);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.EndScrollView();

                GUILayout.EndVertical();
                #endregion

                #region 摄像机组件
                GUILayout.BeginVertical("HelpBox", GUILayout.Width(205), GUILayout.Height(120));

                GUILayout.BeginHorizontal("Icon.OutlineBorder");
                GUILayout.Label("Camera Control", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginVertical("Tooltip");

                GUILayout.BeginHorizontal();
                GUILayout.Label("CT:", GUILayout.Width(30));
                GUI.color = _ct ? Color.white : Color.gray;
                _ct = EditorGUILayout.ObjectField(_ct, typeof(CameraTarget), true, GUILayout.Width(150)) as CameraTarget;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("MR:", GUILayout.Width(30));
                GUI.color = _mr ? Color.white : Color.gray;
                _mr = EditorGUILayout.ObjectField(_mr, typeof(MouseRotation), true, GUILayout.Width(150)) as MouseRotation;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("MP:", GUILayout.Width(30));
                GUI.color = _mp ? Color.white : Color.gray;
                _mp = EditorGUILayout.ObjectField(_mp, typeof(MousePosition), true, GUILayout.Width(150)) as MousePosition;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("P:", GUILayout.Width(30));
                GUI.color = _player ? Color.white : Color.gray;
                _player = EditorGUILayout.ObjectField(_player, typeof(Transform), true, GUILayout.Width(150)) as Transform;
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.EndVertical();
                #endregion

                GUILayout.FlexibleSpace();

                #region 步骤操作的属性
                if (_currentOperation != -1)
                {
                    if (_stepContentGUIRepaint)
                    {
                        _stepContentGUIRepaint = false;
                    }
                    else
                    {
                        GUILayout.BeginVertical("HelpBox", GUILayout.Width(205), GUILayout.Height(320));

                        GUILayout.BeginHorizontal("Icon.OutlineBorder");
                        GUILayout.Label("Step Operation Properties", "BoldLabel");
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        _stepOperationScroll = GUILayout.BeginScrollView(_stepOperationScroll);

                        GUILayout.BeginVertical("Tooltip");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name:", GUILayout.Width(50));
                        _currentOperationObj.Name = EditorGUILayout.TextField(_currentOperationObj.Name, GUILayout.Width(130));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("GUID:", GUILayout.Width(50));
                        _currentOperationObj.GUID = EditorGUILayout.TextField(_currentOperationObj.GUID, GUILayout.Width(130));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUI.enabled = !_currentOperationObj.Instant;
                        GUILayout.Label("Elapse Time:", GUILayout.Width(80));
                        _currentOperationObj.ElapseTime = EditorGUILayout.FloatField(_currentOperationObj.ElapseTime, GUILayout.Width(40));
                        GUI.enabled = true;
                        _currentOperationObj.Instant = GUILayout.Toggle(_currentOperationObj.Instant, "Instant", GUILayout.Width(60));
                        GUILayout.EndHorizontal();

                        #region 步骤目标物体丢失，根据目标GUID重新搜寻
                        if (!_currentOperationObj.Target)
                        {
                            if (_currentOperationObj.TargetGUID != "<None>")
                            {
                                _currentOperationObj.Target = GameObject.Find(_currentOperationObj.TargetPath);
                                if (!_currentOperationObj.Target)
                                {
                                    StepTarget[] targets = FindObjectsOfType<StepTarget>();
                                    foreach (StepTarget target in targets)
                                    {
                                        if (target.GUID == _currentOperationObj.TargetGUID)
                                        {
                                            _currentOperationObj.Target = target.gameObject;
                                            _currentOperationObj.TargetPath = target.transform.FullName();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    StepTarget target = _currentOperationObj.Target.GetComponent<StepTarget>();
                                    if (!target)
                                    {
                                        target = _currentOperationObj.Target.AddComponent<StepTarget>();
                                        target.GUID = _currentOperationObj.TargetGUID;
                                    }
                                }
                            }
                        }
                        #endregion

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Target:", GUILayout.Width(50));
                        GUI.color = _currentOperationObj.Target ? Color.white : Color.gray;
                        GameObject operationObj = EditorGUILayout.ObjectField(_currentOperationObj.Target, typeof(GameObject), true, GUILayout.Width(90)) as GameObject;
                        GUI.color = Color.white;
                        GUILayout.FlexibleSpace();
                        GUI.enabled = _currentOperationObj.Target;
                        if (GUILayout.Button("Clone", "Minibutton", GUILayout.Width(40)))
                        {
                            GameObject clone = Instantiate(_currentOperationObj.Target, _currentOperationObj.Target.transform.parent);
                            clone.transform.localPosition = _currentOperationObj.Target.transform.localPosition;
                            clone.transform.localRotation = _currentOperationObj.Target.transform.localRotation;
                            clone.transform.localScale = _currentOperationObj.Target.transform.localScale;
                            clone.SetActive(true);
                            operationObj = _currentOperationObj.Target = clone;
                            Selection.activeGameObject = _currentOperationObj.Target;
                            EditorGUIUtility.PingObject(_currentOperationObj.Target);
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("GUID: " + _currentOperationObj.TargetGUID, GUILayout.Width(140));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Clear", "Minibutton", GUILayout.Width(40)))
                        {
                            operationObj = _currentOperationObj.Target = null;
                            _currentOperationObj.TargetGUID = "<None>";
                            _currentOperationObj.TargetPath = "<None>";
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        #region 步骤目标改变
                        if (operationObj != _currentOperationObj.Target)
                        {
                            if (operationObj)
                            {
                                StepTarget target = operationObj.GetComponent<StepTarget>();
                                if (!target)
                                {
                                    target = operationObj.AddComponent<StepTarget>();
                                }
                                if (target.GUID == "<None>")
                                {
                                    target.GUID = Guid.NewGuid().ToString();
                                }
                                _currentOperationObj.Target = operationObj;
                                _currentOperationObj.TargetGUID = target.GUID;
                                _currentOperationObj.TargetPath = operationObj.transform.FullName();
                            }
                        }
                        #endregion

                        GUILayout.BeginVertical("Tooltip");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Type:", GUILayout.Width(50));
                        _currentOperationObj.OperationType = (StepOperationType)EditorGUILayout.EnumPopup(_currentOperationObj.OperationType, GUILayout.Width(130));
                        GUILayout.EndHorizontal();

                        _currentOperationObj.OnEditorGUI();

                        GUILayout.EndVertical();

                        GUILayout.FlexibleSpace();

                        GUILayout.EndScrollView();

                        if (GUILayout.Button("Clone"))
                        {
                            StepOperation operationClone = _currentOperationObj.Clone();
                            operationClone.Anchor = _currentOperationObj.Anchor + new Vector2(_operationButtonWidth + 20, 0);
                            operationClone.Name += "(Clone)";
                            _currentStepObj.Operations.Add(operationClone);
                            SelectStepOperation(_currentStepObj.Operations.Count - 1);
                            Repaint();
                        }
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Delete"))
                        {
                            for (int i = 0; i < _currentStepObj.Wireds.Count; i++)
                            {
                                if (_currentStepObj.Wireds[i].Left == _currentOperation || _currentStepObj.Wireds[i].Right == _currentOperation)
                                {
                                    _currentStepObj.Wireds.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    if (_currentStepObj.Wireds[i].Left > _currentOperation)
                                    {
                                        _currentStepObj.Wireds[i].Left -= 1;
                                    }
                                    if (_currentStepObj.Wireds[i].Right > _currentOperation)
                                    {
                                        _currentStepObj.Wireds[i].Right -= 1;
                                    }
                                }
                            }

                            _currentStepObj.Operations.RemoveAt(_currentOperation);
                            SelectStepOperation(-1);
                            GUI.FocusControl(null);
                        }
                        GUI.backgroundColor = Color.white;

                        GUILayout.EndVertical();
                    }
                }
                #endregion

                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            _stepContentRect = GUILayoutUtility.GetLastRect();
        }
        /// <summary>
        /// 步骤内容GUI（可移动内容加连线）
        /// </summary>
        private void StepContentRemovableGUI()
        {
            if (_currentStep != -1)
            {
                #region 步骤的所有连线
                Vector2 offsetHalf = new Vector2(_operationButtonWidth / 2, 0);
                Vector2 offset = new Vector2(_operationButtonWidth, 0);
                for (int i = 0; i < _currentStepObj.Wireds.Count; i++)
                {
                    StepWired wired = _currentStepObj.Wireds[i];
                    if (wired.Left == -1)
                    {
                        Handles.DrawBezier(_currentStepObj.EnterAnchor + offsetHalf, _currentStepObj.Operations[wired.Right].Anchor - offsetHalf,
                        _currentStepObj.EnterAnchor + offset, _currentStepObj.Operations[wired.Right].Anchor - offset,
                        Color.white, null, 5);
                    }
                    else
                    {
                        Handles.DrawBezier(_currentStepObj.Operations[wired.Left].Anchor + offsetHalf, _currentStepObj.Operations[wired.Right].Anchor - offsetHalf,
                        _currentStepObj.Operations[wired.Left].Anchor + offset, _currentStepObj.Operations[wired.Right].Anchor - offset,
                        Color.white, null, 5);
                    }
                }
                #endregion

                #region 步骤的所有操作
                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                {
                    StepOperation operation = _currentStepObj.Operations[i];
                    Rect rectOperation = new Rect(operation.Anchor.x - _operationButtonWidth / 2, operation.Anchor.y - _operationButtonHeight / 2, _operationButtonWidth, _operationButtonHeight);
                    Rect rectLeft = new Rect(operation.Anchor.x - _operationButtonWidth / 2 - 10, operation.Anchor.y - 10, 20, 20);
                    Rect rectRight = new Rect(operation.Anchor.x + _operationButtonWidth / 2 - 10, operation.Anchor.y - 10, 20, 20);
                    string style = (_currentOperation == i ? "flow node 0 on" : "flow node 0");
                    GUI.Box(rectLeft, "", style);
                    GUI.Box(rectRight, "", style);
                    string showName = "[" + operation.OperationType + "] " + operation.Name + "\r\n" + (operation.Instant ? "Instant" : operation.ElapseTime + "s");
                    if (GUI.RepeatButton(rectOperation, showName, style))
                    {
                        GUI.FocusControl(null);
                        
                        if (Event.current.button == 0)
                        {
                            if (operation.Target && Selection.activeGameObject != operation.Target)
                            {
                                Selection.activeGameObject = operation.Target;
                                EditorGUIUtility.PingObject(operation.Target);
                            }
                            SelectStepOperation(i);
                            operation.Anchor = Event.current.mousePosition;
                            _stepContentGUIRepaint = true;
                            Repaint();
                        }
                        else if (Event.current.button == 1)
                        {
                            GenericMenu gm = new GenericMenu();
                            for (int m = 0; m < _currentStepObj.Operations.Count; m++)
                            {
                                if (i != m)
                                {
                                    int j = i;
                                    int n = m;
                                    gm.AddItem(new GUIContent("Connect or break/" + _currentStepObj.Operations[n].Name), _currentStepObj.IsExistWired(j, n), () =>
                                    {
                                        ConnectOrBreakWired(_currentStepObj, j, n);
                                    });
                                }
                            }
                            gm.ShowAsContext();
                        }
                    }
                }
                #endregion

                #region Enter
                Rect rectEnter = new Rect(_currentStepObj.EnterAnchor.x - _operationButtonWidth / 2, _currentStepObj.EnterAnchor.y - _operationButtonHeight / 2, _operationButtonWidth, _operationButtonHeight);
                if (GUI.RepeatButton(rectEnter, "Enter", "flow node 3"))
                {
                    GUI.FocusControl(null);
                    if (Event.current.button == 0)
                    {
                        _currentStepObj.EnterAnchor = Event.current.mousePosition;
                        SelectStepOperation(-1);
                        _stepContentGUIRepaint = true;
                        Repaint();
                    }
                    else if (Event.current.button == 1)
                    {
                        GenericMenu gm = new GenericMenu();
                        for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                        {
                            int j = i;
                            gm.AddItem(new GUIContent("Connect or break/" + _currentStepObj.Operations[j].Name), _currentStepObj.IsExistWired(-1, j), () =>
                            {
                                ConnectOrBreakWired(_currentStepObj, -1, j);
                            });
                        }
                        gm.ShowAsContext();
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 鼠标控制
        /// </summary>
        private void MouseControl()
        {
            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        #region MouseDown
                        if (_splitterRect.Contains(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);
                            _splitterDragging = true;
                        }
                        else if (_stepContentRect.Contains(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);
                            _mouseDownPos = Event.current.mousePosition;
                            
                            if (Event.current.button == 1)
                            {
                                if (_currentStep != -1)
                                {
                                    AddStepOperation();
                                }
                            }
                            else if (Event.current.button == 2)
                            {
                                _stepContentDragging = true;
                            }
                        }
                        else if (_stepListRect.Contains(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);
                            _mouseDownPos = Event.current.mousePosition;

                            if (Event.current.button == 1)
                            {
                                CopyOrPasteStep();
                            }
                        }
                        #endregion
                        break;
                    case EventType.MouseDrag:
                        #region MouseDrag
                        if (_splitterDragging)
                        {
                            _stepListGUIWidth += Event.current.delta.x;
                            if (_stepListGUIWidth > 500)
                            {
                                _stepListGUIWidth = 500;
                            }
                            if (_stepListGUIWidth < 100)
                            {
                                _stepListGUIWidth = 100;
                            }
                            Repaint();
                        }
                        else if (_stepContentDragging)
                        {
                            Vector2 direction = (Event.current.mousePosition - _mouseDownPos);
                            for (int i = 0; i < _contentAsset.Content[_currentStep].Operations.Count; i++)
                            {
                                _contentAsset.Content[_currentStep].Operations[i].Anchor += direction;
                            }
                            _contentAsset.Content[_currentStep].EnterAnchor += direction;
                            _mouseDownPos = Event.current.mousePosition;
                            Repaint();
                        }
                        #endregion
                        break;
                    case EventType.MouseUp:
                        #region MouseUp
                        if (_splitterDragging)
                        {
                            _splitterDragging = false;
                        }
                        if (_stepContentDragging)
                        {
                            _stepContentDragging = false;
                        }
                        #endregion
                        break;
                }
            }
        }
        /// <summary>
        /// 连接或断开连线
        /// </summary>
        private void ConnectOrBreakWired(StepContent content, int left, int right)
        {
            StepWired wiredOld = content.Wireds.Find((w) => { return w.Left == left && w.Right == right; });
            if (wiredOld == null)
            {
                StepWired wired = new StepWired();
                wired.Left = left;
                wired.Right = right;
                content.Wireds.Add(wired);
            }
            else
            {
                content.Wireds.Remove(wiredOld);
            }
        }
        /// <summary>
        /// 新增步骤操作
        /// </summary>
        private void AddStepOperation()
        {
            GenericMenu gm = new GenericMenu();
            foreach (StepOperationType type in Enum.GetValues(typeof(StepOperationType)))
            {
                gm.AddItem(new GUIContent("Add Step Operation/" + type), false, () =>
                {
                    StepOperation operation = new StepOperation();
                    operation.GUID = Guid.NewGuid().ToString();
                    operation.OperationType = type;
                    operation.Anchor = _mouseDownPos;
                    operation.Instant = false;
                    string showName = "";
                    switch (type)
                    {
                        case StepOperationType.Move:
                            showName = "移动";
                            break;
                        case StepOperationType.Rotate:
                            showName = "旋转";
                            break;
                        case StepOperationType.Scale:
                            showName = "缩放";
                            break;
                        case StepOperationType.Color:
                            showName = "颜色改变";
                            break;
                        case StepOperationType.Delay:
                            showName = "延时";
                            break;
                        case StepOperationType.Active:
                            operation.Instant = true;
                            showName = "激活";
                            break;
                        case StepOperationType.Action:
                            operation.Instant = true;
                            showName = "呼叫方法";
                            break;
                        case StepOperationType.ActionArgs:
                            operation.Instant = true;
                            showName = "呼叫方法";
                            break;
                        case StepOperationType.FSM:
                            operation.Instant = true;
                            showName = "切换状态";
                            break;
                        case StepOperationType.TextMesh:
                            operation.Instant = true;
                            showName = "3D文本";
                            break;
                        case StepOperationType.Prompt:
                            operation.Instant = true;
                            showName = "提示";
                            break;
                        case StepOperationType.CameraFollow:
                            operation.Instant = true;
                            showName = "摄像机跟随";
                            break;
                        case StepOperationType.ActiveComponent:
                            operation.Instant = true;
                            showName = "激活组件";
                            break;
                        default:
                            showName = "未知节点";
                            break;
                    }
                    operation.Name = showName + _currentStepObj.GetOperationsCout(type);
                    _contentAsset.Content[_currentStep].Operations.Add(operation);
                    Repaint();
                });
            }
            gm.ShowAsContext();
        }
        /// <summary>
        /// 选中步骤内容
        /// </summary>
        private void SelectStepContent(int currentStep)
        {
            _currentStep = currentStep;
            _currentStepObj = (_currentStep != -1 ? _contentAsset.Content[_currentStep] : null);
        }
        /// <summary>
        /// 选中步骤操作
        /// </summary>
        private void SelectStepOperation(int currentOperation)
        {
            _currentOperation = currentOperation;
            _currentOperationObj = ((_currentOperation != -1 && _currentStep != -1) ? _currentStepObj.Operations[_currentOperation] : null);
        }
        /// <summary>
        /// 步骤筛选
        /// </summary>
        private bool StepFilter(StepContent content)
        {
            switch (_stepListShowType)
            {
                case StepListShowType.ID:
                    return content.GUID.Contains(_stepListFilter);
                case StepListShowType.Name:
                    return content.Name.Contains(_stepListFilter);
                case StepListShowType.IDAndName:
                    return content.GUID.Contains(_stepListFilter) || content.Name.Contains(_stepListFilter);
            }
            return false;
        }
        /// <summary>
        /// 步骤在列表的显示名
        /// </summary>
        private string StepShowName(StepContent content)
        {
            switch (_stepListShowType)
            {
                case StepListShowType.ID:
                    return content.GUID;
                case StepListShowType.Name:
                    return content.Name;
                case StepListShowType.IDAndName:
                    return content.GUID + " " + content.Name;
            }
            return "<None>";
        }
        /// <summary>
        /// 复制、粘贴步骤
        /// </summary>
        private void CopyOrPasteStep()
        {
            GenericMenu gm = new GenericMenu();

            if (_currentStep == -1)
            {
                gm.AddDisabledItem(new GUIContent("Copy"));
            }
            else
            {
                gm.AddItem(new GUIContent("Copy " + _currentStepObj.Name), false, () =>
                {
                    GUIUtility.systemCopyBuffer = string.Format("{0}|{1}", AssetDatabase.GetAssetPath(_contentAsset), _currentStepObj.GUID);
                });
            }

            StepContent stepContent = null;
            string[] buffers = GUIUtility.systemCopyBuffer.Split('|');
            if (buffers.Length == 2)
            {
                if (buffers[0] == AssetDatabase.GetAssetPath(_contentAsset))
                {
                    stepContent = _contentAsset.Content.Find((s) => { return s.GUID == buffers[1]; });
                }
                else
                {
                    StepContentAsset stepContentAsset = AssetDatabase.LoadAssetAtPath<StepContentAsset>(buffers[0]);
                    if (stepContentAsset)
                    {
                        stepContent = stepContentAsset.Content.Find((s) => { return s.GUID == buffers[1]; });
                    }
                }
            }

            if (stepContent == null)
            {
                gm.AddDisabledItem(new GUIContent("Paste"));
            }
            else
            {
                gm.AddItem(new GUIContent("Paste " + stepContent.Name), false, () =>
                {
                    _contentAsset.Content.Add(stepContent.Clone());
                    SelectStepContent(_contentAsset.Content.Count - 1);
                    SelectStepOperation(-1);
                });
            }

            gm.ShowAsContext();
        }
        /// <summary>
        /// 最小化窗口
        /// </summary>
        private void MinimizeWindow()
        {
            _recordedPosition = position;
            minSize = new Vector2(200, 100);
            maxSize = new Vector2(200, 100);
            _isMinimize = true;
        }
        /// <summary>
        /// 最大化窗口
        /// </summary>
        private void MaximizeWindow()
        {
            minSize = new Vector2(800, 600);
            maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            position = _recordedPosition;
            _isMinimize = false;
        }

        /// <summary>
        /// 步骤列表显示方式
        /// </summary>
        private enum StepListShowType
        {
            /// <summary>
            /// 显示步骤ID
            /// </summary>
            ID,
            /// <summary>
            /// 显示步骤名称
            /// </summary>
            Name,
            /// <summary>
            /// 显示步骤ID+名称
            /// </summary>
            IDAndName
        }
    }
}