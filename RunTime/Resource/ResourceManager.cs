using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Resource)]
    public sealed class ResourceManager : InternalModuleBase<IResourceHelper>
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
        /// 所有AssetBundle资源包清单的名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string AssetBundleManifestName;
        /// <summary>
        /// 是否打印资源加载细节日志【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsLogDetail = false;

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

        public override void OnInit()
        {
            base.OnInit();

            _helper.SetLoader(Mode, IsEditorMode, AssetBundleManifestName, IsLogDetail);
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（必须以 / 结尾，仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            _helper.SetAssetBundlePath(path);
        }
        /// <summary>
        /// 通过名称获取指定的AssetBundle（仅限 AssetBundle 模式）
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
        /// <param name="onLoading">资源加载中回调</param>
        /// <param name="onLoadDone">资源加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadAsset<T>(AssetInfo info, HTFAction<float> onLoading = null, HTFAction<T> onLoadDone = null) where T : Object
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, onLoading, onLoadDone, false, null, false));
        }
        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="info">数据集配置信息</param>
        /// <param name="onLoading">数据集加载中回调</param>
        /// <param name="onLoadDone">数据集加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadDataSet<T>(DataSetInfo info, HTFAction<float> onLoading = null, HTFAction<T> onLoadDone = null) where T : DataSetBase
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, onLoading, onLoadDone, false, null, false));
        }
        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        /// <param name="info">预制体配置信息</param>
        /// <param name="parent">预制体的预设父物体</param>
        /// <param name="onLoading">预制体加载中回调</param>
        /// <param name="onLoadDone">预制体加载完成回调</param>
        /// <param name="isUI">预制体是否是UI</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefab(PrefabInfo info, Transform parent, HTFAction<float> onLoading = null, HTFAction<GameObject> onLoadDone = null, bool isUI = false)
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(info, onLoading, onLoadDone, true, parent, isUI));
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">场景配置信息</param>
        /// <param name="onLoading">场景加载中回调</param>
        /// <param name="onLoadDone">场景加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadScene(SceneInfo info, HTFAction<float> onLoading = null, HTFAction onLoadDone = null)
        {
            return Main.Current.StartCoroutine(_helper.LoadSceneAsync(info, onLoading, onLoadDone));
        }
        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="onLoading">资源加载中回调</param>
        /// <param name="onLoadDone">资源加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadAsset<T>(string location, HTFAction<float> onLoading = null, HTFAction<T> onLoadDone = null) where T : Object
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(location, typeof(AssetInfo), onLoading, onLoadDone, false, null, false));
        }
        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="onLoading">数据集加载中回调</param>
        /// <param name="onLoadDone">数据集加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadDataSet<T>(string location, HTFAction<float> onLoading = null, HTFAction<T> onLoadDone = null) where T : DataSetBase
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(location, typeof(DataSetInfo), onLoading, onLoadDone, false, null, false));
        }
        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="parent">预制体的预设父物体</param>
        /// <param name="onLoading">预制体加载中回调</param>
        /// <param name="onLoadDone">预制体加载完成回调</param>
        /// <param name="isUI">预制体是否是UI</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefab(string location, Transform parent, HTFAction<float> onLoading = null, HTFAction<GameObject> onLoadDone = null, bool isUI = false)
        {
            return Main.Current.StartCoroutine(_helper.LoadAssetAsync(location, typeof(PrefabInfo), onLoading, onLoadDone, true, parent, isUI));
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="onLoading">场景加载中回调</param>
        /// <param name="onLoadDone">场景加载完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadScene(string location, HTFAction<float> onLoading = null, HTFAction onLoadDone = null)
        {
            return Main.Current.StartCoroutine(_helper.LoadSceneAsync(location, onLoading, onLoadDone));
        }

        /// <summary>
        /// 卸载资源（仅限 Addressables 模式）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        public void UnLoadAsset(string location)
        {
            _helper.UnLoadAsset(location);
        }
        /// <summary>
        /// 卸载AB包（异步，仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程</returns>
        public Coroutine UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = true)
        {
            return Main.Current.StartCoroutine(_helper.UnLoadAssetBundleAsync(assetBundleName, unloadAllLoadedObjects));
        }
        /// <summary>
        /// 卸载所有AB包（异步，仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程</returns>
        public Coroutine UnLoadAllAssetBundle(bool unloadAllLoadedObjects = true)
        {
            return Main.Current.StartCoroutine(_helper.UnLoadAllAssetBundleAsync(unloadAllLoadedObjects));
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">场景配置信息</param>
        /// <returns>卸载协程</returns>
        public Coroutine UnLoadScene(SceneInfo info)
        {
            return Main.Current.StartCoroutine(_helper.UnLoadSceneAsync(info));
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <returns>卸载协程</returns>
        public Coroutine UnLoadScene(string location)
        {
            return Main.Current.StartCoroutine(_helper.UnLoadSceneAsync(location));
        }
        /// <summary>
        /// 卸载所有场景（异步）
        /// </summary>
        /// <returns>卸载协程</returns>
        public Coroutine UnLoadAllScene()
        {
            return Main.Current.StartCoroutine(_helper.UnLoadAllSceneAsync());
        }

        /// <summary>
        /// 清理内存，释放空闲内存（异步）
        /// </summary>
        /// <returns>协程</returns>
        public Coroutine ClearMemory()
        {
            return Main.Current.StartCoroutine(_helper.ClearMemory());
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
        AssetBundle,
        /// <summary>
        /// 使用Addressables加载
        /// </summary>
        Addressables
    }
}