using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Debug)]
    public sealed class DebugManager : InternalModuleBase<IDebugHelper>
    {
        /// <summary>
        /// 调试器皮肤【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal GUISkin DebuggerSkin;
        /// <summary>
        /// 是否启用调试器
        /// </summary>
        public bool IsEnableDebugger = false;
        /// <summary>
        /// 是否切换为中文【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsChinese = false;
        
        /// <summary>
        /// 当前的帧率
        /// </summary>
        public int FPS
        {
            get
            {
                return _helper.FPS;
            }
        }

        public override void OnInit()
        {
            base.OnInit();
            
            _helper.OnInitDebugger(DebuggerSkin, IsChinese);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (useGUILayout != IsEnableDebugger)
            {
                useGUILayout = IsEnableDebugger;
            }
        }
        private void OnGUI()
        {
            _helper.OnDebuggerGUI();
        }

        /// <summary>
        /// 监控中执行方法
        /// </summary>
        /// <param name="function">方法</param>
        /// <param name="name">监控名称</param>
        /// <returns>监控数据</returns>
        public MonitorData MonitorExecute(HTFAction function, string name = null)
        {
            return _helper.MonitorExecute(function, name);
        }
        /// <summary>
        /// 开始监控
        /// </summary>
        /// <param name="name">监控名称</param>
        public void BeginMonitor(string name = null)
        {
            _helper.BeginMonitor(name);
        }
        /// <summary>
        /// 结束监控
        /// </summary>
        /// <returns>监控数据</returns>
        public MonitorData EndMonitor()
        {
            return _helper.EndMonitor();
        }
    }
}