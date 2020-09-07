using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的调试管理器助手
    /// </summary>
    public sealed class DefaultDebugHelper : IDebugHelper
    {
        private Debugger _debugger;

        /// <summary>
        /// 调试管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {

        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            if (_debugger != null)
            {
                _debugger.OnTermination();
                _debugger = null;
            }
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnUnPause()
        {

        }

        /// <summary>
        /// 初始化调试器
        /// </summary>
        /// <param name="debuggerSkin">调试器皮肤</param>
        public void OnInitDebugger(GUISkin debuggerSkin)
        {
            if (Main.m_Debug.IsEnableDebugger)
            {
                _debugger = new Debugger();
                _debugger.OnInitialization(debuggerSkin);
            }
        }
        /// <summary>
        /// 调试器UI刷新
        /// </summary>
        public void OnDebuggerGUI()
        {
            if (Main.m_Debug.IsEnableDebugger)
            {
                if (_debugger != null)
                {
                    _debugger.OnDebuggerGUI();
                }
            }
        }
    }
}