using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 热更新模块管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HotfixManager : ModuleManager
    {
        public bool IsEnableHotfix = false;
        /// <summary>
        /// 热更新逻辑库路径
        /// </summary>
        public string HotfixLibraryPath = "";

        private HotfixObject _hotfixObject;

        public override void Initialization()
        {
            base.Initialization();

            if (IsEnableHotfix)
            {
                _hotfixObject = new HotfixObject();
            }
        }

        public override void Refresh()
        {
            base.Refresh();

            if (IsEnableHotfix)
            {
                if (_hotfixObject != null)
                    _hotfixObject.UpdateLogic();
            }
        }
    }
}
