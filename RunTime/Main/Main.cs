namespace HT.Framework
{
    /// <summary>
    /// HTFramework 主模块
    /// </summary>
    public sealed partial class Main : InternalModuleBase<IMainHelper>
    {
        public override void OnInitialization()
        {
            base.OnInitialization();
            
            LicenseInitialization();
            DataModelInitialization();
            ModuleInitialization();
        }
        public override void OnPreparatory()
        {
            base.OnPreparatory();

            LicensePreparatory();
            DataModelPreparatory();
            ModulePreparatory();
        }
        public override void OnRefresh()
        {
            base.OnRefresh();

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