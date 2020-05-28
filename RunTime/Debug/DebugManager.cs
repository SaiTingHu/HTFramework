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

        private IDebugHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IDebugHelper;
            _helper.OnInitialization(DebuggerSkin);
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }
        private void OnGUI()
        {
            _helper.OnDebuggerGUI();
        }
    }
}