using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 主程序
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public sealed class Main : MonoBehaviour
    {
        /// <summary>
        /// 当前主程序
        /// </summary>
        public static Main Current { get; private set; }

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

        /// <summary>
        /// 所有模块的切面追踪代理者
        /// </summary>
        private static List<AspectProxyModule<IAspectProxyModule>> _moduleProxys = new List<AspectProxyModule<IAspectProxyModule>>();
        /// <summary>
        /// 所有模块的切面追踪代理者实例
        /// </summary>
        private static List<IAspectProxyModule> _moduleProxysObj = new List<IAspectProxyModule>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Current = this;
            m_AspectTrack = transform.GetComponentByChild<AspectTracker>("AspectTrack");
            m_Audio = transform.GetComponentByChild<AudioManager>("Audio");
            m_Controller = transform.GetComponentByChild<ControllerManager>("Controller");
            m_Event = transform.GetComponentByChild<EventManager>("Event");
            m_ExceptionHandler = transform.GetComponentByChild<ExceptionHandler>("ExceptionHandler");
            m_FSM = transform.GetComponentByChild<FSMManager>("FSM");
            m_Input = transform.GetComponentByChild<InputManager>("Input");
            m_Network = transform.GetComponentByChild<NetworkManager>("Network");
            m_ObjectPool = transform.GetComponentByChild<ObjectPoolManager>("ObjectPool");
            m_Procedure = transform.GetComponentByChild<ProcedureManager>("Procedure");
            m_ReferencePool = transform.GetComponentByChild<ReferencePoolManager>("ReferencePool");
            m_Resource = transform.GetComponentByChild<ResourceManager>("Resource");
            m_StepMaster = transform.GetComponentByChild<StepMaster>("StepMaster");
            m_UI = transform.GetComponentByChild<UIManager>("UI");
            m_WebRequest = transform.GetComponentByChild<WebRequestManager>("WebRequest");

            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_AspectTrack));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Audio));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Controller));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Event));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_ExceptionHandler));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_FSM));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Input));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Network));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_ObjectPool));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Procedure));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_ReferencePool));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_Resource));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_StepMaster));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_UI));
            _moduleProxys.Add(new AspectProxyModule<IAspectProxyModule>(m_WebRequest));
            for (int i = 0; i < _moduleProxys.Count; i++)
            {
                _moduleProxysObj.Add(m_AspectTrack.CreateTracker(_moduleProxys[i]));
            }
            for (int i = 0; i < _moduleProxysObj.Count; i++)
            {
                _moduleProxysObj[i].Initialization();
            }

            m_AspectTrack.Preparatory();
            m_Audio.Preparatory();
            m_Controller.Preparatory();
            m_Event.Preparatory();
            m_ExceptionHandler.Preparatory();
            m_FSM.Preparatory();
            m_Input.Preparatory();
            m_Network.Preparatory();
            m_ObjectPool.Preparatory();
            m_Procedure.Preparatory();
            m_ReferencePool.Preparatory();
            m_Resource.Preparatory();
            m_StepMaster.Preparatory();
            m_UI.Preparatory();
            m_WebRequest.Preparatory();
        }

        private void Update()
        {
            m_AspectTrack.Refresh();
            m_Audio.Refresh();
            m_Controller.Refresh();
            m_Event.Refresh();
            m_ExceptionHandler.Refresh();
            m_FSM.Refresh();
            m_Input.Refresh();
            m_Network.Refresh();
            m_ObjectPool.Refresh();
            m_Procedure.Refresh();
            m_ReferencePool.Refresh();
            m_Resource.Refresh();
            m_StepMaster.Refresh();
            m_UI.Refresh();
            m_WebRequest.Refresh();
        }

        private void OnDestroy()
        {
            m_AspectTrack.Termination();
            m_Audio.Termination();
            m_Controller.Termination();
            m_Event.Termination();
            m_ExceptionHandler.Termination();
            m_FSM.Termination();
            m_Input.Termination();
            m_Network.Termination();
            m_ObjectPool.Termination();
            m_Procedure.Termination();
            m_ReferencePool.Termination();
            m_Resource.Termination();
            m_StepMaster.Termination();
            m_UI.Termination();
            m_WebRequest.Termination();
        }

        /// <summary>
        /// 克隆实例
        /// </summary>
        public static T Clone<T>(T original) where T : Object
        {
            return Instantiate(original);
        }

        /// <summary>
        /// 杀死实例
        /// </summary>
        public static void Kill(Object obj)
        {
            Destroy(obj);
        }
    }
}
