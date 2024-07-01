namespace HT.Framework
{
    /// <summary>
    /// HTFramework 主模块
    /// </summary>
    public sealed partial class Main : InternalModuleBase<IMainHelper>
    {
        public override void OnInit()
        {
            base.OnInit();
            
            LicenseInit();
            DataModelInit();
            ModuleInit();

            IsInitCompleted = true;
        }
        public override void OnReady()
        {
            base.OnReady();

            LicenseReady();
            DataModelReady();
            ModuleReady();

            IsReadyCompleted = true;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            ModuleUpdate();
            BehaviourUpdate();
            UtilityUpdate();
        }
        public override void OnTerminate()
        {
            base.OnTerminate();

            ModuleTerminate();
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