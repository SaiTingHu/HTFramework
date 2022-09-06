using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试管理器的助手接口
    /// </summary>
    public interface IDebugHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 当前的帧率
        /// </summary>
        int FPS { get; }

        /// <summary>
        /// 初始化调试器
        /// </summary>
        /// <param name="debuggerSkin">调试器皮肤</param>
        /// <param name="isChinese">是否切换为中文</param>
        void OnInitDebugger(GUISkin debuggerSkin, bool isChinese);
        /// <summary>
        /// 调试器UI刷新
        /// </summary>
        void OnDebuggerGUI();
        /// <summary>
        /// 监控中执行方法
        /// </summary>
        /// <param name="function">方法</param>
        /// <param name="name">监控名称</param>
        /// <returns>监控数据</returns>
        MonitorData MonitorExecute(HTFAction function, string name = null);
        /// <summary>
        /// 开始监控
        /// </summary>
        /// <param name="name">监控名称</param>
        void BeginMonitor(string name = null);
        /// <summary>
        /// 结束监控
        /// </summary>
        /// <returns>监控数据</returns>
        MonitorData EndMonitor();
    }
}