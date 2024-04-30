using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤编辑器窗口
    /// </summary>
    public sealed class StepEditorWindow : HTFEditorWindow, ILocalizeWindow
    {
        private static HTFAction<StepContent> AddStepContentHandler;
        private static HTFFunc<string, string> NewHelperScriptHandler;
        private static HTFFunc<ControlMode, Vector3> GetBestViewHandler;
        private static Dictionary<string, HTFFunc<StepContent, bool>> AdvancedSearchHandlers = new Dictionary<string, HTFFunc<StepContent, bool>>();
        internal static HTFAction<StepParameter> CustomParameterHandler;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static StepEditorWindow()
        {
            AdvancedSearchHandlers.Add("禁用的步骤", (stepContent) =>
            {
                return !stepContent.IsEnable;
            });
            AdvancedSearchHandlers.Add("存在空目标的步骤", (stepContent) =>
            {
                if (stepContent.TargetGUID == "<None>")
                {
                    return true;
                }
                else
                {
                    return stepContent.Operations.Exists((o) =>
                    {
                        return o.TargetGUID == "<None>";
                    });
                }
            });
            AdvancedSearchHandlers.Add("名称、提示中存在空格的步骤", (stepContent) =>
            {
                return stepContent.Name.Contains(' ') || stepContent.Prompt.Contains(' ');
            });
            AdvancedSearchHandlers.Add("包含<行为>节点的步骤", (stepContent) =>
            {
                return stepContent.Operations.Exists((o) =>
                {
                    return o.OperationType == StepOperationType.Action || o.OperationType == StepOperationType.ActionArgs;
                });
            });
            AdvancedSearchHandlers.Add("包含<播放时间线>节点的步骤", (stepContent) =>
            {
                return stepContent.Operations.Exists((o) =>
                {
                    return o.OperationType == StepOperationType.PlayTimeline;
                });
            });
            AdvancedSearchHandlers.Add("包含自定义<Custom>参数的步骤", (stepContent) =>
            {
                return stepContent.Parameters.Exists((p) => { return p.Type == StepParameter.ParameterType.Custom; });
            });
        }
        /// <summary>
        /// 注册【新增步骤内容】时的自定义处理者
        /// </summary>
        /// <param name="handler">自定义处理者</param>
        public static void RegisterAddStepContentHandler(HTFAction<StepContent> handler)
        {
            AddStepContentHandler = handler;
        }
        /// <summary>
        /// 注册【新建步骤助手脚本】时的自定义处理者
        /// </summary>
        /// <param name="handler">自定义处理者</param>
        public static void RegisterNewHelperScriptHandler(HTFFunc<string, string> handler)
        {
            NewHelperScriptHandler = handler;
        }
        /// <summary>
        /// 注册【获取步骤最佳视角】时的自定义处理者（第一人称、第三人称控制模式）
        /// </summary>
        /// <param name="handler">自定义处理者</param>
        public static void RegisterGetBestViewHandler(HTFFunc<ControlMode, Vector3> handler)
        {
            GetBestViewHandler = handler;
        }
        /// <summary>
        /// 注册【步骤列表高级筛查】的自定义筛查方法
        /// </summary>
        /// <param name="name">筛查名称</param>
        /// <param name="handler">筛查方法</param>
        public static void RegisterAdvancedSearchHandler(string name, HTFFunc<StepContent, bool> handler)
        {
            if (AdvancedSearchHandlers.ContainsKey(name))
                return;

            AdvancedSearchHandlers.Add(name, handler);
        }
        /// <summary>
        /// 注册【编辑步骤参数（自定义类型）】时的自定义处理者
        /// </summary>
        /// <param name="handler">自定义处理者</param>
        public static void RegisterCustomParameterHandler(HTFAction<StepParameter> handler)
        {
            CustomParameterHandler = handler;
        }
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="contentAsset">步骤资源</param>
        public static void ShowWindow(StepContentAsset contentAsset)
        {
            StepEditorWindow window = GetWindow<StepEditorWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("BlendTree Icon").image;
            window.titleContent.text = "Step Editor";
            window._contentAsset = contentAsset;
            window._currentStepIndex = -1;
            window._currentOperationIndex = -1;
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            window._isMinimize = false;
            window.Show();
        }
        
        private StepContentAsset _contentAsset;
        private int _currentStepIndex = -1;
        private int _currentOperationIndex = -1;
        private StepContent _currentStepObj;
        private StepOperation _currentOperationObj;
        private Texture _background;
        private bool _isShowStepContent = true;
        private bool _isShowCamControl = false;
        private bool _isShowStepOperation = true;
        private bool _isMinimize = false;
        private Rect _recordedPosition;

        private StepListShowType _stepListShowType = StepListShowType.Name;
        private string[] _stepListTypes = new string[] { "ID", "Name", "IDAndName" };
        private string[] _triggers = new string[] { "MouseClick", "ButtonClick", "StateChange", "AutoExecute" };
        private string[] _initialModes = new string[] { "FreeControl", "FirstPerson", "ThirdPerson" };
        private bool _isShowAncillary = true;
        private bool _isShowTrigger = false;
        private bool _isShowHelper = false;
        private bool _isLockID = true;
        private GUIContent _stepGC;
        private GUIContent _enableGC;
        private GUIContent _disableGC;
        private GUIContent _stepHelperGC;
        private GUIContent _previewGC;
        private GUIContent _advancedSearchGC;
        private GUIContent _valueEditorGC;
        private GUIContent _editGC;
        private GUIContent _deleteGC;
        private Rect _stepListRect;
        private Vector2 _stepListScroll = Vector3.zero;
        private string _stepListFilter = null;
        private string _stepListAdvancedSearch = null;
        private float _stepListGUIWidth = 340;
        private bool _isMoveTo = false;
        private int _moveToIndex = 1;

        private Rect _stepContentProRect;
        private Rect _stepOperationProRect;
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
        private readonly Type _baseType = typeof(StepHelper);
        private HashSet<int> _operationIndexs = new HashSet<int>();
        private HTFFunc<string, string> _getWord;
        private string[] _operationTypes = new string[] {
            "Move","Rotate","Scale","Color","Delay"
            ,"Active","Action","ActionArgs","FSM","TextMesh"
            ,"Prompt","CameraFollow","ActiveComponent","Transform","ChangeParent"
            ,"PlayTimeline"
        };
        
        protected override bool IsEnableTitleGUI
        {
            get
            {
                return !_isMinimize;
            }
        }
        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/87712995";

        protected override void OnEnable()
        {
            base.OnEnable();

            SelectStepContent(_currentStepIndex);
            SelectStepOperation(_currentOperationIndex);

            _stepGC = new GUIContent();
            _stepGC.image = EditorGUIUtility.IconContent("AnimatorStateTransition Icon").image;
            _enableGC = new GUIContent();
            _enableGC.image = EditorGUIUtility.IconContent("animationvisibilitytoggleon").image;
            _enableGC.tooltip = "Enable";
            _disableGC = new GUIContent();
            _disableGC.image = EditorGUIUtility.IconContent("animationvisibilitytoggleoff").image;
            _disableGC.tooltip = "Disable";
            _stepHelperGC = new GUIContent();
            _previewGC = new GUIContent();
            _previewGC.image = EditorGUIUtility.IconContent("TrailRenderer Icon").image;
            _advancedSearchGC = new GUIContent();
            _advancedSearchGC.image = EditorGUIUtility.IconContent("FilterByType").image;
            _advancedSearchGC.tooltip = "Advanced Search";
            _valueEditorGC = new GUIContent();
            _valueEditorGC.image = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image;
            _valueEditorGC.tooltip = "Edit in a new window";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit";
            _deleteGC = new GUIContent();
            _deleteGC.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _deleteGC.tooltip = "Delete";
            _background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Grid.png");

            _ct = FindObjectOfType<CameraTarget>();
            _mp = FindObjectOfType<MousePosition>();
            _mr = FindObjectOfType<MouseRotation>();
            _player = null;
            _getWord = GetWord;
        }
        private void Update()
        {
            if (_contentAsset == null)
            {
                Close();
            }
        }
        private void OnDestroy()
        {
            StopPreviewInAllStep();
        }
        protected override void OnGUIReady()
        {
            base.OnGUIReady();

            if (_isMinimize)
                return;

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), _background, new Rect(0, 0, position.width / 50, position.height / 50));

            StepContentMovableGUI();
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            if (GUILayout.Button(_contentAsset.name, EditorStyles.toolbarButton))
            {
                Selection.activeObject = _contentAsset;
                EditorGUIUtility.PingObject(_contentAsset);
            }
            if (GUILayout.Button(GetWord("Clear Unused GUID"), EditorStyles.toolbarPopup))
            {
                string prompt = CurrentLanguage == Language.English
                    ? "Are you sure clear unused GUID [StepTarget] in the current opened scene？"
                    : "你确定要从当前打开的场景中清除所有未使用的步骤目标脚本 [StepTarget] 吗？";
                if (EditorUtility.DisplayDialog(GetWord("Prompt"), prompt, GetWord("Yes"), GetWord("No")))
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

                        for (int j = 0; j < content.Parameters.Count; j++)
                        {
                            StepParameter parameter = content.Parameters[j];
                            if (parameter.Type == StepParameter.ParameterType.GameObject && !usedTargets.Contains(parameter.GameObjectGUID) && parameter.GameObjectGUID != "<None>")
                            {
                                usedTargets.Add(parameter.GameObjectGUID);
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
                                GameObject obj = target.gameObject;
                                DestroyImmediate(target);
                                HasChanged(obj);
                            }
                        }
                    }

                    PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null)
                    {
                        StepTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<StepTarget>(true);
                        foreach (StepTarget target in targets)
                        {
                            if (!usedTargets.Contains(target.GUID))
                            {
                                GameObject obj = target.gameObject;
                                DestroyImmediate(target);
                                HasChanged(obj);
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button(GetWord("Regen Step ID"), EditorStyles.toolbarPopup))
            {
                StepRegenIDWindow.ShowWindow(this, _contentAsset, CurrentLanguage);
            }
            _isShowStepContent = GUILayout.Toggle(_isShowStepContent, GetWord("Step Content Properties"), EditorStyles.toolbarButton);
            _isShowCamControl = GUILayout.Toggle(_isShowCamControl, GetWord("Camera Control"), EditorStyles.toolbarButton);
            _isShowStepOperation = GUILayout.Toggle(_isShowStepOperation, GetWord("Step Operation Properties"), EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GetWord("Setting"), EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent(GetWord("Lock ID")), _isLockID, () =>
                {
                    _isLockID = !_isLockID;
                });
                string content = string.Format("{0}/{1}", GetWord("Preview"), GetWord("Stop Preview Current Step"));
                gm.AddItem(new GUIContent(content), false, () =>
                {
                    StopPreviewInStep(_currentStepIndex);
                });
                content = string.Format("{0}/{1}", GetWord("Preview"), GetWord("Stop Preview All Step"));
                gm.AddItem(new GUIContent(content), false, () =>
                {
                    StopPreviewInAllStep();
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button(GetWord("Minimize"), EditorStyles.toolbarPopup))
            {
                MinimizeWindow();
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
                GUILayout.BeginHorizontal();
                StepListGUI();
                SplitterGUI();
                StepContentFixedGUI();
                GUILayout.EndHorizontal();

                EventHandle();
            }

            if (GUI.changed)
            {
                HasChanged(_contentAsset);
            }
        }
        protected override void GenerateWords()
        {
            base.GenerateWords();

            AddWord("清除未使用的步骤目标", "Clear Unused GUID");
            AddWord("重新生成步骤身份号", "Regen Step ID");
            AddWord("步骤内容的属性", "Step Content Properties");
            AddWord("摄像机控制组件", "Camera Control");
            AddWord("步骤操作的属性", "Step Operation Properties");
            AddWord("设置", "Setting");
            AddWord("锁定身份号", "Lock ID");
            AddWord("最小化", "Minimize");
            AddWord("风格", "Style");
            AddWord("步骤内容列表 [背景]", "StepContentList [BG]");
            AddWord("深色", "Dark");
            AddWord("灰色", "Gray");
            AddWord("预览", "Preview");
            AddWord("停止当前步骤的预览", "Stop Preview Current Step");
            AddWord("停止所有步骤的预览", "Stop Preview All Step");
            AddWord("最大化", "Maximize");
            AddWord("步骤数量", "Step Count");
            AddWord("步骤内容列表", "Step Content List");
            AddWord("身份号", "ID");
            AddWord("名称", "Name");
            AddWord("身份号和名称", "IDAndName");
            AddWord("附加信息", "Ancillary");
            AddWord("触发方式", "Trigger");
            AddWord("鼠标点击目标", "MouseClick");
            AddWord("按钮被点击", "ButtonClick");
            AddWord("目标状态改变", "StateChange");
            AddWord("自动执行", "AutoExecute");
            AddWord("脚本助手", "Helper");
            AddWord("移动到", "Move To");
            AddWord("确定", "Sure");
            AddWord("取消", "Cancel");
            AddWord("添加", "Add");
            AddWord("上移", "Move Up");
            AddWord("下移", "Move Down");
            AddWord("克隆", "Clone");
            AddWord("删除", "Delete");
            AddWord("身份号", "GUID");
            AddWord("执行时间", "Elapse Time");
            AddWord("立即执行", "Instant");
            AddWord("目标", "Target");
            AddWord("复制", "Copy");
            AddWord("粘贴", "Paste");
            AddWord("清除", "Clear");
            AddWord("提示", "Prompt");
            AddWord("是否显示", "Is Display");
            AddWord("操作", "Operation");
            AddWord("查找", "Find");
            AddWord("进入", "Enter");
            AddWord("初始控制模式", "Initial Mode");
            AddWord("自由视角", "FreeControl");
            AddWord("第一人称", "FirstPerson");
            AddWord("第三人称", "ThirdPerson");
            AddWord("最佳视角", "Best View");
            AddWord("获取", "Get");
            AddWord("视点偏移", "View Offset");
            AddWord("最佳位置", "Best Pos");
            AddWord("<无>", "<None>");
            AddWord("<新建助手脚本>", "<New Helper Script>");
            AddWord("编辑", "Edit");
            AddWord("脚本参数", "Parameter");
            AddWord("编辑参数", "Edit Parameter");
            AddWord("重新查找", "ReFind");
            AddWord("类型", "Type");
            AddWord("停止", "Stop");
            AddWord("是的", "Yes");
            AddWord("不", "No");
            AddWord("计算总时间", "Compute total time");
            AddWord("连接或断开", "Connect or break");
            AddWord("添加步骤操作", "Add Step Operation");
            AddWord("缓动类型", "Ease");
            AddWord("立即变换", "Transformation");
            AddWord("旋转到", "Rotate To");
            AddWord("是否为轴累加形式", "Is Axis Add");
            AddWord("缩放到", "Scale To");
            AddWord("颜色变换到", "Color To");
            AddWord("作用于渲染器", "Act Renderer");
            AddWord("作用于图形组件", "Act Graphic");
            AddWord("设置激活", "Set Active");
            AddWord("行为", "Action");
            AddWord("参数", "Args");
            AddWord("注视位置", "Look Point");
            AddWord("注视角度", "Look Angle");
            AddWord("注视距离", "Look Distance");
            AddWord("文本改变到", "TextMesh To");
            AddWord("切换状态到", "FSM Switch State To");
            AddWord("状态", "State");
            AddWord("延时是无效的", "Delay Time Is Invalid");
            AddWord("延时", "Delay Time");
            AddWord("秒", "Second");
            AddWord("组件", "Component");
            AddWord("父亲", "Parent");
            AddWord("移动", "Move");
            AddWord("旋转", "Rotate");
            AddWord("缩放", "Scale");
            AddWord("颜色", "Color");
            AddWord("延时", "Delay");
            AddWord("激活", "Active");
            AddWord("行为（带参数）", "ActionArgs");
            AddWord("状态机", "FSM");
            AddWord("网格文本", "TextMesh");
            AddWord("摄像机跟随", "CameraFollow");
            AddWord("激活组件", "ActiveComponent");
            AddWord("变换", "Transform");
            AddWord("改变父级", "ChangeParent");
            AddWord("播放时间线", "PlayTimeline");
            AddWord("持续的时间", "Duration");
            AddWord("初始的时间", "Initial Time");
            AddWord("启用所有步骤", "Enable All Steps");
            AddWord("禁用所有步骤", "Disable All Steps");
        }
        protected override void OnLanguageChanged()
        {
            base.OnLanguageChanged();

            for (int i = 0; i < _stepListTypes.Length; i++)
            {
                _stepListTypes[i] = GetWord(((StepListShowType)i).ToString());
            }
            for (int i = 0; i < _triggers.Length; i++)
            {
                _triggers[i] = GetWord(((StepTrigger)i).ToString());
            }
            for (int i = 0; i < _initialModes.Length; i++)
            {
                _initialModes[i] = GetWord(((ControlMode)i).ToString());
            }
            for (int i = 0; i < _operationTypes.Length; i++)
            {
                _operationTypes[i] = GetWord(((StepOperationType)i).ToString());
            }
        }

        /// <summary>
        /// 最小化后的GUI
        /// </summary>
        private void MinimizeGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GetWord("Maximize"), EditorStyles.toolbarButton))
            {
                MaximizeWindow();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label($"{GetWord("Step Count")}:{_contentAsset.Content.Count}");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        /// <summary>
        /// 步骤列表GUI
        /// </summary>
        private void StepListGUI()
        {
            GUILayout.BeginVertical("PreBackground", GUILayout.Width(_stepListGUIWidth));

            #region 筛选步骤
            GUILayout.Space(5);

            GUILayout.BeginHorizontal("TE NodeBoxSelected");
            GUILayout.Label(GetWord("Step Content List"), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _stepListShowType = (StepListShowType)EditorGUILayout.Popup((int)_stepListShowType, _stepListTypes, EditorStyles.toolbarPopup, GUILayout.Width(100));
            _isShowAncillary = GUILayout.Toggle(_isShowAncillary, GetWord("Ancillary"), EditorStyles.toolbarButton);
            _isShowTrigger = GUILayout.Toggle(_isShowTrigger, GetWord("Trigger"), EditorStyles.toolbarButton);
            _isShowHelper = GUILayout.Toggle(_isShowHelper, GetWord("Helper"), EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _stepListFilter = EditorGUILayout.TextField("", _stepListFilter, EditorGlobalTools.Styles.SearchTextField);
            if (GUILayout.Button("", string.IsNullOrEmpty(_stepListFilter) ? EditorGlobalTools.Styles.SearchCancelButtonEmpty : EditorGlobalTools.Styles.SearchCancelButton))
            {
                _stepListFilter = null;
                GUI.FocusControl(null);
            }
            GUI.color = string.IsNullOrEmpty(_stepListAdvancedSearch) ? Color.white : Color.yellow;
            if (GUILayout.Button(_advancedSearchGC, "InvisibleButton", GUILayout.Width(20)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent(GetWord("<None>")), string.IsNullOrEmpty(_stepListAdvancedSearch), () =>
                {
                    AdvancedSearchChange(null);
                });
                foreach (var handler in AdvancedSearchHandlers)
                {
                    gm.AddItem(new GUIContent(handler.Key), _stepListAdvancedSearch == handler.Key, () =>
                    {
                        AdvancedSearchChange(handler.Key);
                    });
                }
                gm.ShowAsContext();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            #endregion

            #region 步骤列表
            _stepListScroll = GUILayout.BeginScrollView(_stepListScroll);
            for (int i = 0; i < _contentAsset.Content.Count; i++)
            {
                string showName = StepShowName(_contentAsset.Content[i]);
                if (string.IsNullOrEmpty(_stepListFilter) || showName.Contains(_stepListFilter))
                {
                    if (string.IsNullOrEmpty(_stepListAdvancedSearch) || _contentAsset.Content[i].IsSearched)
                    {
                        GUILayout.BeginHorizontal();
                        GUI.color = _contentAsset.Content[i].IsEnable ? Color.white : Color.gray;
                        GUILayout.Label(_stepGC, GUILayout.Height(20), GUILayout.Width(20));
                        if (_isShowAncillary && !string.IsNullOrEmpty(_contentAsset.Content[i].Ancillary))
                        {
                            GUI.color = Color.yellow;
                            GUILayout.Label($"[{_contentAsset.Content[i].Ancillary}]", GUILayout.Height(16));
                        }
                        GUI.color = _contentAsset.Content[i].IsEnable ? Color.white : Color.gray;
                        GUI.backgroundColor = _currentStepIndex == i ? Color.cyan : Color.white;
                        string style = _currentStepIndex == i ? "TV Selection" : "PrefixLabel";
                        if (GUILayout.Button($"{i}.{showName}", style, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                        {
                            SelectStepContent(i);
                            SelectStepOperation(-1);
                            GUI.FocusControl(null);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(_contentAsset.Content[i].IsEnable ? _enableGC : _disableGC, "InvisibleButton", GUILayout.Height(20), GUILayout.Width(20)))
                        {
                            _contentAsset.Content[i].IsEnable = !_contentAsset.Content[i].IsEnable;
                            HasChanged(_contentAsset);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.EndScrollView();
            #endregion

            GUILayout.FlexibleSpace();

            #region 移动到
            if (_isMoveTo)
            {
                GUILayout.BeginHorizontal();
                GUI.enabled = _currentStepIndex != -1;
                GUILayout.Label(GetWord("Move To") + ": ");
                _moveToIndex = EditorGUILayout.IntField(_moveToIndex);
                if (GUILayout.Button(GetWord("Sure"), EditorStyles.miniButtonLeft))
                {
                    if (_moveToIndex >= 0 && _moveToIndex <= _contentAsset.Content.Count - 1)
                    {
                        _contentAsset.Content.RemoveAt(_currentStepIndex);
                        _currentStepIndex = _moveToIndex;
                        _contentAsset.Content.Insert(_currentStepIndex, _currentStepObj);
                        SetStepListScroll((float)_currentStepIndex / _contentAsset.Content.Count);
                    }
                    else
                    {
                        Log.Error("当前要移动到的索引不存在！");
                    }
                    _isMoveTo = false;
                }
                if (GUILayout.Button(GetWord("Cancel"), EditorStyles.miniButtonRight))
                {
                    _isMoveTo = false;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            #endregion

            #region 添加、移动、克隆、删除步骤
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetWord("Add"), EditorGlobalTools.Styles.ButtonLeft))
            {
                AddStepContent();
            }
            GUI.enabled = (_currentStepIndex != -1);
            if (GUILayout.Button(GetWord("Move Up"), EditorGlobalTools.Styles.ButtonMid))
            {
                MoveUpStepContent();
            }
            if (GUILayout.Button(GetWord("Move Down"), EditorGlobalTools.Styles.ButtonMid))
            {
                MoveDownStepContent();
            }
            if (GUILayout.Button(GetWord("Move To"), EditorGlobalTools.Styles.ButtonMid))
            {
                _isMoveTo = !_isMoveTo;
            }
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button(GetWord("Clone"), EditorGlobalTools.Styles.ButtonMid))
            {
                CloneStepContent();
            }
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button(GetWord("Delete"), EditorGlobalTools.Styles.ButtonRight))
            {
                DeleteStepContent(_currentStepIndex);
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

            if (_currentStepIndex == -1)
            {
                GUILayout.BeginHorizontal();
                string prompt = CurrentLanguage == Language.English ? "Please select a Step Content!" : "请选择一个步骤内容！";
                GUILayout.Label(prompt, EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                #region 步骤的属性
                if (_isShowStepContent)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(210), GUILayout.Height(450));

                    GUILayout.BeginHorizontal("TE NodeBoxSelected");
                    GUILayout.Label(GetWord("Step Content Properties"), EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginVertical("Tooltip");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Name") + ":", GUILayout.Width(50));
                    _currentStepObj.Name = EditorGUILayout.TextField(_currentStepObj.Name, GUILayout.Width(135));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("GUID") + ":", GUILayout.Width(50));
                    GUI.enabled = !_isLockID;
                    _currentStepObj.GUID = EditorGUILayout.TextField(_currentStepObj.GUID, GUILayout.Width(135));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUI.enabled = !_currentStepObj.Instant;
                    GUILayout.Label(GetWord("Elapse Time") + ":", GUILayout.Width(80));
                    _currentStepObj.ElapseTime = EditorGUILayout.FloatField(_currentStepObj.ElapseTime, GUILayout.Width(35));
                    GUI.enabled = true;
                    _currentStepObj.Instant = GUILayout.Toggle(_currentStepObj.Instant, GetWord("Instant"), GUILayout.Width(65));
                    GUILayout.EndHorizontal();

                    SearchStepTarget(_currentStepObj);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Target") + ":", GUILayout.Width(50));
                    GUI.color = _currentStepObj.TargetGUID != "<None>" ? Color.white : Color.gray;
                    GameObject contentObj = EditorGUILayout.ObjectField(_currentStepObj.Target, typeof(GameObject), true, GUILayout.Width(135)) as GameObject;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("GUID") + ":", "Label", GUILayout.Width(45)))
                    {
                        GenericMenu gm = new GenericMenu();
                        if (_currentStepObj.TargetGUID == "<None>")
                        {
                            gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(GetWord("Copy")), false, () =>
                            {
                                GUIUtility.systemCopyBuffer = _currentStepObj.TargetGUID;
                            });
                        }
                        if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                        {
                            gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(GetWord("Paste")), false, () =>
                            {
                                _currentStepObj.TargetGUID = GUIUtility.systemCopyBuffer;
                            });
                        }
                        gm.ShowAsContext();
                    }
                    GUILayout.Label(_currentStepObj.TargetGUID == "<None>" ? GetWord(_currentStepObj.TargetGUID) : _currentStepObj.TargetGUID, GUILayout.Width(115));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(_deleteGC, "InvisibleButton", GUILayout.Width(20)))
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
                                HasChanged(contentObj);
                            }
                            if (target.GUID == "<None>")
                            {
                                target.GUID = Guid.NewGuid().ToString();
                                HasChanged(target);
                            }
                            _currentStepObj.Target = contentObj;
                            _currentStepObj.TargetGUID = target.GUID;
                            _currentStepObj.TargetPath = contentObj.transform.FullName();
                        }
                    }
                    #endregion

                    GUILayout.BeginVertical("Tooltip");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Prompt") + ":", GUILayout.Width(105));
                    _currentStepObj.IsDisplayPrompt = GUILayout.Toggle(_currentStepObj.IsDisplayPrompt, GetWord("Is Display"), GUILayout.Width(80));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    _currentStepObj.Prompt = EditorGUILayout.TextField(_currentStepObj.Prompt, GUILayout.Width(165));
                    if (GUILayout.Button(_valueEditorGC, "IconButton", GUILayout.Width(20)))
                    {
                        StringValueEditor.OpenWindow(this, _currentStepObj.Prompt, $"{_currentStepIndex}.{_currentStepObj.Name}({GetWord("Prompt")})", (str) =>
                        {
                            _currentStepObj.Prompt = str;
                            HasChanged(_contentAsset);
                        });
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Ancillary") + ":", GUILayout.Width(185));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    _currentStepObj.Ancillary = EditorGUILayout.TextField(_currentStepObj.Ancillary, GUILayout.Width(185));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{GetWord("Operation")}: {_currentStepObj.Operations.Count}", GUILayout.Width(135));
                    if (GUILayout.Button(GetWord("Find"), EditorStyles.popup, GUILayout.Width(50)))
                    {
                        GUI.FocusControl(null);
                        GenericMenu gm = new GenericMenu();
                        StringToolkit.BeginNoRepeatNaming();
                        gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName(GetWord("Enter"))), false, () =>
                        {
                            FindStepOperation(_currentStepObj.EnterAnchor);
                        });
                        gm.AddSeparator("");
                        for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                        {
                            int j = i;
                            gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName(_currentStepObj.Operations[j].Name)), _currentOperationIndex == j, () =>
                            {
                                SelectStepOperation(j);

                                FindStepOperation(_currentStepObj.Operations[j].Anchor);
                            });
                        }
                        gm.ShowAsContext();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Trigger") + ":", GUILayout.Width(85));
                    _currentStepObj.Trigger = (StepTrigger)EditorGUILayout.Popup((int)_currentStepObj.Trigger, _triggers, GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Initial Mode") + ":", GUILayout.Width(85));
                    _currentStepObj.InitialMode = (ControlMode)EditorGUILayout.Popup((int)_currentStepObj.InitialMode, _initialModes, GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Best View") + ":", GUILayout.Width(80));
                    GUILayout.FlexibleSpace();
                    GUI.enabled = _mr;
                    if (GUILayout.Button(GetWord("Get"), GUILayout.Width(40)))
                    {
                        if (_currentStepObj.InitialMode == ControlMode.FreeControl)
                        {
                            _currentStepObj.BestView = new Vector3(_mr.X, _mr.Y, _mr.Distance);
                        }
                        else
                        {
                            if (GetBestViewHandler != null)
                            {
                                _currentStepObj.BestView = GetBestViewHandler(_currentStepObj.InitialMode);
                            }
                        }
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    _currentStepObj.BestView = EditorGUILayout.Vector3Field("", _currentStepObj.BestView, GUILayout.Width(185));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("View Offset") + ":", GUILayout.Width(80));
                    GUILayout.FlexibleSpace();
                    GUI.enabled = _ct && _currentStepObj.Target;
                    if (GUILayout.Button(GetWord("Get"), GUILayout.Width(40)))
                    {
                        _currentStepObj.ViewOffset = _ct.transform.position - _currentStepObj.Target.transform.position;
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    _currentStepObj.ViewOffset = EditorGUILayout.Vector3Field("", _currentStepObj.ViewOffset, GUILayout.Width(185));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUI.enabled = _currentStepObj.InitialMode != ControlMode.FreeControl;
                    GUILayout.Label(GetWord("Best Pos") + ":", GUILayout.Width(80));
                    GUILayout.FlexibleSpace();
                    GUI.enabled = _currentStepObj.InitialMode != ControlMode.FreeControl && _player && _currentStepObj.Target;
                    if (GUILayout.Button(GetWord("Get"), GUILayout.Width(40)))
                    {
                        _currentStepObj.BestPos = _player.transform.position;
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUI.enabled = _currentStepObj.InitialMode != ControlMode.FreeControl;
                    _currentStepObj.BestPos = EditorGUILayout.Vector3Field("", _currentStepObj.BestPos, GUILayout.Width(185));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Helper") + ":", GUILayout.Width(55));
                    _stepHelperGC.text = _currentStepObj.Helper == "<None>" ? GetWord(_currentStepObj.Helper) : _currentStepObj.Helper;
                    _stepHelperGC.tooltip = _currentStepObj.HelperName;
                    if (GUILayout.Button(_stepHelperGC, EditorStyles.popup, GUILayout.Width(105)))
                    {
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(_baseType) && !type.IsAbstract;
                        });
                        GenericMenu gm = new GenericMenu();
                        StringToolkit.BeginNoRepeatNaming();
                        gm.AddItem(new GUIContent(GetWord("<None>")), _currentStepObj.Helper == "<None>", () =>
                        {
                            _currentStepObj.Helper = "<None>";
                        });
                        gm.AddItem(new GUIContent(GetWord("<New Helper Script>")), false, () =>
                        {
                            NewHelperScript();
                        });
                        gm.AddSeparator("");
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
                    if (GUILayout.Button(_editGC, "InvisibleButton", GUILayout.Width(20)))
                    {
                        OpenHelperScript(_currentStepObj.Helper);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Parameter") + ":", GUILayout.Width(70));
                    if (GUILayout.Button($"{GetWord("Edit Parameter")} {_currentStepObj.Parameters.Count}"))
                    {
                        StepParameterWindow.ShowWindow(this, _contentAsset, _currentStepObj, CurrentLanguage);
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

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
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(210), GUILayout.Height(130));

                    GUILayout.BeginHorizontal("TE NodeBoxSelected");
                    GUILayout.Label(GetWord("Camera Control"), EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginVertical("Tooltip");
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("CT:", GUILayout.Width(30));
                    GUI.color = _ct ? Color.white : Color.gray;
                    _ct = EditorGUILayout.ObjectField(_ct, typeof(CameraTarget), true, GUILayout.Width(155)) as CameraTarget;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("MR:", GUILayout.Width(30));
                    GUI.color = _mr ? Color.white : Color.gray;
                    _mr = EditorGUILayout.ObjectField(_mr, typeof(MouseRotation), true, GUILayout.Width(155)) as MouseRotation;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("MP:", GUILayout.Width(30));
                    GUI.color = _mp ? Color.white : Color.gray;
                    _mp = EditorGUILayout.ObjectField(_mp, typeof(MousePosition), true, GUILayout.Width(155)) as MousePosition;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("P:", GUILayout.Width(30));
                    GUI.color = _player ? Color.white : Color.gray;
                    _player = EditorGUILayout.ObjectField(_player, typeof(Transform), true, GUILayout.Width(155)) as Transform;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(GetWord("ReFind")))
                    {
                        _ct = FindObjectOfType<CameraTarget>();
                        _mp = FindObjectOfType<MousePosition>();
                        _mr = FindObjectOfType<MouseRotation>();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    GUILayout.EndVertical();
                }
                #endregion

                GUILayout.FlexibleSpace();

                #region 步骤操作的属性
                if (_isShowStepOperation && _currentOperationIndex != -1)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(210), GUILayout.Height(340));

                    GUILayout.BeginHorizontal("TE NodeBoxSelected");
                    GUILayout.Label(GetWord("Step Operation Properties"), EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);

                    GUILayout.BeginVertical("Tooltip");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Name") + ":", GUILayout.Width(50));
                    _currentOperationObj.Name = EditorGUILayout.TextField(_currentOperationObj.Name, GUILayout.Width(135));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("GUID") + ":", GUILayout.Width(50));
                    GUI.enabled = !_isLockID;
                    _currentOperationObj.GUID = EditorGUILayout.TextField(_currentOperationObj.GUID, GUILayout.Width(135));
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUI.enabled = !_currentOperationObj.Instant;
                    GUILayout.Label(GetWord("Elapse Time") + ":", GUILayout.Width(80));
                    _currentOperationObj.ElapseTime = EditorGUILayout.FloatField(_currentOperationObj.ElapseTime, GUILayout.Width(35));
                    GUI.enabled = true;
                    _currentOperationObj.Instant = GUILayout.Toggle(_currentOperationObj.Instant, GetWord("Instant"), GUILayout.Width(65));
                    GUILayout.EndHorizontal();

                    SearchStepTarget(_currentOperationObj);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Target") + ":", GUILayout.Width(50));
                    GUI.color = _currentOperationObj.TargetGUID != "<None>" ? Color.white : Color.gray;
                    GameObject operationObj = EditorGUILayout.ObjectField(_currentOperationObj.Target, typeof(GameObject), true, GUILayout.Width(135)) as GameObject;
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("GUID") + ":", "Label", GUILayout.Width(45)))
                    {
                        GenericMenu gm = new GenericMenu();
                        if (_currentOperationObj.TargetGUID == "<None>")
                        {
                            gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(GetWord("Copy")), false, () =>
                            {
                                GUIUtility.systemCopyBuffer = _currentOperationObj.TargetGUID;
                            });
                        }
                        if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                        {
                            gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(GetWord("Paste")), false, () =>
                            {
                                _currentOperationObj.TargetGUID = GUIUtility.systemCopyBuffer;
                            });
                        }
                        gm.ShowAsContext();
                    }
                    GUILayout.Label(_currentOperationObj.TargetGUID == "<None>" ? GetWord(_currentOperationObj.TargetGUID) : _currentOperationObj.TargetGUID, GUILayout.Width(115));
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(_deleteGC, "InvisibleButton", GUILayout.Width(20)))
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
                                HasChanged(operationObj);
                            }
                            if (target.GUID == "<None>")
                            {
                                target.GUID = Guid.NewGuid().ToString();
                                HasChanged(target);
                            }
                            _currentOperationObj.Target = operationObj;
                            _currentOperationObj.TargetGUID = target.GUID;
                            _currentOperationObj.TargetPath = operationObj.transform.FullName();
                        }
                    }
                    #endregion

                    GUILayout.BeginVertical("Tooltip");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(GetWord("Type") + ":", GUILayout.Width(50));
                    _currentOperationObj.OperationType = (StepOperationType)EditorGUILayout.Popup((int)_currentOperationObj.OperationType, _operationTypes, GUILayout.Width(135));
                    GUILayout.EndHorizontal();

                    _currentOperationObj.OnEditorGUI(_getWord);

                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    GUI.enabled = _currentOperationObj.Target && !_currentOperationObj.PreviewTarget;
                    if (GUILayout.Button(GetWord("Preview"), EditorGlobalTools.Styles.ButtonLeft))
                    {
                        _currentOperationObj.CreatePreviewTarget(_currentStepObj, _currentOperationIndex);
                        SearchStepPreview();
                    }
                    GUI.enabled = _currentOperationObj.PreviewTarget;
                    if (GUILayout.Button(GetWord("Stop"), EditorGlobalTools.Styles.ButtonRight))
                    {
                        _currentOperationObj.DeletePreviewTarget();
                        ClearSearchStepPreview();
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button(GetWord("Clone")))
                    {
                        CloneStepOperation(_currentStepObj, _currentOperationObj);
                    }
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(GetWord("Delete")))
                    {
                        DeleteStepOperation(_currentStepObj, _currentOperationIndex);
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
        private void StepContentMovableGUI()
        {
            if (_isShowStepOperation && _currentStepIndex != -1)
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
                    Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.white, null, 3);

                    if (_isBreakWired)
                    {
                        Vector2 center = (leftAnchor + rightAnchor) / 2;
                        Rect centerRect = new Rect(center.x - 10, center.y - 10, 20, 20);
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

                        Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.yellow, null, 3);
                    }
                    else
                    {
                        Vector2 leftAnchor = _currentStepObj.Operations[_wiredOriginIndex].Anchor - offsetHalf;
                        Vector2 rightAnchor = Event.current.mousePosition;
                        Vector2 leftTangent = _currentStepObj.Operations[_wiredOriginIndex].Anchor - offset;
                        Vector2 rightTangent = Event.current.mousePosition;

                        Handles.DrawBezier(leftAnchor, rightAnchor, leftTangent, rightTangent, Color.yellow, null, 3);
                    }
                }
                #endregion

                #region 步骤的所有操作
                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                {
                    StepOperation operation = _currentStepObj.Operations[i];
                    GUI.color = operation.TargetGUID != "<None>" ? Color.white : Color.gray;
                    GUIStyle style = _currentOperationIndex == i ? "flow node 0 on" : "flow node 0";
                    style.richText = true;
                    string showName = string.Format("[<color=cyan>{0}</color>] {1}\r\n<color=yellow>{2}</color>"
                        , GetWord(operation.OperationType.ToString())
                        , operation.Name
                        , operation.Instant ? GetWord("Instant") : (operation.ElapseTime.ToString() + "s"));
                    Rect leftRect = operation.LeftPosition;
                    Rect rightRect = operation.RightPosition;
                    Rect operationRect = operation.Position;
                    GUI.Box(leftRect, "", style);
                    GUI.Box(rightRect, "", style);
                    GUI.Box(operationRect, showName, style);
                    if (operation.PreviewTarget)
                    {
                        leftRect.Set(leftRect.x + 20, leftRect.y + 10, 20, 20);
                        GUI.Label(leftRect, _previewGC);
                    }
                    EditorGUIUtility.AddCursorRect(leftRect, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(rightRect, MouseCursor.ArrowPlus);
                    EditorGUIUtility.AddCursorRect(operationRect, MouseCursor.MoveArrow);
                }
                GUI.color = Color.white;
                #endregion

                #region Enter
                Rect enterRect = _currentStepObj.EnterPosition;
                string enterName = string.Format("{0}\r\n{1}s", GetWord("Enter"), _currentStepObj.Totaltime.ToString());
                GUI.Box(enterRect, enterName, "flow node 3");
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
                                StepListRightMenu();
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
                                gm.AddItem(new GUIContent(GetWord("Compute total time")), false, () =>
                                {
                                    ComputeTotalTime(_currentStepObj);
                                });
                                gm.AddSeparator("");
                                StringToolkit.BeginNoRepeatNaming();
                                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                                {
                                    int j = i;
                                    gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName($"{GetWord("Connect or break")}/{_currentStepObj.Operations[j].Name}")), _currentStepObj.IsExistWired(-1, j), () =>
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
                                _stepOperationDragging = true;
                                GUI.changed = true;
                            }
                            else if (Event.current.button == 1)
                            {
                                GenericMenu gm = new GenericMenu();
                                CopyStepOperation(gm);
                                gm.AddSeparator("");
                                StringToolkit.BeginNoRepeatNaming();
                                for (int i = 0; i < _currentStepObj.Operations.Count; i++)
                                {
                                    if (i != downIndex)
                                    {
                                        int j = i;
                                        gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName($"{GetWord("Connect or break")}/{_currentStepObj.Operations[j].Name}")), _currentStepObj.IsExistWired(downIndex, j), () =>
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
                        else if (_stepContentAreaRect.Contains(Event.current.mousePosition) && _currentStepIndex != -1)
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
                                if (_currentStepIndex != -1)
                                {
                                    if (_currentOperationIndex != -1)
                                    {
                                        DeleteStepOperation(_currentStepObj, _currentOperationIndex);
                                    }
                                    else
                                    {
                                        DeleteStepContent(_currentStepIndex);
                                    }
                                    GUI.changed = true;
                                }
                                break;
                            case KeyCode.DownArrow:
                                if (_currentStepIndex == -1)
                                {
                                    if (0 < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(0);
                                        SelectStepOperation(-1);
                                        SetStepListScroll(0);
                                        GUI.changed = true;
                                    }
                                }
                                else
                                {
                                    int stepIndex = _currentStepIndex + 1;
                                    if (stepIndex < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(stepIndex);
                                        SelectStepOperation(-1);
                                        SetStepListScroll((float)stepIndex / _contentAsset.Content.Count);
                                        GUI.changed = true;
                                    }
                                }
                                break;
                            case KeyCode.UpArrow:
                                if (_currentStepIndex == -1)
                                {
                                    if (0 < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(_contentAsset.Content.Count - 1);
                                        SelectStepOperation(-1);
                                        SetStepListScroll(1);
                                        GUI.changed = true;
                                    }
                                }
                                else
                                {
                                    int stepIndex = _currentStepIndex - 1;
                                    if (stepIndex >= 0 && stepIndex < _contentAsset.Content.Count)
                                    {
                                        SelectStepContent(stepIndex);
                                        SelectStepOperation(-1);
                                        SetStepListScroll((float)stepIndex / _contentAsset.Content.Count);
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
            if (_currentStepIndex != -1)
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
            if (_currentStepIndex != -1)
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
            if (_currentStepIndex != -1)
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
            //查找是否存在此连线
            StepWired wiredOld = content.Wireds.Find((w) => { return w.Left == left && w.Right == right; });
            //不存在则连接
            if (wiredOld == null)
            {
                //是否有其他节点连入右侧节点，有则移除（一个节点的左侧只允许接入一条线，逻辑只能分流，不能合并）
                StepWired wiredOther = content.Wireds.Find((w) => { return w.Right == right; });
                if (wiredOther != null)
                {
                    content.Wireds.Remove(wiredOther);
                }

                StepWired wired = new StepWired();
                wired.Left = left;
                wired.Right = right;
                content.Wireds.Add(wired);
            }
            //存在则断开
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
            content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
            _contentAsset.StepIDSign += 1;
            content.EnterAnchor = new Vector2(position.width * 0.5f, position.height * 0.5f);
            _contentAsset.Content.Add(content);
            AddStepContentHandler?.Invoke(content);
            SelectStepContent(_contentAsset.Content.Count - 1);
            SelectStepOperation(-1);
            SetStepListScroll(1);
        }
        /// <summary>
        /// 上移当前选中的步骤内容
        /// </summary>
        private void MoveUpStepContent()
        {
            if (_currentStepIndex > 0)
            {
                _contentAsset.Content.RemoveAt(_currentStepIndex);
                _currentStepIndex -= 1;
                _contentAsset.Content.Insert(_currentStepIndex, _currentStepObj);
                SetStepListScroll((float)_currentStepIndex / _contentAsset.Content.Count);
            }
        }
        /// <summary>
        /// 下移当前选中的步骤内容
        /// </summary>
        private void MoveDownStepContent()
        {
            if (_currentStepIndex < _contentAsset.Content.Count - 1)
            {
                _contentAsset.Content.RemoveAt(_currentStepIndex);
                _currentStepIndex += 1;
                _contentAsset.Content.Insert(_currentStepIndex, _currentStepObj);
                SetStepListScroll((float)_currentStepIndex / _contentAsset.Content.Count);
            }
        }
        /// <summary>
        /// 克隆当前选中的步骤内容
        /// </summary>
        private void CloneStepContent()
        {
            StepContent content = _currentStepObj.Clone();
            content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
            _contentAsset.StepIDSign += 1;
            _contentAsset.Content.Add(content);
            SelectStepContent(_contentAsset.Content.Count - 1);
            SelectStepOperation(-1);
            SetStepListScroll(1);
        }
        /// <summary>
        /// 删除步骤内容
        /// </summary>
        private void DeleteStepContent(int contentIndex)
        {
            if (EditorUtility.DisplayDialog("Prompt", $"Are you sure delete step {_contentAsset.Content[contentIndex].Name}？", "Yes", "No"))
            {
                StopPreviewInStep(contentIndex);
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
            PasteStepOperation(gm, position);
            gm.AddSeparator("");
            foreach (StepOperationType type in Enum.GetValues(typeof(StepOperationType)))
            {
                gm.AddItem(new GUIContent($"{GetWord("Add Step Operation")}/{GetWord(type.ToString())}"), false, () =>
                {
                    StepOperation operation = new StepOperation();
                    operation.GUID = Guid.NewGuid().ToString();
                    operation.OperationType = type;
                    operation.Anchor = position - new Vector2(340, 0);
                    switch (type)
                    {
                        case StepOperationType.Active:
                        case StepOperationType.Action:
                        case StepOperationType.ActionArgs:
                        case StepOperationType.FSM:
                        case StepOperationType.TextMesh:
                        case StepOperationType.Prompt:
                        case StepOperationType.CameraFollow:
                        case StepOperationType.ActiveComponent:
                        case StepOperationType.ChangeParent:
                            operation.Instant = true;
                            break;
                        default:
                            operation.Instant = false;
                            break;
                    }
                    operation.Name = type.GetRemark() + content.GetOperationsCout(type);
                    content.Operations.Add(operation);
                    GUI.changed = true;
                });
            }
            gm.ShowAsContext();
        }
        /// <summary>
        /// 克隆步骤操作
        /// </summary>
        private void CloneStepOperation(StepContent content, StepOperation operation)
        {
            StepOperation operationClone = operation.Clone();
            operationClone.Anchor = operation.Anchor + new Vector2(StepOperation.Width + 20, 0);
            operationClone.Name += content.Operations.Count.ToString();
            content.Operations.Add(operationClone);
            SelectStepOperation(content.Operations.Count - 1);
            GUI.FocusControl(null);
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

            content.Operations[operationIndex].DeletePreviewTarget();
            content.Operations.RemoveAt(operationIndex);
            SelectStepOperation(-1);
            GUI.FocusControl(null);
        }
        /// <summary>
        /// 选中步骤内容
        /// </summary>
        private void SelectStepContent(int currentStep)
        {
            _currentStepIndex = currentStep;
            _currentStepObj = (_currentStepIndex != -1 ? _contentAsset.Content[_currentStepIndex] : null);
            if (_currentStepObj != null)
            {
                _currentStepObj.FocusTarget();
                _currentStepObj.RefreshHelperName();
            }
        }
        /// <summary>
        /// 选中步骤操作
        /// </summary>
        private void SelectStepOperation(int currentOperation)
        {
            _currentOperationIndex = currentOperation;
            _currentOperationObj = ((_currentOperationIndex != -1 && _currentStepIndex != -1) ? _currentStepObj.Operations[_currentOperationIndex] : null);
            if (_currentOperationObj != null) _currentOperationObj.FocusTarget();
        }
        /// <summary>
        /// 查找步骤操作
        /// </summary>
        private void FindStepOperation(Vector2 operationAnchor)
        {
            Vector2 direction = new Vector2((position.width - _stepListGUIWidth) * 0.5f, position.height * 0.5f) - operationAnchor;
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
                    showName = $"{content.GUID} {content.Name}";
                    break;
                default:
                    showName = "<None>";
                    break;
            }
            if (_isShowTrigger)
            {
                showName = $"{showName} [{GetWord(content.Trigger.ToString())}]";
            }
            if (_isShowHelper && content.Helper != "<None>")
            {
                showName = $"{showName} [{content.Helper}]";
            }
            return showName;
        }
        /// <summary>
        /// 步骤列表右键功能菜单
        /// </summary>
        private void StepListRightMenu()
        {
            GenericMenu gm = new GenericMenu();
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);

            if (_currentStepIndex == -1)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Copy")} {_currentStepObj.Name}"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = $"StepContent|{assetPath}|{_currentStepObj.GUID}";
                });
            }

            StepContent stepContent = null;
            string[] buffers = GUIUtility.systemCopyBuffer.Split('|');
            if (buffers.Length == 3 && buffers[0] == "StepContent")
            {
                if (buffers[1] == assetPath)
                {
                    stepContent = _contentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                }
                else
                {
                    StepContentAsset stepContentAsset = AssetDatabase.LoadAssetAtPath<StepContentAsset>(buffers[1]);
                    if (stepContentAsset)
                    {
                        stepContent = stepContentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                    }
                }
            }

            if (stepContent == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Paste")} {stepContent.Name}"), false, () =>
                {
                    StepContent content = stepContent.Clone();
                    content.GUID = _contentAsset.StepIDName + _contentAsset.StepIDSign.ToString();
                    _contentAsset.StepIDSign += 1;
                    _contentAsset.Content.Add(content);
                    SelectStepContent(_contentAsset.Content.Count - 1);
                    SelectStepOperation(-1);
                    SetStepListScroll(1);
                });
            }

            gm.AddSeparator("");

            gm.AddItem(new GUIContent(GetWord("Enable All Steps")), false, () =>
            {
                for (int i = 0; i < _contentAsset.Content.Count; i++)
                {
                    _contentAsset.Content[i].IsEnable = true;
                }
                HasChanged(_contentAsset);
            });
            gm.AddItem(new GUIContent(GetWord("Disable All Steps")), false, () =>
            {
                for (int i = 0; i < _contentAsset.Content.Count; i++)
                {
                    _contentAsset.Content[i].IsEnable = false;
                }
                HasChanged(_contentAsset);
            });

            gm.ShowAsContext();
        }
        /// <summary>
        /// 复制步骤操作
        /// </summary>
        private void CopyStepOperation(GenericMenu gm)
        {
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);

            if (_currentOperationIndex == -1)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Copy")} {_currentOperationObj.Name}"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = $"StepOperation|{assetPath}|{_currentStepObj.GUID}|{_currentOperationObj.GUID}";
                });
            }
        }
        /// <summary>
        /// 粘贴步骤操作
        /// </summary>
        private void PasteStepOperation(GenericMenu gm, Vector2 position)
        {
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);

            StepContent content = null;
            StepOperation operation = null;
            string[] buffers = GUIUtility.systemCopyBuffer.Split('|');
            if (buffers.Length == 4 && buffers[0] == "StepOperation")
            {
                if (buffers[1] == assetPath)
                {
                    content = _contentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                    if (content != null)
                    {
                        operation = content.Operations.Find((s) => { return s.GUID == buffers[3]; });
                    }
                }
                else
                {
                    StepContentAsset stepContentAsset = AssetDatabase.LoadAssetAtPath<StepContentAsset>(buffers[1]);
                    if (stepContentAsset)
                    {
                        content = stepContentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                        if (content != null)
                        {
                            operation = content.Operations.Find((s) => { return s.GUID == buffers[3]; });
                        }
                    }
                }
            }

            if (operation == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Paste")} {operation.Name}"), false, () =>
                {
                    StepOperation ope = operation.Clone();
                    ope.Anchor = position - new Vector2(340, 0);
                    _currentStepObj.Operations.Add(ope);
                    SelectStepOperation(_currentStepObj.Operations.Count - 1);
                });
            }
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
        /// 停止指定步骤的所有操作预览
        /// </summary>
        private void StopPreviewInStep(int stepIndex)
        {
            StepContent content = stepIndex != -1 ? _contentAsset.Content[stepIndex] : null;
            if (content != null)
            {
                for (int i = 0; i < content.Operations.Count; i++)
                {
                    content.Operations[i].DeletePreviewTarget();
                }
            }
            ClearSearchStepPreview();
        }
        /// <summary>
        /// 停止所有步骤的所有操作预览
        /// </summary>
        private void StopPreviewInAllStep()
        {
            for (int i = 0; i < _contentAsset.Content.Count; i++)
            {
                StepContent content = _contentAsset.Content[i];
                for (int j = 0; j < content.Operations.Count; j++)
                {
                    content.Operations[j].DeletePreviewTarget();
                }
            }
            ClearSearchStepPreview();
        }
        /// <summary>
        /// 筛选步骤预览目标
        /// </summary>
        private void SearchStepPreview()
        {
            Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
            PropertyInfo propertyInfo = type.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Static | BindingFlags.Public);
            EditorWindow window = propertyInfo.GetValue(null) as EditorWindow;
            MethodInfo methodInfo = type.GetMethod("SetSearchFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            object[] args = new object[] { "StepPreview", SearchableEditorWindow.SearchMode.Type, true, true };
            methodInfo.Invoke(window, args);
        }
        /// <summary>
        /// 清除筛选步骤预览目标
        /// </summary>
        private void ClearSearchStepPreview()
        {
            Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
            PropertyInfo propertyInfo = type.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Static | BindingFlags.Public);
            EditorWindow window = propertyInfo.GetValue(null) as EditorWindow;
            MethodInfo methodInfo = type.GetMethod("ClearSearchFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(window, null);
        }

        /// <summary>
        /// 打开助手脚本
        /// </summary>
        public void OpenHelperScript(string helper)
        {
            CSharpScriptToolkit.OpenScript(helper);
        }
        /// <summary>
        /// 新建助手脚本
        /// </summary>
        private void NewHelperScript()
        {
            _currentStepObj.Helper = EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_StepHelper_Folder, "StepHelper", "StepHelperTemplate", NewHelperScriptHandler, "#HELPERNAME#");
        }

        /// <summary>
        /// 计算Enter节点开始执行的所有操作的总时间
        /// </summary>
        private void ComputeTotalTime(StepContent content)
        {
            content.GetExecuteTwice(_operationIndexs);
            foreach (var item in _operationIndexs)
            {
                Log.Warning($"注意：操作节点【{content.Operations[item].Name}】有两次或以上连线接入，可能会被多次执行！");
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
        /// 设置步骤内容列表的滚动值
        /// </summary>
        private void SetStepListScroll(float scroll)
        {
            _stepListScroll.Set(0, scroll * (_contentAsset.Content.Count * 20));
        }
        /// <summary>
        /// 在场景中搜索步骤目标
        /// </summary>
        private void SearchStepTarget(StepContent content)
        {
            if (content.TargetGUID == "<None>")
                return;

            if (content.Target != null)
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                content.Target = prefabStage.prefabContentsRoot.FindChildren(content.TargetPath);
                if (content.Target == null)
                {
                    StepTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<StepTarget>(true);
                    foreach (StepTarget target in targets)
                    {
                        if (target.GUID == content.TargetGUID && !target.GetComponent<StepPreview>())
                        {
                            content.Target = target.gameObject;
                            content.TargetPath = target.transform.FullName();
                            content.TargetPath = content.TargetPath.Substring(content.TargetPath.IndexOf("/") + 1);
                            break;
                        }
                    }
                }
            }
            else
            {
                content.Target = GameObject.Find(content.TargetPath);
                if (content.Target == null)
                {
                    StepTarget[] targets = FindObjectsOfType<StepTarget>(true);
                    foreach (StepTarget target in targets)
                    {
                        if (target.GUID == content.TargetGUID && !target.GetComponent<StepPreview>())
                        {
                            content.Target = target.gameObject;
                            content.TargetPath = target.transform.FullName();
                            break;
                        }
                    }
                }
            }

            if (content.Target != null)
            {
                StepTarget target = content.Target.GetComponent<StepTarget>();
                if (!target)
                {
                    target = content.Target.AddComponent<StepTarget>();
                    target.GUID = content.TargetGUID;
                    HasChanged(content.Target);
                }
            }
        }
        /// <summary>
        /// 在场景中搜索步骤（操作）目标
        /// </summary>
        private void SearchStepTarget(StepOperation operation)
        {
            if (operation.TargetGUID == "<None>")
                return;

            if (operation.Target != null)
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                operation.Target = prefabStage.prefabContentsRoot.FindChildren(operation.TargetPath);
                if (operation.Target == null)
                {
                    StepTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<StepTarget>(true);
                    foreach (StepTarget target in targets)
                    {
                        if (target.GUID == operation.TargetGUID && !target.GetComponent<StepPreview>())
                        {
                            operation.Target = target.gameObject;
                            operation.TargetPath = target.transform.FullName();
                            operation.TargetPath = operation.TargetPath.Substring(operation.TargetPath.IndexOf("/") + 1);
                            break;
                        }
                    }
                }
            }
            else
            {
                operation.Target = GameObject.Find(operation.TargetPath);
                if (operation.Target == null)
                {
                    StepTarget[] targets = FindObjectsOfType<StepTarget>(true);
                    foreach (StepTarget target in targets)
                    {
                        if (target.GUID == operation.TargetGUID && !target.GetComponent<StepPreview>())
                        {
                            operation.Target = target.gameObject;
                            operation.TargetPath = target.transform.FullName();
                            break;
                        }
                    }
                }
            }

            if (operation.Target != null)
            {
                StepTarget target = operation.Target.GetComponent<StepTarget>();
                if (!target)
                {
                    target = operation.Target.AddComponent<StepTarget>();
                    target.GUID = operation.TargetGUID;
                    HasChanged(operation.Target);
                }
            }
        }
        /// <summary>
        /// 高级筛查方法改变
        /// </summary>
        private void AdvancedSearchChange(string searchName)
        {
            SelectStepContent(-1);
            SelectStepOperation(-1);

            if (_stepListAdvancedSearch == searchName)
                return;
           
            _stepListAdvancedSearch = searchName;
            if (!string.IsNullOrEmpty(_stepListAdvancedSearch))
            {
                for (int i = 0; i < _contentAsset.Content.Count; i++)
                {
                    StepContent stepContent = _contentAsset.Content[i];
                    stepContent.IsSearched = AdvancedSearchHandlers[_stepListAdvancedSearch].Invoke(stepContent);
                }
            }
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