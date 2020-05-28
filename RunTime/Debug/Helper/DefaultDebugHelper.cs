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
        /// 初始化
        /// </summary>
        /// <param name="debuggerSkin">调试器皮肤</param>
        public void OnInitialization(GUISkin debuggerSkin)
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
        /// <summary>
        /// 终结
        /// </summary>
        public void OnTermination()
        {
            if (_debugger != null)
            {
                _debugger.OnTermination();
                _debugger = null;
            }
        }
    }
}