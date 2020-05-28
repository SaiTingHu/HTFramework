using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试管理器的助手接口
    /// </summary>
    public interface IDebugHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="debuggerSkin">调试器皮肤</param>
        void OnInitialization(GUISkin debuggerSkin);
        /// <summary>
        /// 调试器UI刷新
        /// </summary>
        void OnDebuggerGUI();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();
    }
}