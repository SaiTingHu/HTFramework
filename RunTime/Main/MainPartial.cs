using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 主程序
    /// </summary>
    public sealed partial class Main : MonoBehaviour
    {
        #region Static Method
        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original) where T : Object
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
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation) where T : Object
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
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
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
        public static T Clone<T>(T original, Transform parent) where T : Object
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
        public static T Clone<T>(T original, Transform parent, bool worldPositionStays) where T : Object
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
                obj.rectTransform().anchoredPosition3D = original.rectTransform().anchoredPosition3D;
                obj.rectTransform().sizeDelta = original.rectTransform().sizeDelta;
                obj.rectTransform().anchorMin = original.rectTransform().anchorMin;
                obj.rectTransform().anchorMax = original.rectTransform().anchorMax;
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
        public static void Kill(Object obj)
        {
            Destroy(obj);
        }
        /// <summary>
        /// 立即杀死实例
        /// </summary>
        /// <param name="obj">实例对象</param>
        public static void KillImmediate(Object obj)
        {
            DestroyImmediate(obj);
        }
        /// <summary>
        /// 杀死一群实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">实例集合</param>
        public static void Kills<T>(List<T> objs) where T : Object
        {
            for (int i = 0; i < objs.Count; i++)
            {
                Destroy(objs[i]);
            }
            objs.Clear();
        }
        #endregion
        
        #region Module
        /// <summary>
        /// 切面调试模块
        /// </summary>
        public static AspectTracker m_AspectTrack { get; private set; }
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
        public static Coroutiner m_Coroutiner { get; private set; }
        /// <summary>
        /// 数据集模块
        /// </summary>
        public static DataSetManager m_DataSet { get; private set; }
        /// <summary>
        /// 调试模块
        /// </summary>
        public static DebugManager m_Debug { get; private set; }
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
        public static ExceptionHandler m_ExceptionHandler { get; private set; }
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
        /// UI模块
        /// </summary>
        public static UIManager m_UI { get; private set; }
        /// <summary>
        /// Web请求模块
        /// </summary>
        public static WebRequestManager m_WebRequest { get; private set; }

        private bool _isPause = false;

        private void ModuleInitialization()
        {
            m_AspectTrack = transform.GetComponentByChild<AspectTracker>("AspectTrack");
            m_Audio = transform.GetComponentByChild<AudioManager>("Audio");
            m_Controller = transform.GetComponentByChild<ControllerManager>("Controller");
            m_Coroutiner = transform.GetComponentByChild<Coroutiner>("Coroutiner");
            m_DataSet = transform.GetComponentByChild<DataSetManager>("DataSet");
            m_Debug = transform.GetComponentByChild<DebugManager>("Debug");
            m_Entity = transform.GetComponentByChild<EntityManager>("Entity");
            m_Event = transform.GetComponentByChild<EventManager>("Event");
            m_ExceptionHandler = transform.GetComponentByChild<ExceptionHandler>("ExceptionHandler");
            m_FSM = transform.GetComponentByChild<FSMManager>("FSM");
            m_Hotfix = transform.GetComponentByChild<HotfixManager>("Hotfix");
            m_Input = transform.GetComponentByChild<InputManager>("Input");
            m_Network = transform.GetComponentByChild<NetworkManager>("Network");
            m_ObjectPool = transform.GetComponentByChild<ObjectPoolManager>("ObjectPool");
            m_Procedure = transform.GetComponentByChild<ProcedureManager>("Procedure");
            m_ReferencePool = transform.GetComponentByChild<ReferencePoolManager>("ReferencePool");
            m_Resource = transform.GetComponentByChild<ResourceManager>("Resource");
            m_StepMaster = transform.GetComponentByChild<StepMaster>("StepMaster");
            m_UI = transform.GetComponentByChild<UIManager>("UI");
            m_WebRequest = transform.GetComponentByChild<WebRequestManager>("WebRequest");

            m_AspectTrack.OnInitialization();
            m_Audio.OnInitialization();
            m_Controller.OnInitialization();
            m_Coroutiner.OnInitialization();
            m_DataSet.OnInitialization();
            m_Debug.OnInitialization();
            m_Entity.OnInitialization();
            m_Event.OnInitialization();
            m_ExceptionHandler.OnInitialization();
            m_FSM.OnInitialization();
            m_Hotfix.OnInitialization();
            m_Input.OnInitialization();
            m_Network.OnInitialization();
            m_ObjectPool.OnInitialization();
            m_Procedure.OnInitialization();
            m_ReferencePool.OnInitialization();
            m_Resource.OnInitialization();
            m_StepMaster.OnInitialization();
            m_UI.OnInitialization();
            m_WebRequest.OnInitialization();
        }
        private void ModulePreparatory()
        {
            m_AspectTrack.OnPreparatory();
            m_Audio.OnPreparatory();
            m_Controller.OnPreparatory();
            m_Coroutiner.OnPreparatory();
            m_DataSet.OnPreparatory();
            m_Debug.OnPreparatory();
            m_Entity.OnPreparatory();
            m_Event.OnPreparatory();
            m_ExceptionHandler.OnPreparatory();
            m_FSM.OnPreparatory();
            m_Hotfix.OnPreparatory();
            m_Input.OnPreparatory();
            m_Network.OnPreparatory();
            m_ObjectPool.OnPreparatory();
            m_Procedure.OnPreparatory();
            m_ReferencePool.OnPreparatory();
            m_Resource.OnPreparatory();
            m_StepMaster.OnPreparatory();
            m_UI.OnPreparatory();
            m_WebRequest.OnPreparatory();
        }
        private void ModuleRefresh()
        {
            if (_isPause)
            {
                return;
            }

            m_AspectTrack.OnRefresh();
            m_Audio.OnRefresh();
            m_Controller.OnRefresh();
            m_Coroutiner.OnRefresh();
            m_DataSet.OnRefresh();
            m_Debug.OnRefresh();
            m_Entity.OnRefresh();
            m_Event.OnRefresh();
            m_ExceptionHandler.OnRefresh();
            m_FSM.OnRefresh();
            m_Hotfix.OnRefresh();
            m_Input.OnRefresh();
            m_Network.OnRefresh();
            m_ObjectPool.OnRefresh();
            m_Procedure.OnRefresh();
            m_ReferencePool.OnRefresh();
            m_Resource.OnRefresh();
            m_StepMaster.OnRefresh();
            m_UI.OnRefresh();
            m_WebRequest.OnRefresh();
        }
        private void ModuleTermination()
        {
            m_AspectTrack.OnTermination();
            m_Audio.OnTermination();
            m_Controller.OnTermination();
            m_Coroutiner.OnTermination();
            m_DataSet.OnTermination();
            m_Debug.OnTermination();
            m_Entity.OnTermination();
            m_Event.OnTermination();
            m_ExceptionHandler.OnTermination();
            m_FSM.OnTermination();
            m_Hotfix.OnTermination();
            m_Input.OnTermination();
            m_Network.OnTermination();
            m_ObjectPool.OnTermination();
            m_Procedure.OnTermination();
            m_ReferencePool.OnTermination();
            m_Resource.OnTermination();
            m_StepMaster.OnTermination();
            m_UI.OnTermination();
            m_WebRequest.OnTermination();
        }
        private void ModulePause()
        {
            m_AspectTrack.OnPause();
            m_Audio.OnPause();
            m_Controller.OnPause();
            m_Coroutiner.OnPause();
            m_DataSet.OnPause();
            m_Debug.OnPause();
            m_Entity.OnPause();
            m_Event.OnPause();
            m_ExceptionHandler.OnPause();
            m_FSM.OnPause();
            m_Hotfix.OnPause();
            m_Input.OnPause();
            m_Network.OnPause();
            m_ObjectPool.OnPause();
            m_Procedure.OnPause();
            m_ReferencePool.OnPause();
            m_Resource.OnPause();
            m_StepMaster.OnPause();
            m_UI.OnPause();
            m_WebRequest.OnPause();
        }
        private void ModuleUnPause()
        {
            m_AspectTrack.OnUnPause();
            m_Audio.OnUnPause();
            m_Controller.OnUnPause();
            m_Coroutiner.OnUnPause();
            m_DataSet.OnUnPause();
            m_Debug.OnUnPause();
            m_Entity.OnUnPause();
            m_Event.OnUnPause();
            m_ExceptionHandler.OnUnPause();
            m_FSM.OnUnPause();
            m_Hotfix.OnUnPause();
            m_Input.OnUnPause();
            m_Network.OnUnPause();
            m_ObjectPool.OnUnPause();
            m_Procedure.OnUnPause();
            m_ReferencePool.OnUnPause();
            m_Resource.OnUnPause();
            m_StepMaster.OnUnPause();
            m_UI.OnUnPause();
            m_WebRequest.OnUnPause();
        }

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
                {
                    return;
                }

                _isPause = value;
                if (_isPause)
                {
                    ModulePause();
                    m_Event.Throw(this, m_ReferencePool.Spawn<EventPauseGame>());
                }
                else
                {
                    ModuleUnPause();
                    m_Event.Throw(this, m_ReferencePool.Spawn<EventUnPauseGame>());
                }
            }
        }
        #endregion
        
        #region License
        /// <summary>
        /// 是否永久授权
        /// </summary>
        public bool IsPermanentLicense = true;
        /// <summary>
        /// 授权到期时，主界面文字提示内容
        /// </summary>
        public string EndingPrompt = "授权已到期！";
        /// <summary>
        /// 授权终止年份
        /// </summary>
        public int Year = 5000;
        /// <summary>
        /// 授权终止月份
        /// </summary>
        public int Month = 5;
        /// <summary>
        /// 授权终止日
        /// </summary>
        public int Day = 5;
        
        private DateTime _endingTime;
        private GUIStyle _promptStyle;
        private Rect _promptStyleRect;

        private void LicenseInitialization()
        {
            _endingTime = new DateTime(Year, Month, Day);
            _promptStyle = new GUIStyle();
            _promptStyle.alignment = TextAnchor.MiddleCenter;
            _promptStyle.normal.textColor = Color.red;
            _promptStyle.fontSize = 30;
        }
        private void LicenseRefresh()
        {
            if (!IsPermanentLicense)
            {
                if (DateTime.Now > _endingTime)
                {
                    m_Controller.MainCamera.enabled = false;
                    m_UI.IsHideAll = true;
                    m_Entity.IsHideAll = true;
                }
            }
        }
        private void LicenseOnGUI()
        {
            if (!IsPermanentLicense)
            {
                if (DateTime.Now > _endingTime)
                {
                    _promptStyleRect.Set(0, 0, Screen.width, Screen.height);
                    GUI.Label(_promptStyleRect, EndingPrompt, _promptStyle);
                }
            }
        }
        #endregion

        #region MainData
        /// <summary>
        /// 当前主要数据类名【请勿在代码中修改】
        /// </summary>
        public string MainDataType = "<None>";

        private MainDataBase _data;

        private void MainDataInitialization()
        {
            if (MainDataType != "<None>")
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(MainDataType);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(MainDataBase)))
                    {
                        _data = Activator.CreateInstance(type) as MainDataBase;
                        _data.OnInitialization();
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("创建全局数据类失败：数据类 {0} 必须继承至基类：MainDataBase！", MainDataType));
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("创建全局数据类失败：丢失数据类 {0}！", MainDataType));
                }
            }
        }
        private void MainDataPreparatory()
        {
            if (_data != null)
            {
                _data.OnPreparatory();
            }
        }

        /// <summary>
        /// 获取全局主要数据
        /// </summary>
        /// <typeparam name="T">数据类</typeparam>
        /// <returns>主要数据对象</returns>
        public T GetMainData<T>() where T : MainDataBase
        {
            if (_data != null)
            {
                return _data as T;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Parameter
        /// <summary>
        /// 主要参数【请勿在代码中修改】
        /// </summary>
        public List<MainParameter> MainParameters = new List<MainParameter>();

        /// <summary>
        /// 是否存在指定名称、类型的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName, MainParameter.ParameterType parameterType)
        {
            if (MainParameters == null)
            {
                return false;
            }
            else
            {
                MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
                return mainParameter != null;
            }
        }
        /// <summary>
        /// 是否存在指定名称的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName)
        {
            if (MainParameters == null)
            {
                return false;
            }
            else
            {
                MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName; });
                return mainParameter != null;
            }
        }
        /// <summary>
        /// 通过名称、类型获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <param name="mainParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, MainParameter.ParameterType parameterType, List<MainParameter> mainParameters)
        {
            if (MainParameters != null)
            {
                for (int i = 0; i < MainParameters.Count; i++)
                {
                    if (MainParameters[i].Name == parameterName && MainParameters[i].Type == parameterType)
                    {
                        mainParameters.Add(MainParameters[i]);
                    }
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
            if (MainParameters != null)
            {
                for (int i = 0; i < MainParameters.Count; i++)
                {
                    if (MainParameters[i].Name == parameterName)
                    {
                        mainParameters.Add(MainParameters[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 通过名称获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public MainParameter GetParameter(string parameterName)
        {
            if (MainParameters == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Main, "当前不存在参数：" + parameterName + "！");
            }
            else
            {
                MainParameter mainParameter = MainParameters.Find((p) => { return p.Name == parameterName; });
                if (mainParameter != null)
                {
                    return mainParameter;
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Main, "当前不存在参数：" + parameterName + "！");
                }
            }
        }
        /// <summary>
        /// 通过名称获取String参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public string GetStringParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.StringValue : "";
        }
        /// <summary>
        /// 通过名称获取Integer参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public int GetIntegerParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.IntegerValue : 0;
        }
        /// <summary>
        /// 通过名称获取Float参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public float GetFloatParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.FloatValue : 0f;
        }
        /// <summary>
        /// 通过名称获取Boolean参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public bool GetBooleanParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.BooleanValue : false;
        }
        /// <summary>
        /// 通过名称获取Vector2参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector2 GetVector2Parameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.Vector2Value : Vector2.zero;
        }
        /// <summary>
        /// 通过名称获取Vector3参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector3 GetVector3Parameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.Vector3Value : Vector3.zero;
        }
        /// <summary>
        /// 通过名称获取Color参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Color GetColorParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.ColorValue : Color.white;
        }
        /// <summary>
        /// 通过名称获取DataSet参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public DataSetBase GetDataSetParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.DataSet : null;
        }
        /// <summary>
        /// 通过名称获取Prefab参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public GameObject GetPrefabParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.PrefabValue : null;
        }
        /// <summary>
        /// 通过名称获取Texture参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Texture GetTextureParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.TextureValue : null;
        }
        /// <summary>
        /// 通过名称获取AudioClip参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public AudioClip GetAudioClipParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.AudioClipValue : null;
        }
        /// <summary>
        /// 通过名称获取Material参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Material GetMaterialParameter(string parameterName)
        {
            MainParameter mainParameter = GetParameter(parameterName);
            return (mainParameter != null) ? mainParameter.MaterialValue : null;
        }
        /// <summary>
        /// 通过名称设置String参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        public void SetStringParameter(string parameterName, string value)
        {
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
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
            MainParameter mainParameter = GetParameter(parameterName);
            if (mainParameter != null)
            {
                mainParameter.MaterialValue = value;
            }
        }
        #endregion

        #region LogicLoop
        /// <summary>
        /// 主逻辑循环事件
        /// </summary>
        public event HTFAction LogicLoopEvent;

        private void LogicLoopRefresh()
        {
            if (_isPause)
            {
                return;
            }

            LogicLoopEvent?.Invoke();
        }
        #endregion
    }

    /// <summary>
    /// 框架模块
    /// </summary>
    public enum HTFrameworkModule
    {
        AspectTrack,
        Audio,
        Controller,
        Coroutiner,
        DataSet,
        Debug,
        Entity,
        Event,
        ExceptionHandler,
        FSM,
        Hotfix,
        Input,
        Main,
        Network,
        ObjectPool,
        Procedure,
        ReferencePool,
        Resource,
        StepEditor,
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