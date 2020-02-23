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

            LicenseInitialization();
            MainDataInitialization();
            ModuleInitialization();
        }

        private void Start()
        {
            MainDataPreparatory();
            ModulePreparatory();
        }

        private void Update()
        {
            LogicLoopRefresh();
            UtilityRefresh();
            ModuleRefresh();
            LicenseRefresh();
        }

        private void FixedUpdate()
        {
            LogicFixedLoopRefresh();
        }

        private void OnGUI()
        {
            LicenseOnGUI();
        }

        private void OnDestroy()
        {
            ModuleTermination();
        }

        private void OnApplicationQuit()
        {
            ApplicationQuitEvent?.Invoke();
        }
    }
}