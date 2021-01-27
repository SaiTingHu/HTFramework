namespace HT.Framework
{
    /// <summary>
    /// HTFramework 主模块
    /// </summary>
    public sealed partial class Main : InternalModuleBase
    {
        /// <summary>
        /// 当前主程序
        /// </summary>
        public static Main Current { get; private set; }
        
        internal override void OnInitialization()
        {
            base.OnInitialization();
            
            if (Current == null)
            {
                Current = this;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Main, "框架致命错误：不能存在两个及以上Main主模块！");
            }

            LicenseInitialization();
            MainDataInitialization();
            ModuleInitialization();
        }

        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            LicensePreparatory();
            MainDataPreparatory();
            ModulePreparatory();
        }

        internal override void OnRefresh()
        {
            base.OnRefresh();

            LogicLoopRefresh();
            UtilityRefresh();
            ModuleRefresh();
        }

        internal void OnFixedRefresh()
        {
            LogicFixedLoopRefresh();
        }

        internal void OnMainGUI()
        {
            LicenseOnGUI();
        }

        internal override void OnTermination()
        {
            base.OnTermination();

            ModuleTermination();
        }

        internal override void OnPause()
        {
            base.OnPause();

            ModulePause();
        }

        internal override void OnUnPause()
        {
            base.OnUnPause();

            ModuleUnPause();
        }
    }
}