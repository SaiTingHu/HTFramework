using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 主程序
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public sealed partial class Main : MonoBehaviour
    {
        /// <summary>
        /// 当前主程序
        /// </summary>
        public static Main Current { get; private set; }
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Current = this;

            LicenseAwake();

            MainDataAwake();

            ModuleInitialization();

            ModulePreparatory();
        }

        private void Update()
        {
            ModuleRefresh();

            LicenseUpdate();
        }

        private void OnDestroy()
        {
            ModuleTermination();
        }

        private void OnGUI()
        {
            LicenseOnGUI();
        }
    }
}
