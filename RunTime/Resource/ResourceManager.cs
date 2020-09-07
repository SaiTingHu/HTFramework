using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Resource)]
    public sealed class ResourceManager : InternalModuleBase
    {
        /// <summary>
        /// 资源加载模式【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal ResourceLoadMode Mode = ResourceLoadMode.Resource;
        /// <summary>
        /// 是否是编辑器模式【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEditorMode = true;
        /// <summary>
        /// 是否缓存AB包【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsCacheAssetBundle = true;
        /// <summary>
        /// 所有AssetBundle资源包清单的名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string AssetBundleManifestName;
        
        private IResourceHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IResourceHelper;
            _helper.SetLoader(Mode, IsEditorMode, IsCacheAssetBundle, AssetBundleManifestName);
        }

        /// <summary>
        /// 当前的资源加载模式
        /// </summary>
        public ResourceLoadMode LoadMode
        {
            get
            {
                return _helper.LoadMode;
            }
        }
        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            return _helper.GetAssetBundle(assetBundleName);
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源配置信息</param>
        /// <param name="loadingAction">资源加载中回调</param>
        /// <param name="loadDoneAction">资源加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadAsset<T>(AssetInfo info, HTFAction<float> loadingAction = null, HTFAction<T> loadDoneAction = null) where T : UnityEngine.Object
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, loadingAction, loadDoneAction, false, null, false));
        }
        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="info">数据集配置信息</param>
        /// <param name="loadingAction">数据集加载中回调</param>
        /// <param name="loadDoneAction">数据集加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadDataSet<T>(DataSetInfo info, HTFAction<float> loadingAction = null, HTFAction<T> loadDoneAction = null) where T : DataSetBase
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, loadingAction, loadDoneAction, false, null, false));
        }
        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        /// <param name="info">预制体配置信息</param>
        /// <param name="parent">预制体的预设父物体</param>
        /// <param name="loadingAction">预制体加载中回调</param>
        /// <param name="loadDoneAction">预制体加载完成回调</param>
        /// <param name="isUI">预制体是否是UI</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefab(PrefabInfo info, Transform parent, HTFAction<float> loadingAction = null, HTFAction<GameObject> loadDoneAction = null, bool isUI = false)
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, loadingAction, loadDoneAction, true, parent, isUI));
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            _helper.SetAssetBundlePath(path);
        }
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            _helper.UnLoadAsset(assetBundleName, unloadAllLoadedObjects);
        }
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            _helper.UnLoadAllAsset(unloadAllLoadedObjects);
        }
        /// <summary>
        /// 清理内存，释放空闲内存
        /// </summary>
        public void ClearMemory()
        {
            _helper.ClearMemory();
        }
    }

    /// <summary>
    /// 资源加载模式
    /// </summary>
    public enum ResourceLoadMode
    {
        /// <summary>
        /// 使用Resource加载
        /// </summary>
        Resource,
        /// <summary>
        /// 使用AssetBundle加载
        /// </summary>
        AssetBundle
    }
}