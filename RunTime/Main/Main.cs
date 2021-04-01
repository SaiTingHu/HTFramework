namespace HT.Framework
{
    /// <summary>
    /// HTFramework 主模块
    /// </summary>
    public sealed partial class Main : InternalModuleBase<IMainHelper>
    {
        /// <summary>
        /// 当前主程序
        /// </summary>
        public static Main Current { get; private set; }

        public override void OnInitialization()
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
        public override void OnPreparatory()
        {
            base.OnPreparatory();

            LicensePreparatory();
            MainDataPreparatory();
            ModulePreparatory();
        }
        public override void OnRefresh()
        {
            base.OnRefresh();

            LogicLoopRefresh();
            UtilityRefresh();
            ModuleRefresh();
        }
        public override void OnTermination()
        {
            base.OnTermination();

            ModuleTermination();
        }
        public override void OnPause()
        {
            base.OnPause();

            ModulePause();
        }
        public override void OnResume()
        {
            base.OnResume();

            ModuleResume();
        }
    }
}