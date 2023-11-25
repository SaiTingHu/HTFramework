using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UObject = UnityEngine.Object;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("HTFramework.Editor")]
[assembly: InternalsVisibleTo("HTFramework.AI.Editor")]
[assembly: InternalsVisibleTo("HTFramework.AI.RunTime")]
[assembly: InternalsVisibleTo("HTFramework.ILHotfix.Editor")]
[assembly: InternalsVisibleTo("HTFramework.ILHotfix.RunTime")]
[assembly: InternalsVisibleTo("HTFramework.GC.Editor")]
[assembly: InternalsVisibleTo("HTFramework.GC.RunTime")]

namespace HT.Framework
{
    /// <summary>
    /// HTFramework 主模块
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    [InternalModule(HTFrameworkModule.Main)]
    public sealed partial class Main : InternalModuleBase<IMainHelper>
    {
        /// <summary>
        /// 当前主程序
        /// </summary>
        public static Main Current { get; private set; }

        #region Static Method
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original) where T : UObject
        {
            return Instantiate(original);
        }
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的旋转</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation) where T : UObject
        {
            return Instantiate(original, position, rotation);
        }
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的旋转</param>
        /// <param name="parent">新对象的父物体</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : UObject
        {
            return Instantiate(original, position, rotation, parent);
        }
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="parent">新对象的父物体</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Transform parent) where T : UObject
        {
            return Instantiate(original, parent);
        }
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="parent">新对象的父物体</param>
        /// <param name="worldPositionStays">是否保持世界位置不变</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Transform parent, bool worldPositionStays) where T : UObject
        {
            return Instantiate(original, parent, worldPositionStays);
        }
        /// <summary>
        /// 克隆 GameObject 实例
        /// </summary>
        /// <param name="original">初始对象</param>
        /// <param name="isUI">是否是UI对象</param>
        /// <returns>克隆的新对象</returns>
        public static GameObject CloneGameObject(GameObject original, bool isUI = false)
        {
            GameObject obj = Instantiate(original);
            obj.transform.SetParent(original.transform.parent);
            if (isUI)
            {
                RectTransform rect = obj.rectTransform();
                RectTransform originalRect = original.rectTransform();
                rect.anchoredPosition3D = originalRect.anchoredPosition3D;
                rect.sizeDelta = originalRect.sizeDelta;
                rect.offsetMin = originalRect.offsetMin;
                rect.offsetMax = originalRect.offsetMax;
                rect.anchorMin = originalRect.anchorMin;
                rect.anchorMax = originalRect.anchorMax;
                rect.pivot = originalRect.pivot;
            }
            else
            {
                obj.transform.localPosition = original.transform.localPosition;
            }
            obj.transform.localRotation = original.transform.localRotation;
            obj.transform.localScale = original.transform.localScale;
            obj.SetActive(true);
            return obj;
        }
        /// <summary>
        /// 杀死实例
        /// </summary>
        /// <param name="obj">实例对象</param>
        public static void Kill(UObject obj)
        {
            Destroy(obj);
        }
        /// <summary>
        /// 立即杀死实例
        /// </summary>
        /// <param name="obj">实例对象</param>
        public static void KillImmediate(UObject obj)
        {
            DestroyImmediate(obj);
        }
        /// <summary>
        /// 杀死一群实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">实例集合</param>
        public static void Kills<T>(List<T> objs) where T : UObject
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Destroy(objs[i]);
            }
            objs.Clear();
        }
        /// <summary>
        /// 杀死一群实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">实例数组</param>
        public static void Kills<T>(T[] objs) where T : UObject
        {
            for (int i = 0; i < objs.Length; i++)
            {
                Destroy(objs[i]);
            }
        }
        #endregion

        #region Lifecycle
        protected override void Awake()
        {
            base.Awake();

            useGUILayout = true;

            if (Current == null)
            {
                Current = this;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Main, "框架致命错误：不能存在两个及以上Main主模块！");
            }

            OnInit();
        }
        private void Start()
        {
            OnReady();
        }
        private void Update()
        {
            OnUpdate();
        }
        private void OnGUI()
        {
            BehaviourDrawGUI();
            LicenseOnGUI();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnTerminate();
        }
        private void OnApplicationFocus(bool focus)
        {
            ApplicationFocusEvent?.Invoke(focus);
        }
        private void OnApplicationQuit()
        {
            ApplicationQuitEvent?.Invoke();
        }
        #endregion

        #region Module
        /// <summary>
        /// 切面代理模块
        /// </summary>
        public static AspectTrackManager m_AspectTrack { get; private set; }
        /// <summary>
        /// 音频模块
        /// </summary>
        public static AudioManager m_Audio { get; private set; }
        /// <summary>
        /// 操作控制模块
        /// </summary>
        public static ControllerManager m_Controller { get; private set; }
        /// <summary>
        /// 协程调度模块
        /// </summary>
        public static CoroutinerManager m_Coroutiner { get; private set; }
        /// <summary>
        /// 自定义模块
        /// </summary>
        public static CustomModuleManager m_CustomModule { get; private set; }
        /// <summary>
        /// 数据集模块
        /// </summary>
        public static DataSetManager m_DataSet { get; private set; }
        /// <summary>
        /// 调试模块
        /// </summary>
        public static DebugManager m_Debug { get; private set; }
        /// <summary>
        /// ECS模块
        /// </summary>
        public static ECSManager m_ECS { get; private set; }
        /// <summary>
        /// 实体模块
        /// </summary>
        public static EntityManager m_Entity { get; private set; }
        /// <summary>
        /// 事件模块
        /// </summary>
        public static EventManager m_Event { get; private set; }
        /// <summary>
        /// 异常处理模块
        /// </summary>
        public static ExceptionManager m_Exception { get; private set; }
        /// <summary>
        /// 有限状态机模块
        /// </summary>
        public static FSMManager m_FSM { get; private set; }
        /// <summary>
        /// 热更新模块
        /// </summary>
        public static HotfixManager m_Hotfix { get; private set; }
        /// <summary>
        /// 输入模块
        /// </summary>
        public static InputManager m_Input { get; private set; }
        /// <summary>
        /// 指令模块
        /// </summary>
        public static InstructionManager m_Instruction { get; private set; }
        /// <summary>
        /// 主模块
        /// </summary>
        public static Main m_Main { get; private set; }
        /// <summary>
        /// 网络模块
        /// </summary>
        public static NetworkManager m_Network { get; private set; }
        /// <summary>
        /// 对象池模块
        /// </summary>
        public static ObjectPoolManager m_ObjectPool { get; private set; }
        /// <summary>
        /// 流程模块
        /// </summary>
        public static ProcedureManager m_Procedure { get; private set; }
        /// <summary>
        /// 引用池模块
        /// </summary>
        public static ReferencePoolManager m_ReferencePool { get; private set; }
        /// <summary>
        /// 资源模块
        /// </summary>
        public static ResourceManager m_Resource { get; private set; }
        /// <summary>
        /// 步骤模块
        /// </summary>
        public static StepMaster m_StepMaster { get; private set; }
        /// <summary>
        /// 任务模块
        /// </summary>
        public static TaskMaster m_TaskMaster { get; private set; }
        /// <summary>
        /// UI模块
        /// </summary>
        public static UIManager m_UI { get; private set; }
        /// <summary>
        /// Web请求模块
        /// </summary>
        public static WebRequestManager m_WebRequest { get; private set; }

        private Dictionary<HTFrameworkModule, IModuleManager> _internalModules = new Dictionary<HTFrameworkModule, IModuleManager>();
        private List<IModuleManager> _internalModulesList = new List<IModuleManager>();
        private bool _isPause = false;

        /// <summary>
        /// 暂停主程序
        /// </summary>
        public bool Pause
        {
            get
            {
                return _isPause;
            }
            set
            {
                if (_isPause == value)
                    return;

                _isPause = value;
                if (_isPause)
                {
                    OnPause();
                    m_Event.Throw<EventPauseGame>();
                }
                else
                {
                    OnResume();
                    m_Event.Throw<EventResumeGame>();
                }
            }
        }

        private void ModuleInit()
        {
            IModuleManager[] modules = transform.GetComponentsInChildren<IModuleManager>(true);
            for (int i = 0; i < modules.Length; i++)
            {
                InternalModuleAttribute attribute = modules[i].GetType().GetCustomAttribute<InternalModuleAttribute>();
                if (attribute != null)
                {
                    if (!_internalModules.ContainsKey(attribute.ModuleName))
                    {
                        if (attribute.ModuleName != HTFrameworkModule.Main)
                        {
                            _internalModules.Add(attribute.ModuleName, modules[i]);
                            _internalModulesList.Add(modules[i]);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Main, $"获取内置模块失败：内置模块类 {modules[i].GetType().FullName} 的 InternalModule 标记与已有模块重复！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Main, $"获取内置模块失败：内置模块类 {modules[i].GetType().FullName} 丢失了 InternalModule 标记！");
                }
            }

            m_AspectTrack = GetInternalModule(HTFrameworkModule.AspectTrack) as AspectTrackManager;
            m_Audio = GetInternalModule(HTFrameworkModule.Audio) as AudioManager;
            m_Controller = GetInternalModule(HTFrameworkModule.Controller) as ControllerManager;
            m_Coroutiner = GetInternalModule(HTFrameworkModule.Coroutiner) as CoroutinerManager;
            m_CustomModule = GetInternalModule(HTFrameworkModule.CustomModule) as CustomModuleManager;
            m_DataSet = GetInternalModule(HTFrameworkModule.DataSet) as DataSetManager;
            m_Debug = GetInternalModule(HTFrameworkModule.Debug) as DebugManager;
            m_ECS = GetInternalModule(HTFrameworkModule.ECS) as ECSManager;
            m_Entity = GetInternalModule(HTFrameworkModule.Entity) as EntityManager;
            m_Event = GetInternalModule(HTFrameworkModule.Event) as EventManager;
            m_Exception = GetInternalModule(HTFrameworkModule.Exception) as ExceptionManager;
            m_FSM = GetInternalModule(HTFrameworkModule.FSM) as FSMManager;
            m_Hotfix = GetInternalModule(HTFrameworkModule.Hotfix) as HotfixManager;
            m_Input = GetInternalModule(HTFrameworkModule.Input) as InputManager;
            m_Instruction = GetInternalModule(HTFrameworkModule.Instruction) as InstructionManager;
            m_Main = GetInternalModule(HTFrameworkModule.Main) as Main;
            m_Network = GetInternalModule(HTFrameworkModule.Network) as NetworkManager;
            m_ObjectPool = GetInternalModule(HTFrameworkModule.ObjectPool) as ObjectPoolManager;
            m_Procedure = GetInternalModule(HTFrameworkModule.Procedure) as ProcedureManager;
            m_ReferencePool = GetInternalModule(HTFrameworkModule.ReferencePool) as ReferencePoolManager;
            m_Resource = GetInternalModule(HTFrameworkModule.Resource) as ResourceManager;
            m_StepMaster = GetInternalModule(HTFrameworkModule.StepMaster) as StepMaster;
            m_TaskMaster = GetInternalModule(HTFrameworkModule.TaskMaster) as TaskMaster;
            m_UI = GetInternalModule(HTFrameworkModule.UI) as UIManager;
            m_WebRequest = GetInternalModule(HTFrameworkModule.WebRequest) as WebRequestManager;

            _internalModulesList.Sort((a, b) => { return a.Priority.CompareTo(b.Priority); });

            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnInit();
            }
        }
        private void ModuleReady()
        {
            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnReady();
            }
        }
        private void ModuleUpdate()
        {
            if (Pause)
                return;

            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnUpdate();
            }
        }
        private void ModuleTerminate()
        {
            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnTerminate();
            }
        }
        private void ModulePause()
        {
            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnPause();
            }
        }
        private void ModuleResume()
        {
            for (int i = 0; i < _internalModulesList.Count; i++)
            {
                _internalModulesList[i].OnResume();
            }
        }

        /// <summary>
        /// 获取内置模块
        /// </summary>
        /// <param name="moduleName">内置模块名称</param>
        /// <returns>内置模块对象</returns>
        public IModuleManager GetInternalModule(HTFrameworkModule moduleName)
        {
            if (moduleName == HTFrameworkModule.Main)
            {
                return Current;
            }

            if (_internalModules.ContainsKey(moduleName))
            {
                return _internalModules[moduleName];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Main, $"获取内置模块失败：不存在名为 {moduleName} 的内置模块！");
            }
        }
        #endregion

        #region Behaviour
        private List<HTBehaviour> _drawGUIBehaviours = new List<HTBehaviour>();
        private List<HTBehaviour> _updateFrameBehaviours = new List<HTBehaviour>();
        private List<HTBehaviour> _updateSecondBehaviours = new List<HTBehaviour>();
        private List<IDrawGUI> _drawGUIs = new List<IDrawGUI>();
        private List<IUpdateFrame> _updateFrames = new List<IUpdateFrame>();
        private List<IUpdateSecond> _updateSeconds = new List<IUpdateSecond>();
        private float _timer = 0;

        private void BehaviourDrawGUI()
        {
            if (_drawGUIs.Count > 0)
            {
                for (int i = 0; i < _drawGUIBehaviours.Count && i < _drawGUIs.Count; i++)
                {
                    if (_drawGUIBehaviours[i] != null && _drawGUIBehaviours[i].enabled && _drawGUIBehaviours[i].gameObject.activeSelf && _drawGUIs[i] != null)
                    {
                        _drawGUIs[i].OnDrawGUI();
                    }
                }
            }
        }
        private void BehaviourUpdate()
        {
            if (Pause)
                return;

            if (_updateFrames.Count > 0)
            {
                for (int i = 0; i < _updateFrameBehaviours.Count && i < _updateFrames.Count; i++)
                {
                    if (_updateFrameBehaviours[i] != null && _updateFrameBehaviours[i].enabled && _updateFrameBehaviours[i].gameObject.activeSelf && _updateFrames[i] != null)
                    {
                        _updateFrames[i].OnUpdateFrame();
                    }
                }
            }

            if (_timer >= 1)
            {
                _timer -= 1;
                if (_updateSeconds.Count > 0)
                {
                    for (int i = 0; i < _updateSecondBehaviours.Count && i < _updateSeconds.Count; i++)
                    {
                        if (_updateSecondBehaviours[i] != null && _updateSecondBehaviours[i].enabled && _updateSecondBehaviours[i].gameObject.activeSelf && _updateSeconds[i] != null)
                        {
                            _updateSeconds[i].OnUpdateSecond();
                        }
                    }
                }
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        /// <summary>
        /// 注册行为类
        /// </summary>
        /// <param name="behaviour">行为类对象</param>
        internal void RegisterBehaviour(HTBehaviour behaviour)
        {
            IDrawGUI drawGUI = behaviour as IDrawGUI;
            if (drawGUI != null)
            {
                if (!_drawGUIBehaviours.Contains(behaviour))
                {
                    _drawGUIBehaviours.Add(behaviour);
                    _drawGUIs.Add(drawGUI);
                }
            }
            IUpdateFrame updateFrame = behaviour as IUpdateFrame;
            if (updateFrame != null)
            {
                if (!_updateFrameBehaviours.Contains(behaviour))
                {
                    _updateFrameBehaviours.Add(behaviour);
                    _updateFrames.Add(updateFrame);
                }
            }
            IUpdateSecond updateSecond = behaviour as IUpdateSecond;
            if (updateSecond != null)
            {
                if (!_updateSecondBehaviours.Contains(behaviour))
                {
                    _updateSecondBehaviours.Add(behaviour);
                    _updateSeconds.Add(updateSecond);
                }
            }
        }
        /// <summary>
        /// 注销行为类
        /// </summary>
        /// <param name="behaviour">行为类对象</param>
        internal void UnregisterBehaviour(HTBehaviour behaviour)
        {
            IDrawGUI drawGUI = behaviour as IDrawGUI;
            if (drawGUI != null)
            {
                if (_drawGUIBehaviours.Contains(behaviour))
                {
                    _drawGUIBehaviours.Remove(behaviour);
                    _drawGUIs.Remove(drawGUI);
                }
            }
            IUpdateFrame updateFrame = behaviour as IUpdateFrame;
            if (updateFrame != null)
            {
                if (_updateFrameBehaviours.Contains(behaviour))
                {
                    _updateFrameBehaviours.Remove(behaviour);
                    _updateFrames.Remove(updateFrame);
                }
            }
            IUpdateSecond updateSecond = behaviour as IUpdateSecond;
            if (updateSecond != null)
            {
                if (_updateSecondBehaviours.Contains(behaviour))
                {
                    _updateSecondBehaviours.Remove(behaviour);
                    _updateSeconds.Remove(updateSecond);
                }
            }
        }
        #endregion

        #region License
        /// <summary>
        /// 是否永久授权【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsPermanentLicense = true;
        /// <summary>
        /// 当前授权者类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string LicenserType = "<None>";

        private LicenserBase _licenser;
        private GUIStyle _promptStyle;
        private Rect _promptRect;
        private bool _isLicenseEnd = false;
        private bool _isLicensePass = false;

        private void LicenseInit()
        {
            if (IsPermanentLicense)
            {
                _isLicenseEnd = true;
                _isLicensePass = true;
            }
            else
            {
                if (LicenserType != "<None>")
                {
                    Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(LicenserType);
                    if (type != null)
                    {
                        if (type.IsSubclassOf(typeof(LicenserBase)))
                        {
                            _licenser = Activator.CreateInstance(type) as LicenserBase;
                            _licenser.OnInit();
                        }
                        else
                        {
                            Log.Error($"创建授权者失败：授权者类 {LicenserType} 必须继承至基类：LicenserBase！");
                        }
                    }
                    else
                    {
                        Log.Error($"创建授权者失败：丢失授权者类 {LicenserType}！");
                    }
                }
                else
                {
                    Log.Error("已启用授权验证，但授权者类型不能为 <None>！");
                }

                _promptStyle = new GUIStyle();
                _promptStyle.alignment = TextAnchor.MiddleCenter;
                _promptStyle.normal.textColor = Color.red;
                _promptStyle.fontSize = 30;

                _isLicenseEnd = false;
                _isLicensePass = false;
            }
        }
        private void LicenseReady()
        {
            if (_licenser != null)
            {
                StartCoroutine(LicenseChecking());
            }
        }
        private void LicenseOnGUI()
        {
            if (_isLicenseEnd && !_isLicensePass)
            {
                Paralyze();

                _promptRect.Set(0, 0, Screen.width, Screen.height);
                GUI.Label(_promptRect, _licenser.LicenseFailurePrompt, _promptStyle);
            }
        }

        private IEnumerator LicenseChecking()
        {
            yield return StartCoroutine(_licenser.Checking());

            _isLicenseEnd = true;
            _isLicensePass = _licenser.IsLicensePass;
        }
        private void Paralyze()
        {
            m_Controller.MainCamera.clearFlags = CameraClearFlags.SolidColor;
            m_Controller.MainCamera.cullingMask = 0;
            m_Audio.Mute = true;
            m_UI.IsHideAll = true;
            m_Entity.IsHideAll = true;
            m_Input.IsEnableInputDevice = false;
        }
        #endregion

        #region DataModel
        /// <summary>
        /// 当前环境的所有数据模型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> DataModelTypes = new List<string>();

        private Dictionary<Type, DataModelBase> _dataModels = new Dictionary<Type, DataModelBase>();

        private void DataModelInit()
        {
            for (int i = 0; i < DataModelTypes.Count; i++)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(DataModelTypes[i]);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(DataModelBase)))
                    {
                        DataModelBase dataModel = Activator.CreateInstance(type) as DataModelBase;
                        _dataModels.Add(type, dataModel);
                        dataModel.OnInit();
                    }
                    else
                    {
                        Log.Error($"创建数据模型失败：数据模型类 {DataModelTypes[i]} 必须继承至基类：DataModelBase！");
                    }
                }
                else
                {
                    Log.Error($"创建数据模型失败：丢失数据模型类 {DataModelTypes[i]}！");
                }
            }
        }
        private void DataModelReady()
        {
            foreach (var dataModel in _dataModels)
            {
                dataModel.Value.OnReady();
            }
        }

        /// <summary>
        /// 获取数据模型
        /// </summary>
        /// <typeparam name="T">数据模型类</typeparam>
        /// <returns>数据模型对象</returns>
        public T GetDataModel<T>() where T : DataModelBase
        {
            return GetDataModel(typeof(T)) as T;
        }
        /// <summary>
        /// 获取数据模型
        /// </summary>
        /// <param name="type">数据模型类</param>
        /// <returns>数据模型对象</returns>
        public DataModelBase GetDataModel(Type type)
        {
            if (_dataModels.ContainsKey(type))
            {
                return _dataModels[type];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 清空指定数据模型的数据绑定
        /// </summary>
        /// <typeparam name="T">数据模型类</typeparam>
        public void ClearDataBinding<T>() where T : DataModelBase
        {
            ClearDataBinding(typeof(T));
        }
        /// <summary>
        /// 清空指定数据模型的数据绑定
        /// </summary>
        /// <param name="type">数据模型类</param>
        public void ClearDataBinding(Type type)
        {
            if (_dataModels.ContainsKey(type))
            {
                AutomaticTask.ClearDataBinding(_dataModels[type]);
            }
        }
        /// <summary>
        /// 清空所有数据模型的数据绑定
        /// </summary>
        public void ClearAllDataBinding()
        {
            foreach (var dataModel in _dataModels)
            {
                AutomaticTask.ClearDataBinding(dataModel.Value);
            }
        }
        #endregion

        #region Parameter
        /// <summary>
        /// 主要参数【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<MainParameter> MainParameters = new List<MainParameter>();

        /// <summary>
        /// 是否存在指定名称、类型的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName, MainParameter.ParameterType parameterType)
        {
            MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
            return mainParameter != null;
        }
        /// <summary>
        /// 是否存在指定名称的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName)
        {
            MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName; });
            return mainParameter != null;
        }
        /// <summary>
        /// 通过名称、类型获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <param name="mainParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, MainParameter.ParameterType parameterType, List<MainParameter> mainParameters)
        {
            for (int i = 0; i < MainParameters.Count; i++)
            {
                if (MainParameters[i].Name == parameterName && MainParameters[i].Type == parameterType)
                {
                    mainParameters.Add(MainParameters[i]);
                }
            }
        }
        /// <summary>
        /// 通过名称获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="mainParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, List<MainParameter> mainParameters)
        {
            for (int i = 0; i < MainParameters.Count; i++)
            {
                if (MainParameters[i].Name == parameterName)
                {
                    mainParameters.Add(MainParameters[i]);
                }
            }
        }
        /// <summary>
        /// 通过名称、类型获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>参数</returns>
        public MainParameter GetParameter(string parameterName, MainParameter.ParameterType parameterType)
        {
            MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
            if (mainParameter != null)
            {
                return mainParameter;
            }
            else
            {
                Log.Error($"获取参数失败：当前不存在参数 {parameterName}！");
                return null;
            }
        }
        /// <summary>
        /// 通过名称获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public MainParameter GetParameter(string parameterName)
        {
            MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName; });
            if (mainParameter != null)
            {
                return mainParameter;
            }
            else
            {
                Log.Error($"获取参数失败：当前不存在参数 {parameterName}！");
                return null;
            }
        }
        /// <summary>
        /// 通过名称获取String参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public string GetStringParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.String);
            return (mainParameter != null) ? mainParameter.StringValue : "";
        }
        /// <summary>
        /// 通过名称获取Integer参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public int GetIntegerParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Integer);
            return (mainParameter != null) ? mainParameter.IntegerValue : 0;
        }
        /// <summary>
        /// 通过名称获取Float参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public float GetFloatParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Float);
            return (mainParameter != null) ? mainParameter.FloatValue : 0f;
        }
        /// <summary>
        /// 通过名称获取Boolean参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public bool GetBooleanParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Boolean);
            return (mainParameter != null) ? mainParameter.BooleanValue : false;
        }
        /// <summary>
        /// 通过名称获取Vector2参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector2 GetVector2Parameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Vector2);
            return (mainParameter != null) ? mainParameter.Vector2Value : Vector2.zero;
        }
        /// <summary>
        /// 通过名称获取Vector3参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector3 GetVector3Parameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Vector3);
            return (mainParameter != null) ? mainParameter.Vector3Value : Vector3.zero;
        }
        /// <summary>
        /// 通过名称获取Color参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Color GetColorParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Color);
            return (mainParameter != null) ? mainParameter.ColorValue : Color.white;
        }
        /// <summary>
        /// 通过名称获取DataSet参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public DataSetBase GetDataSetParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.DataSet);
            return (mainParameter != null) ? mainParameter.DataSet : null;
        }
        /// <summary>
        /// 通过名称获取Prefab参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public GameObject GetPrefabParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Prefab);
            return (mainParameter != null) ? mainParameter.PrefabValue : null;
        }
        /// <summary>
        /// 通过名称获取Texture参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Texture GetTextureParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Texture);
            return (mainParameter != null) ? mainParameter.TextureValue : null;
        }
        /// <summary>
        /// 通过名称获取AudioClip参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public AudioClip GetAudioClipParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.AudioClip);
            return (mainParameter != null) ? mainParameter.AudioClipValue : null;
        }
        /// <summary>
        /// 通过名称获取Material参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Material GetMaterialParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Material);
            return (mainParameter != null) ? mainParameter.MaterialValue : null;
        }
        /// <summary>
        /// 通过名称设置String参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetStringParameter(string parameterName, string value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.String);
            if (mainParameter != null)
            {
                mainParameter.StringValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Integer参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetIntegerParameter(string parameterName, int value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Integer);
            if (mainParameter != null)
            {
                mainParameter.IntegerValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Float参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetFloatParameter(string parameterName, float value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Float);
            if (mainParameter != null)
            {
                mainParameter.FloatValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Boolean参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetBooleanParameter(string parameterName, bool value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Boolean);
            if (mainParameter != null)
            {
                mainParameter.BooleanValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Vector2参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetVector2Parameter(string parameterName, Vector2 value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Vector2);
            if (mainParameter != null)
            {
                mainParameter.Vector2Value = value;
            }
        }
        /// <summary>
        /// 通过名称设置Vector3参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetVector3Parameter(string parameterName, Vector3 value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Vector3);
            if (mainParameter != null)
            {
                mainParameter.Vector3Value = value;
            }
        }
        /// <summary>
        /// 通过名称设置Color参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetColorParameter(string parameterName, Color value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Color);
            if (mainParameter != null)
            {
                mainParameter.ColorValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置DataSet参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetDataSetParameter(string parameterName, DataSetBase dataset)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.DataSet);
            if (mainParameter != null)
            {
                mainParameter.DataSet = dataset;
            }
        }
        /// <summary>
        /// 通过名称设置Prefab参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetPrefabParameter(string parameterName, GameObject value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Prefab);
            if (mainParameter != null)
            {
                mainParameter.PrefabValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Texture参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetTextureParameter(string parameterName, Texture value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Texture);
            if (mainParameter != null)
            {
                mainParameter.TextureValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置AudioClip参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetAudioClipParameter(string parameterName, AudioClip value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.AudioClip);
            if (mainParameter != null)
            {
                mainParameter.AudioClipValue = value;
            }
        }
        /// <summary>
        /// 通过名称设置Material参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetMaterialParameter(string parameterName, Material value)
        {
            MainParameter mainParameter = GetParameter(parameterName, MainParameter.ParameterType.Material);
            if (mainParameter != null)
            {
                mainParameter.MaterialValue = value;
            }
        }
        #endregion

        #region Setting
        /// <summary>
        /// 是否启用常规日志打印
        /// </summary>
        public bool IsEnabledLogInfo = true;
        /// <summary>
        /// 是否启用警告日志打印
        /// </summary>
        public bool IsEnabledLogWarning = true;
        /// <summary>
        /// 是否启用错误日志打印
        /// </summary>
        public bool IsEnabledLogError = true;
        /// <summary>
        /// 是否允许将场景添加到发布界面
        /// </summary>
        public bool IsAllowSceneAddBuild = false;
        #endregion

        #region Utility
        private List<HTFAction> _actionQueue = new List<HTFAction>();
        private List<HTFAction> _actionExecuteQueue = new List<HTFAction>();
        private object _mutex = new object();

        private void UtilityUpdate()
        {
            if (_actionQueue.Count > 0)
            {
                lock (_mutex)
                {
                    _actionExecuteQueue.Clear();
                    _actionExecuteQueue.AddRange(_actionQueue);
                    _actionQueue.Clear();
                }
            }
            if (_actionExecuteQueue.Count > 0)
            {
                for (int i = 0; i < _actionExecuteQueue.Count; i++)
                {
                    _actionExecuteQueue[i]();
                }
                _actionExecuteQueue.Clear();
            }
        }

        /// <summary>
        /// 将执行委托排到主线程调用（在子线程中）
        /// </summary>
        /// <param name="action">执行委托</param>
        public void QueueOnMainThread(HTFAction action)
        {
            lock (_mutex)
            {
                _actionQueue.Add(action);
            }
        }
        /// <summary>
        /// 将执行委托排到子线程调用（在主线程中）
        /// </summary>
        /// <param name="action">执行委托</param>
        /// <param name="backToMainThread">执行委托完成后，返回到主线程中回调的委托</param>
        /// <returns>是否执行成功</returns>
        public bool QueueOnSubThread(HTFAction<object> action, HTFAction backToMainThread = null)
        {
            WaitCallback callback = (args) =>
            {
                try
                {
                    action?.Invoke(args);
                }
                catch (Exception e)
                {
                    string error = $"子线程执行中出现异常，子线程方法：{action.Target.GetType().FullName}.{action.Method.Name}，异常信息：{e.Message}";
                    Log.Error(error);
                }
                finally
                {
                    if (backToMainThread != null)
                    {
                        QueueOnMainThread(backToMainThread);
                    }
                }
            };
            return ThreadPool.QueueUserWorkItem(callback);
        }
        /// <summary>
        /// 将执行委托排到子线程调用（在主线程中）
        /// </summary>
        /// <param name="action">执行委托</param>
        /// <param name="state">委托的参数</param>
        /// <param name="backToMainThread">执行委托完成后，返回到主线程中回调的委托</param>
        /// <returns>是否执行成功</returns>
        public bool QueueOnSubThread(HTFAction<object> action, object state, HTFAction backToMainThread = null)
        {
            WaitCallback callback = (args) =>
            {
                try
                {
                    action?.Invoke(args);
                }
                catch (Exception e)
                {
                    string error = $"子线程执行中出现异常，子线程方法：{action.Target.GetType().FullName}.{action.Method.Name}，异常信息：{e.Message}";
                    Log.Error(error);
                }
                finally
                {
                    if (backToMainThread != null)
                    {
                        QueueOnMainThread(backToMainThread);
                    }
                }
            };
            return ThreadPool.QueueUserWorkItem(callback, state);
        }
        #endregion

        #region Version
        [SerializeField] private string _version;

        /// <summary>
        /// 框架的版本号
        /// </summary>
        internal string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
        #endregion

        #region ApplicationFocus
        /// <summary>
        /// 程序焦点事件
        /// </summary>
        public event HTFAction<bool> ApplicationFocusEvent;
        #endregion

        #region ApplicationQuit
        /// <summary>
        /// 程序退出事件
        /// </summary>
        public event HTFAction ApplicationQuitEvent;
        #endregion
    }

    /// <summary>
    /// 框架内置模块
    /// </summary>
    public enum HTFrameworkModule
    {
        AspectTrack,
        Audio,
        Controller,
        Coroutiner,
        CustomModule,
        DataSet,
        Debug,
        ECS,
        Entity,
        Event,
        Exception,
        FSM,
        Hotfix,
        Input,
        Instruction,
        Main,
        Network,
        ObjectPool,
        Procedure,
        ReferencePool,
        Resource,
        StepMaster,
        TaskMaster,
        UI,
        Utility,
        WebRequest
    }

    public delegate void HTFAction();
    public delegate void HTFAction<in T>(T arg);
    public delegate void HTFAction<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void HTFAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult HTFFunc<out TResult>();
    public delegate TResult HTFFunc<in T, out TResult>(T arg);
    public delegate TResult HTFFunc<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult HTFFunc<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate IEnumerator CoroutineAction();
    public delegate IEnumerator CoroutineAction<in T>(T arg);
    public delegate IEnumerator CoroutineAction<in T1, in T2>(T1 arg1, T2 arg2);
}