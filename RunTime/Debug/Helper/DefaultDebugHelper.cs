using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的调试管理器助手
    /// </summary>
    internal sealed class DefaultDebugHelper : IDebugHelper
    {
        private DebugManager _debugManager;
        private Debugger _debugger;
        private bool _isEnableDebugger = false;
        private string _monitorName;
        private float _monitorBeginTime;
        private long _monitorBeginMemory;
        private int _monitorBeginGC;

        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 当前的帧率
        /// </summary>
        public int FPS
        {
            get
            {
                if (_debugManager.IsEnableDebugger && _debugger != null)
                {
                    return _debugger.FPS;
                }
                else
                {
                    return (int)(1.0f / Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            _debugManager = Module as DebugManager;
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_debugger != null)
            {
                _debugger.RefreshFPS();
                if (_isEnableDebugger != _debugManager.IsEnableDebugger)
                {
                    _isEnableDebugger = _debugManager.IsEnableDebugger;
                    _debugger.RefreshMaskState();
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            if (_debugger != null)
            {
                _debugger.OnTerminate();
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
        public void OnResume()
        {

        }

        /// <summary>
        /// 初始化调试器
        /// </summary>
        /// <param name="debuggerSkin">调试器皮肤</param>
        /// <param name="isChinese">是否切换为中文</param>
        public void OnInitDebugger(GUISkin debuggerSkin, bool isChinese)
        {
            if (_debugManager.IsEnableDebugger)
            {
                _debugger = new Debugger();
                _debugger.OnInit(debuggerSkin, isChinese);
            }
        }
        /// <summary>
        /// 调试器UI刷新
        /// </summary>
        public void OnDebuggerGUI()
        {
            if (_debugManager.IsEnableDebugger)
            {
                if (_debugger != null)
                {
                    _debugger.OnDebuggerGUI();
                }
            }
        }
        /// <summary>
        /// 监控中执行方法
        /// </summary>
        /// <param name="function">方法</param>
        /// <param name="name">监控名称</param>
        /// <returns>监控数据</returns>
        public MonitorData MonitorExecute(HTFAction function, string name = null)
        {
            float beginTime = Time.realtimeSinceStartup;
            long beginMemory = GC.GetTotalMemory(false);
            int beginGC = GC.CollectionCount(0);

            function();

            float endTime = Time.realtimeSinceStartup;
            long endMemory = GC.GetTotalMemory(false);
            int endGC = GC.CollectionCount(0);

            MonitorData data = new MonitorData();
            data.Name = string.IsNullOrEmpty(name) ? function.Method.Name : name;
            data.Elapsed = endTime - beginTime;
            data.MemoryIncrement = endMemory - beginMemory;
            data.GCCount = endGC - beginGC;
            return data;
        }
        /// <summary>
        /// 开始监控
        /// </summary>
        /// <param name="name">监控名称</param>
        public void BeginMonitor(string name = null)
        {
            _monitorName = name;
            _monitorBeginTime = Time.realtimeSinceStartup;
            _monitorBeginMemory = GC.GetTotalMemory(false);
            _monitorBeginGC = GC.CollectionCount(0);
        }
        /// <summary>
        /// 结束监控
        /// </summary>
        /// <returns>监控数据</returns>
        public MonitorData EndMonitor()
        {
            float endTime = Time.realtimeSinceStartup;
            long endMemory = GC.GetTotalMemory(false);
            int endGC = GC.CollectionCount(0);

            MonitorData data = new MonitorData();
            data.Name = _monitorName;
            data.Elapsed = endTime - _monitorBeginTime;
            data.MemoryIncrement = endMemory - _monitorBeginMemory;
            data.GCCount = endGC - _monitorBeginGC;
            return data;
        }
    }
}