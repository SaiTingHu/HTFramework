using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class StepEditorWindow : HTFEditorWindow
    {
        public static void ShowWindow(StepContentAsset contentAsset)
        {
            StepEditorWindow window = GetWindow<StepEditorWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("BlendTree Icon").image;
            window.titleContent.text = "Step Editor";
            window._contentAsset = contentAsset;
            window._currentStep = -1;
            window._currentOperation = -1;
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            window._isMinimize = false;
            window.GetEditorStyle();
            window.Show();
        }
        
        private StepContentAsset _contentAsset;
        private int _currentStep = -1;
        private int _currentOperation = -1;
        private StepContent _currentStepObj;
        private StepOperation _currentOperationObj;
        private Texture _background;
        private bool _isShowStepContent = true;
        private bool _isShowCamControl = false;
        private bool _isShowStepOperation = true;
        private bool _isMinimize = false;
        private Rect _recordedPosition;

        private StepListShowType _stepListShowType = StepListShowType.Name;
        private bool _isShowAncillary = true;
        private bool _isShowTrigger = false;
        private bool _isShowHelper = false;
        private Rect _stepListRect;
        private Vector2 _stepListScroll = Vector3.zero;
        private string _stepListFilter = "";
        private float _stepListGUIWidth = 340;
        private bool _isMoveTo = false;
        private int _moveToIndex = 1;

        private Rect _stepContentProRect;
        private Vector2 _stepContentProScroll = Vector3.zero;

        private Rect _stepOperationProRect;
        private Vector2 _stepOperationProScroll = Vector3.zero;

        private Rect _stepContentAreaRect;
        private bool _stepContentAreaDragging = false;

        private Rect _splitterRect;
        private float _splitterWidth = 5;
        private bool _splitterDragging = false;
        
        private bool _enterDragging = false;
        private bool _stepOperationDragging = false;
        private bool _isWired = false;
        private bool _isWiredRight = false;
        private int _wiredOriginIndex;
        private bool _isBreakWired = false;

        private CameraTarget _ct;
        private MousePosition _mp;
        private MouseRotation _mr;
        private Transform _player;
        private Type _baseType = typeof(StepHelper);
        private HashSet<int> _operationIndexs = new HashSet<int>();

        private string _stepListBGStyle;

        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }
        private void OnEnable()
        {
            SelectStepContent(_currentStep);
            SelectStepOperation(_currentOperation);

            _background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/StepEditor/Texture/background.png");

            _ct = FindObjectOfType<CameraTarget>();
            _mp = FindObjectOfType<MousePosition>();
            _mr = FindObjectOfType<MouseRotation>();
            _player = null;
        }
        private void Update()
        {
            if (_contentAsset == null)
            {
                Close();
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (_isMinimize)
            {
                MinimizeGUI();
            }
            else
            {
                GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), _background, new Rect(0, 0, position.width / 50, position.height / 50));

                StepContentRemovableGUI();

                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                TitleGUI();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                StepListGUI();
                SplitterGUI();
                StepContentFixedGUI();
                GUILayout.EndHorizontal();

                EventHandle();
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
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Maximize", EditorStyles.toolbarButton))
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
            if (GUILayout.Button(_contentAsset.name, EditorStyles.toolbarButton))
            {
                Selection.activeObject = _contentAsset;
                EditorGUIUtility.PingObject(_contentAsset);
            }
            if (GUILayout.Button("Clear Unused GUID", EditorStyles.toolbarPopup))
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
            if (GUILayout.Button("Regen Step ID", EditorStyles.toolbarPopup))
            {
                StepRegenIDWindow.ShowWindow(this, _contentAsset);
            }
            _isShowStepContent = GUILayout.Toggle(_isShowStepContent, "Step Content Properties", EditorStyles.toolbarButton);
            _isShowCamControl = GUILayout.Toggle(_isShowCamControl, "Camera Control", EditorStyles.toolbarButton);
            _isShowStepOperation = GUILayout.Toggle(_isShowStepOperation, "Step Operation Properties", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Setting", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Style/StepList Background/Dark"), _stepListBGStyle == "PreBackground", () =>
                {
                    _stepListBGStyle = "PreBackground";
                    ApplyEditorStyle();
                });
                gm.AddItem(new GUIContent("Style/StepList Background/Gray Transparent"), _stepListBGStyle == "HelpBox", () =>
                {
                    _stepListBGStyle = "HelpBox";
                    ApplyEditorStyle();
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button("About", EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Browse Ease Type"), false, () =>
                {
                    Application.OpenURL(@"http://robertpenner.com/easing/easing_demo.html");
                });
                gm.AddItem(new GUIContent("CSDN Blog"), false, () =>
                {
                    Application.OpenURL(@"https://wanderer.blog.csdn.net/article/details/87712995");
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button("Minimize", EditorStyles.toolbarPopup))
            {
                MinimizeWindow();
            }
        }
        /// <summary>
        /// 步骤列表GUI
        /// </summary>
        private void StepListGUI()
        {
            GUILayout.BeginVertical(_stepListBGStyle, GUILayout.Width(_stepListGUIWidth));

            #region 筛选步骤
            GUILayout.BeginHorizontal("Icon.OutlineBorder");
            GUILayout.Label("Step Content List", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _stepListShowType = (StepListShowType)EditorGUILayout.EnumPopup(_stepListShowType, EditorStyles.toolbarPopup, GUILayout.Width(100));
            _isShowAncillary = GUILayout.Toggle(_isShowAncillary, "Ancillary", EditorStyles.toolbarButton);
            _isShowTrigger = GUILayout.Toggle(_isShowTrigger, "Trigger", EditorStyles.toolbarButton);
            _isShowHelper = GUILayout.Toggle(_isShowHelper, "Helper", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _stepListFilter = EditorGUILayout.TextField("", _stepListFilter, EditorGlobalTools.Styles.SearchTextField);
            if (GUILayout.Button("", _stepListFilter != "" ? EditorGlobalTools.Styles.SearchCancelButton : EditorGlobalTools.Styles.SearchCancelButtonEmpty))
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
                string showName = StepShowName(_contentAsset.Content[i]);
                if (showName.Contains(_stepListFilter))
                {
                    if (_isShowAncillary && _contentAsset.Content[i].Ancillary != "")
                    {
                        GUILayout.BeginHorizontal();
                        GUI.color = Color.yellow;
                        GUILayout.Label("【" + _contentAsset.Content[i].Ancillary + "】", GUILayout.Height(16));
                        GUI.color = Color.white;
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    GUI.color = _contentAsset.Content[i].TargetGUID != "<None>" ? Color.white : Color.gray;
                    string style = _currentStep == i ? "InsertionMarker" : EditorGlobalTools.Styles.Label;
                    GUIContent content = EditorGUIUtility.IconContent("Avatar Icon");
                    content.text = i + "." + showName;
                    content.tooltip = _contentAsset.Content[i].Prompt;
                    if (GUILayout.Button(content, style, GUILayout.Height(16), GUILayout.ExpandWidth(true)))
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
                    GUILayout.EndHorizontal();
                }
            }
            GUI.color = Color.white;
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
                if (GUILayout.Button("Sure", EditorStyles.miniButtonLeft))
                {
                    if (_moveToIndex >= 0 && _moveToIndex <= _contentAsset.Content.Count - 1)
                    {
                        _contentAsset.Content.RemoveAt(_currentStep);
                        _currentStep = _moveToIndex;
                        _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                    }
                    else
                    {
                        Log.Error("当前要移动到的索引不存在！");
                    }
                    _isMoveTo = false;
                }
                if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
                {
                    _isMoveTo = false;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            #endregion

            #region 添加、移动、克隆、删除步骤
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", EditorGlobalTools.Styles.ButtonLeft))
            {
                AddStepContent();
            }
            GUI.enabled = (_currentStep != -1);
            if (GUILayout.Button("Move Up", EditorGlobalTools.Styles.ButtonMid))
            {
                if (_currentStep > 0)
                {
                    _contentAsset.Content.RemoveAt(_currentStep);
                    _currentStep -= 1;
                    _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                }
            }
            if (GUILayout.Button("Move Down", EditorGlobalTools.Styles.ButtonMid))
            {
                if (_currentStep < _contentAsset.Content.Count - 1)
                {
                    _contentAsset.Content.RemoveAt(_currentStep);
                    _currentStep += 1;
                    _contentAsset.Content.Insert(_currentStep, _currentStepObj);
                }
            }
            if (GUILayout.Button("Move To", EditorGlobalTools.Styles.ButtonMid))
            {
                _isMoveTo = !_isMoveTo;
            }
            if (GUILayout.Button("Clone", EditorGlobalTools.Styles.ButtonMid))
            {
                StepContent content = _currentStepObj.Clone();
                content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
                _contentAsset.StepIDSign += 1;
                _contentAsset.Content.Add(content);
                SelectStepContent(_contentAsset.Content.Count - 1);
                SelectStepOperation(-1);
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete", EditorGlobalTools.Styles.ButtonRight))
            {
                DeleteStepContent(_currentStep);
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
            EditorGUIUtility.AddCursorRect(_splitterRect, MouseCursor.SplitResizeLeftRight);
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
                GUILayout.Label("Please select a Step Content!", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                #region 步骤的属性
                if (_isShowStepContent)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(205), GUILayout.Height(420));

                    GUILayout.BeginHorizontal("Icon.OutlineBorder");
                    GUILayout.Label("Step Content Properties", EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    _stepContentProScroll = GUILayout.BeginScrollView(_stepContentProScroll);

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
                    if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(40)))
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
                    if (GUILayout.Button("Find", EditorGlobalTools.Styles.MiniPopup, GUILayout.Width(50)))
                    {
                        GUI.FocusControl(null);
                        GenericMenu gm = new GenericMenu();
                        StringToolkit.BeginNoRepeatNaming();
                        gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName("Enter")), false, () =>
                        {
                            FindStepOperation(_currentStepObj.EnterAnchor);
                        });
                        gm.AddSeparator("");
                        for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                        {
                            int j = i;
                            gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName(_currentStepObj.Operations[j].Name)), _currentOperation == j, () =>
                            {
                                SelectStepOperation(j);

                                FindStepOperation(_currentStepObj.Operations[j].Anchor);
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
                    if (GUILayout.Button("Get", EditorStyles.miniButton, GUILayout.Width(40)))
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
                    if (GUILayout.Button("Get", EditorStyles.miniButton, GUILayout.Width(40)))
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
                    if (GUILayout.Button("Get", EditorStyles.miniButton, GUILayout.Width(40)))
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
                    if (GUILayout.Button(_currentStepObj.Helper, EditorGlobalTools.Styles.MiniPopup, GUILayout.Width(100)))
                    {
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(_baseType);
                        });
                        GenericMenu gm = new GenericMenu();
                        StringToolkit.BeginNoRepeatNaming();
                        gm.AddItem(new GUIContent("<None>"), _currentStepObj.Helper == "<None>", () =>
                        {
                            _currentStepObj.Helper = "<None>";
                        });
                        gm.AddItem(new GUIContent("<New Helper Script>"), false, () =>
                        {
                            NewHelperScript();
                        });
                        foreach (Type type in types)
                        {
                            CustomHelperAttribute helper = type.GetCustomAttribute<CustomHelperAttribute>();
                            if (helper != null)
                            {
                                gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName(helper.Name)), _currentStepObj.Helper == type.FullName, () =>
                                {
                                    _currentStepObj.Helper = type.FullName;
                                });
                            }
                        }
                        gm.ShowAsContext();
                    }
                    GUI.enabled = _currentStepObj.Helper != "<None>";
                    if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(30)))
                    {
                        OpenHelperScript(_currentStepObj.Helper);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Parameter:", GUILayout.Width(70));
                    if (GUILayout.Button("Edit Parameter " + _currentStepObj.Parameters.Count, EditorStyles.miniButton))
                    {
                        StepParameterWindow.ShowWindow(this, _contentAsset, _currentStepObj);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.EndScrollView();

                    GUILayout.EndVertical();

                    _stepContentProRect = GUILayoutUtility.GetLastRect();
                }
                else
                {
                    _stepContentProRect.Set(0, 0, 0, 0);
                }
                #endregion

                #region 摄像机组件
                if (_isShowCamControl)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(205), GUILayout.Height(120));

                    GUILayout.BeginHorizontal("Icon.OutlineBorder");
                    GUILayout.Label("Camera Control", EditorStyles.boldLabel);
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
                }
                #endregion

                GUILayout.FlexibleSpace();

                #region 步骤操作的属性
                if (_isShowStepOperation && _currentOperation != -1)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(205), GUILayout.Height(320));

                    GUILayout.BeginHorizontal("Icon.OutlineBorder");
                    GUILayout.Label("Step Operation Properties", EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    _stepOperationProScroll = GUILayout.BeginScrollView(_stepOperationProScroll);

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
                    GameObject operationObj = EditorGUILayout.ObjectField(_currentOperationObj.Target, typeof(GameObject), true, GUILayout.Width(130)) as GameObject;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("GUID: " + _currentOperationObj.TargetGUID, GUILayout.Width(140));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(40)))
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
                        operationClone.Anchor = _currentOperationObj.Anchor + new Vector2(StepOperation.Width + 20, 0);
                        operationClone.Name += "(Clone)";
                        _currentStepObj.Operations.Add(operationClone);
                        SelectStepOperation(_currentStepObj.Operations.Count - 1);
                        GUI.changed = true;
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Delete"))
                    {
                        DeleteStepOperation(_currentStepObj, _currentOperation);
                    }
                    GUI.backgroundColor = Color.white;

                    GUILayout.EndVertical();

                    _stepOperationProRect = GUILayoutUtility.GetLastRect();
                }
                else
                {
                    _stepOperationProRect.Set(0, 0, 0, 0);
                }
                #endregion

                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            _stepContentAreaRect = GUILayoutUtility.GetLastRect();
        }
        /// <summary>
        /// 步骤内容GUI（可移动内容加连线）
        /// </summary>
        private void StepContentRemovableGUI()
        {
            if (_isShowStepOperation && _currentStep != -1)
            {
                _stepContentAreaRect.Set(_stepListGUIWidth, 0, position.width - _stepListGUIWidth, position.height);
                GUILayout.BeginArea(_stepContentAreaRect);

                #region 步骤的所有连线
                Vector2 offset = new Vector2(StepOperation.Width * 1.5f, 0);
                Vector2 offsetHalf = new Vector2(StepOperation.Width / 2, 0);
                for (int i = 0; i < _currentStepObj.Wireds.Count; i++)
                {
                    StepWired wired = _currentStepObj.Wireds[i];

                    Vector2 leftAnchor;
                    Vector2 rightAnchor;
                    Vector2 leftTangent;
                    Vector2 rightTangent;
                    if (wired.Left == -1)
                    {
                        leftAnchor = _currentStepObj.EnterAnchor + offsetHalf;
                        rightAnchor = _currentStepObj.Operations[wired.Right].Anchor - offsetHalf;
                        leftTangent = _currentStepObj.EnterAnchor + offset;
                        rightTangent = _currentStepObj.Operations[wired.Right].Anchor - offset;
                    }
                    else
                    {
                        leftAnchor = _currentStepObj.Operations[wired.Left].Anchor + offsetHalf;
                        rightAnchor = _currentStepObj.Operations[wired.Right].Anchor - offsetHalf;
                        leftTangent = _currentStepObj.Operations[wired.Left].Anchor + offset;
                        rightTangent = _currentStepObj.Operations[wired.Right].Anchor - offset;
                    }
                    Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.white, null, 5);

                    if (_isBreakWired)
                    {
                        Vector2 center = (leftAnchor + rightAnchor) / 2;
                        Rect centerRect = new Rect(center.x - 8, center.y - 8, 20, 20);
                        if (GUI.Button(centerRect, "", EditorGlobalTools.Styles.OLMinus))
                        {
                            _currentStepObj.Wireds.RemoveAt(i);
                            break;
                        }
                        EditorGUIUtility.AddCursorRect(centerRect, MouseCursor.ArrowMinus);
                    }
                }
                #endregion

                #region 鼠标左键拖拽连线
                if (_isWired)
                {
                    if (_isWiredRight)
                    {
                        Vector2 leftAnchor = _currentStepObj.Operations[_wiredOriginIndex].Anchor + offsetHalf;
                        Vector2 rightAnchor = Event.current.mousePosition;
                        Vector2 leftTangent = _currentStepObj.Operations[_wiredOriginIndex].Anchor + offset;
                        Vector2 rightTangent = Event.current.mousePosition;

                        Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.white, null, 5);
                    }
                    else
                    {
                        Vector2 leftAnchor = _currentStepObj.Operations[_wiredOriginIndex].Anchor - offsetHalf;
                        Vector2 rightAnchor = Event.current.mousePosition;
                        Vector2 leftTangent = _currentStepObj.Operations[_wiredOriginIndex].Anchor - offset;
                        Vector2 rightTangent = Event.current.mousePosition;

                        Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.white, null, 5);
                    }
                }
                #endregion

                #region 步骤的所有操作
                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                {
                    StepOperation operation = _currentStepObj.Operations[i];
                    GUI.color = operation.TargetGUID != "<None>" ? Color.white : Color.gray;
                    string style = _currentOperation == i ? "flow node 0 on" : "flow node 0";
                    string showName = string.Format("[{0}] {1}\r\n{2}", operation.OperationType, operation.Name, operation.Instant ? "Instant" : (operation.ElapseTime.ToString() + "s"));
                    Rect leftRect = operation.LeftPosition;
                    Rect rightRect = operation.RightPosition;
                    Rect operationRect = operation.Position;
                    GUI.Box(leftRect, "", style);
                    GUI.Box(rightRect, "", style);
                    GUI.Box(operationRect, showName, style);
                    EditorGUIUtility.AddCursorRect(leftRect, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(rightRect, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(operationRect, MouseCursor.MoveArrow);
                }
                GUI.color = Color.white;
                #endregion

                #region Enter
                Rect enterRect = _currentStepObj.EnterPosition;
                GUI.Box(enterRect, "Enter\r\n" + _currentStepObj.Totaltime.ToString() + "s", "flow node 3");
                EditorGUIUtility.AddCursorRect(enterRect, MouseCursor.MoveArrow);
                #endregion
                
                GUILayout.EndArea();
            }
        }
        
        /// <summary>
        /// 事件处理
        /// </summary>
        private void EventHandle()
        {
            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        #region MouseDown
                        int downIndex;
                        if (_stepListRect.Contains(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 1)
                            {
                                CopyOrPasteStep();
                            }
                        }
                        else if (_splitterRect.Contains(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 0)
                            {
                                _splitterDragging = true;
                            }
                        }
                        else if (_stepContentProRect.Contains(Event.current.mousePosition))
                        { }
                        else if (_stepOperationProRect.Contains(Event.current.mousePosition))
                        { }
                        else if (ChooseEnter(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 0)
                            {
                                SelectStepOperation(-1);
                                _enterDragging = true;
                                GUI.changed = true;
                            }
                            else if (Event.current.button == 1)
                            {
                                GenericMenu gm = new GenericMenu();
                                gm.AddItem(new GUIContent("Compute total time"), false, () =>
                                {
                                    ComputeTotalTime(_currentStepObj);
                                });
                                gm.AddSeparator("");
                                StringToolkit.BeginNoRepeatNaming();
                                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                                {
                                    int j = i;
                                    gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName("Connect or break/" + _currentStepObj.Operations[j].Name)), _currentStepObj.IsExistWired(-1, j), () =>
                                    {
                                        ConnectOrBreakWired(_currentStepObj, -1, j);
                                    });
                                }
                                gm.ShowAsContext();
                            }
                        }
                        else if (ChooseOperation(Event.current.mousePosition, out downIndex))
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 0)
                            {
                                SelectStepOperation(downIndex);
                                if (_currentOperationObj.Target && Selection.activeGameObject != _currentOperationObj.Target)
                                {
                                    Selection.activeGameObject = _currentOperationObj.Target;
                                    EditorGUIUtility.PingObject(_currentOperationObj.Target);
                                }
                                _stepOperationDragging = true;
                                GUI.changed = true;
                            }
                            else if (Event.current.button == 1)
                            {
                                GenericMenu gm = new GenericMenu();
                                StringToolkit.BeginNoRepeatNaming();
                                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                                {
                                    if (i != downIndex)
                                    {
                                        int j = i;
                                        gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName("Connect or break/" + _currentStepObj.Operations[j].Name)), _currentStepObj.IsExistWired(downIndex, j), () =>
                                        {
                                            ConnectOrBreakWired(_currentStepObj, downIndex, j);
                                        });
                                    }
                                }
                                gm.ShowAsContext();
                            }
                        }
                        else if (ChooseLeftRight(Event.current.mousePosition))
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 0)
                            {
                                _isWired = true;
                            }
                        }
                        else if (_stepContentAreaRect.Contains(Event.current.mousePosition) && _currentStep != -1)
                        {
                            GUI.FocusControl(null);

                            if (Event.current.button == 1)
                            {
                                AddStepOperation(_currentStepObj, Event.current.mousePosition);
                            }
                            else if (Event.current.button == 2)
                            {
                                _stepContentAreaDragging = true;
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
                            if (_stepListGUIWidth < 340)
                            {
                                _stepListGUIWidth = 340;
                            }
                            GUI.changed = true;
                        }
                        else if(_enterDragging)
                        {
                            _currentStepObj.EnterAnchor += Event.current.delta;
                            GUI.changed = true;
                        }
                        else if (_stepOperationDragging)
                        {
                            _currentOperationObj.Anchor += Event.current.delta;
                            GUI.changed = true;
                        }
                        else if (_isWired)
                        {
                            GUI.changed = true;
                        }
                        else if (_stepContentAreaDragging)
                        {
                            for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                            {
                                _currentStepObj.Operations[i].Anchor += Event.current.delta;
                            }
                            _currentStepObj.EnterAnchor += Event.current.delta;
                            GUI.changed = true;
                        }
                        #endregion
                        break;
                    case EventType.MouseUp:
                        #region MouseUp
                        int upIndex;
                        if (ChooseEnter(Event.current.mousePosition))
                        {
                            if (_isWired)
                            {
                                ConnectOrBreakWired(_currentStepObj, -1, _wiredOriginIndex);
                            }
                        }
                        else if (ChooseOperation(Event.current.mousePosition, out upIndex))
                        {
                            if (_isWired)
                            {
                                if (_wiredOriginIndex != upIndex)
                                {
                                    if (_isWiredRight)
                                    {
                                        ConnectOrBreakWired(_currentStepObj, _wiredOriginIndex, upIndex);
                                    }
                                    else
                                    {
                                        ConnectOrBreakWired(_currentStepObj, upIndex, _wiredOriginIndex);
                                    }
                                }
                            }
                        }

                        _splitterDragging = false;
                        _enterDragging = false;
                        _stepOperationDragging = false;
                        if (_isWired)
                        {
                            _isWired = false;
                            GUI.changed = true;
                        }
                        _stepContentAreaDragging = false;
                        #endregion
                        break;
                    case EventType.KeyDown:
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.Delete:
                                if (_currentStep != -1)
                                {
                                    if (_currentOperation != -1)
                                    {
                                        DeleteStepOperation(_currentStepObj, _currentOperation);
                                    }
                                    else
                                    {
                                        DeleteStepContent(_currentStep);
                                    }
                                    GUI.changed = true;
                                }
                                break;
                            case KeyCode.DownArrow:
                                if (_currentStep == -1)
                                {
                                    if (0 < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(0);
                                        GUI.changed = true;
                                    }
                                }
                                else
                                {
                                    int stepIndex = _currentStep + 1;
                                    if (stepIndex < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(stepIndex);
                                        GUI.changed = true;
                                    }
                                }
                                break;
                            case KeyCode.UpArrow:
                                if (_currentStep == -1)
                                {
                                    if (0 < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(_contentAsset.Content.Count - 1);
                                        GUI.changed = true;
                                    }
                                }
                                else
                                {
                                    int stepIndex = _currentStep - 1;
                                    if (stepIndex >= 0 && stepIndex < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(stepIndex);
                                        GUI.changed = true;
                                    }
                                }
                                break;
                            case KeyCode.LeftAlt:
                            case KeyCode.RightAlt:
                                _isBreakWired = true;
                                GUI.changed = true;
                                break;
                        }
                        break;
                    case EventType.KeyUp:
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.LeftAlt:
                            case KeyCode.RightAlt:
                                _isBreakWired = false;
                                GUI.changed = true;
                                break;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 鼠标选中步骤Enter节点
        /// </summary>
        private bool ChooseEnter(Vector2 mousePosition)
        {
            if (_currentStep != -1)
            {
                Rect rect = _currentStepObj.EnterPosition;
                rect.x += _stepListGUIWidth;
                return rect.Contains(mousePosition);
            }
            return false;
        }
        /// <summary>
        /// 鼠标选中步骤操作节点
        /// </summary>
        private bool ChooseOperation(Vector2 mousePosition, out int index)
        {
            if (_currentStep != -1)
            {
                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                {
                    Rect rect = _currentStepObj.Operations[i].Position;
                    rect.x += _stepListGUIWidth;
                    if (rect.Contains(mousePosition))
                    {
                        index = i;
                        return true;
                    }
                }
            }
            index = -1;
            return false;
        }
        /// <summary>
        /// 鼠标选中步骤节点的左、右连线区域
        /// </summary>
        private bool ChooseLeftRight(Vector2 mousePosition)
        {
            if (_currentStep != -1)
            {
                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                {
                    Rect rect = _currentStepObj.Operations[i].LeftPosition;
                    rect.x += _stepListGUIWidth;
                    if (rect.Contains(mousePosition))
                    {
                        _isWiredRight = false;
                        _wiredOriginIndex = i;
                        return true;
                    }
                    else
                    {
                        rect = _currentStepObj.Operations[i].RightPosition;
                        rect.x += _stepListGUIWidth;
                        if (rect.Contains(mousePosition))
                        {
                            _isWiredRight = true;
                            _wiredOriginIndex = i;
                            return true;
                        }
                    }
                }
            }
            return false;
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
        /// 新增步骤内容
        /// </summary>
        private void AddStepContent()
        {
            StepContent content = new StepContent();
            content.GUID = Guid.NewGuid().ToString();
            content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
            _contentAsset.StepIDSign += 1;
            content.EnterAnchor = new Vector2(position.width / 2, position.height / 2);
            _contentAsset.Content.Add(content);
            SelectStepContent(_contentAsset.Content.Count - 1);
            SelectStepOperation(-1);
        }
        /// <summary>
        /// 删除步骤内容
        /// </summary>
        private void DeleteStepContent(int contentIndex)
        {
            if (EditorUtility.DisplayDialog("Prompt", "Are you sure delete step " + _contentAsset.Content[contentIndex].Name + "？", "Yes", "No"))
            {
                _contentAsset.Content.RemoveAt(contentIndex);
                SelectStepContent(-1);
                SelectStepOperation(-1);
            }
        }
        /// <summary>
        /// 新增步骤操作
        /// </summary>
        private void AddStepOperation(StepContent content, Vector2 position)
        {
            GenericMenu gm = new GenericMenu();
            foreach (StepOperationType type in Enum.GetValues(typeof(StepOperationType)))
            {
                gm.AddItem(new GUIContent("Add Step Operation/" + type), false, () =>
                {
                    StepOperation operation = new StepOperation();
                    operation.GUID = Guid.NewGuid().ToString();
                    operation.OperationType = type;
                    operation.Anchor = position - new Vector2(340, 0);
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
                    operation.Name = showName + content.GetOperationsCout(type);
                    content.Operations.Add(operation);
                    GUI.changed = true;
                });
            }
            gm.ShowAsContext();
        }
        /// <summary>
        /// 删除步骤操作
        /// </summary>
        private void DeleteStepOperation(StepContent content, int operationIndex)
        {
            for (int i = 0; i < content.Wireds.Count; i++)
            {
                if (content.Wireds[i].Left == operationIndex || content.Wireds[i].Right == operationIndex)
                {
                    content.Wireds.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (content.Wireds[i].Left > operationIndex)
                    {
                        content.Wireds[i].Left -= 1;
                    }
                    if (content.Wireds[i].Right > operationIndex)
                    {
                        content.Wireds[i].Right -= 1;
                    }
                }
            }

            content.Operations.RemoveAt(operationIndex);
            SelectStepOperation(-1);
            GUI.FocusControl(null);
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
        /// 查找步骤操作
        /// </summary>
        private void FindStepOperation(Vector2 operationAnchor)
        {
            Vector2 direction = new Vector2((position.width - _stepListGUIWidth) / 2, position.height / 2) - operationAnchor;
            for (int i = 0; i < _currentStepObj.Operations.Count; i++)
            {
                _currentStepObj.Operations[i].Anchor += direction;
            }
            _currentStepObj.EnterAnchor += direction;
        }
        /// <summary>
        /// 步骤在列表的显示名
        /// </summary>
        private string StepShowName(StepContent content)
        {
            string showName;
            switch (_stepListShowType)
            {
                case StepListShowType.ID:
                    showName = content.GUID;
                    break;
                case StepListShowType.Name:
                    showName = content.Name;
                    break;
                case StepListShowType.IDAndName:
                    showName = content.GUID + " " + content.Name;
                    break;
                default:
                    showName = "<None>";
                    break;
            }
            if (_isShowTrigger)
            {
                showName += " [" + content.Trigger.ToString() + "]";
            }
            if (_isShowHelper && content.Helper != "<None>")
            {
                showName += " [" + content.Helper + "]";
            }
            return showName;
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
                    StepContent content = stepContent.Clone();
                    content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
                    _contentAsset.StepIDSign += 1;
                    _contentAsset.Content.Add(content);
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
        /// 获取并设置编辑器样式
        /// </summary>
        private void GetEditorStyle()
        {
            _stepListBGStyle = EditorPrefs.GetString(EditorPrefsTable.Style_StepEditor_StepListBG, "PreBackground");
        }
        /// <summary>
        /// 应用并存储编辑器样式
        /// </summary>
        private void ApplyEditorStyle()
        {
            EditorPrefs.SetString(EditorPrefsTable.Style_StepEditor_StepListBG, _stepListBGStyle);
        }

        /// <summary>
        /// 打开助手脚本
        /// </summary>
        public void OpenHelperScript(string helper)
        {
            MonoScriptToolkit.OpenMonoScript(helper);
        }
        /// <summary>
        /// 新建助手脚本
        /// </summary>
        private void NewHelperScript()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_StepHelper_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 Helper 类", directory, "NewHelper", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!MonoScriptToolkit.IsExistMonoScriptName(className))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/StepHelperTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        code = code.Replace("#HELPERNAME#", className);
                        File.AppendAllText(path, code);
                        _currentStepObj.Helper = className;
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(cs);
                        Selection.activeObject = cs;
                        AssetDatabase.OpenAsset(cs);
                        EditorPrefs.SetString(EditorPrefsTable.Script_StepHelper_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    Log.Error("新建Helper失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 计算Enter节点开始执行的所有操作的总时间
        /// </summary>
        private void ComputeTotalTime(StepContent content)
        {
            content.GetExecuteTwice(_operationIndexs);
            foreach (var item in _operationIndexs)
            {
                Log.Warning("注意：操作节点【" + content.Operations[item].Name + "】有两次或以上连线接入，可能会被多次执行！");
            }
            _operationIndexs.Clear();

            float totalTime = 0;
            content.GetTerminus(_operationIndexs);
            foreach (var item in _operationIndexs)
            {
                float time = content.ComputeTotalTime(item);
                if (time > totalTime)
                {
                    totalTime = time;
                }
            }
            _operationIndexs.Clear();

            content.Totaltime = totalTime;
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