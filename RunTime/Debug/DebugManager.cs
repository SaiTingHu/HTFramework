using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Debug)]
    public sealed class DebugManager : InternalModuleBase
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

        private IDebugHelper _helper;

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

        private DebugManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IDebugHelper;
            _helper.OnInitDebugger(DebuggerSkin, IsChinese);
        }
        private void OnGUI()
        {
            _helper.OnDebuggerGUI();
        }
    }
}