using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        //AssetBundle资源加载根路径
        private string _assetBundleRootPath;
        //缓存的所有AssetBundle包 <AB包名称、AB包>
        private Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();
        //所有AssetBundle资源包清单
        private AssetBundleManifest _assetBundleManifest;
        //所有AssetBundle的Hash128值 <AB包名称、Hash128值>
        private Dictionary<string, Hash128> _assetBundleHashs = new Dictionary<string, Hash128>();
        //单线下载中
        private bool _isLoading = false;
        //单线下载等待
        private WaitUntil _loadWait;

        /// <summary>
        /// 当前的资源加载模式
        /// </summary>
        public ResourceLoadMode LoadMode
        {
            get
            {
                return Mode;
            }
        }

        public override void OnInitialization()
        {
            base.OnInitialization();

            _assetBundleRootPath = Application.streamingAssetsPath + "/";
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }

        public override void OnTermination()
        {
            base.OnTermination();

            UnLoadAllAsset(true);
            ClearMemory();
        }

        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            if (_assetBundles.ContainsKey(assetBundleName))
            {
                return _assetBundles[assetBundleName];
            }
            else
            {
                return null;
            }
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
            return Main.Current.StartCoroutine(LoadAssetAsync(info, loadingAction, loadDoneAction));
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
            return Main.Current.StartCoroutine(LoadAssetAsync(info, loadingAction, loadDoneAction));
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
            return Main.Current.StartCoroutine(LoadAssetAsync(info, loadingAction, loadDoneAction, true, parent, isUI));
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            _assetBundleRootPath = path;
        }
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (Mode == ResourceLoadMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                if (_assetBundles.ContainsKey(assetBundleName))
                {
                    _assetBundles[assetBundleName].Unload(unloadAllLoadedObjects);
                    _assetBundles.Remove(assetBundleName);
                }
                if (_assetBundleHashs.ContainsKey(assetBundleName))
                {
                    _assetBundleHashs.Remove(assetBundleName);
                }
            }
        }
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            if (Mode == ResourceLoadMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                foreach (var assetBundle in _assetBundles)
                {
                    assetBundle.Value.Unload(unloadAllLoadedObjects);
                }
                _assetBundles.Clear();
                _assetBundleHashs.Clear();
                AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
            }
        }
        /// <summary>
        /// 清理内存，释放空闲内存
        /// </summary>
        public void ClearMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        //异步加载资源
        private IEnumerator LoadAssetAsync<T>(ResourceInfoBase info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            DateTime beginTime = DateTime.Now;

            if (_isLoading)
            {
                yield return _loadWait;
            }

            _isLoading = true;

            yield return Main.Current.StartCoroutine(LoadDependenciesAssetBundleAsync(info.AssetBundleName));
            
            DateTime waitTime = DateTime.Now;
            
            UnityEngine.Object asset = null;

            if (Mode == ResourceLoadMode.Resource)
            {
                ResourceRequest request = Resources.LoadAsync<T>(info.ResourcePath);
                while (!request.isDone)
                {
                    loadingAction?.Invoke(request.progress);
                    yield return null;
                }
                asset = request.asset;
                if (asset)
                {
                    if (isPrefab)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：Resources文件夹中不存在资源 " + info.ResourcePath + "！");
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    loadingAction?.Invoke(1);
                    yield return null;

                    asset = AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
                    if (asset)
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：路径中不存在资源 " + info.AssetPath + "！");
                    }
                }
                else
                {
                    if (_assetBundles.ContainsKey(info.AssetBundleName))
                    {
                        loadingAction?.Invoke(1);
                        yield return null;

                        asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                        if (asset)
                        {
                            if (isPrefab)
                            {
                                asset = ClonePrefab(asset as GameObject, parent, isUI);
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath + " ！");
                        }
                    }
                    else
                    {
                        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(_assetBundleRootPath + info.AssetBundleName, GetAssetBundleHash(info.AssetBundleName)))
                        {
                            request.SendWebRequest();
                            while (!request.isDone)
                            {
                                loadingAction?.Invoke(request.downloadProgress);
                                yield return null;
                            }
                            if (!request.isNetworkError && !request.isHttpError)
                            {
                                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                                if (bundle)
                                {
                                    asset = bundle.LoadAsset<T>(info.AssetPath);
                                    if (asset)
                                    {
                                        if (isPrefab)
                                        {
                                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                                        }
                                    }
                                    else
                                    {
                                        throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath + " ！");
                                    }

                                    if (IsCacheAssetBundle)
                                    {
                                        if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                            _assetBundles.Add(info.AssetBundleName, bundle);
                                    }
                                    else
                                    {
                                        bundle.Unload(false);
                                    }
                                }
                                else
                                {
                                    throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 未下载到AB包！");
                                }
                            }
                            else
                            {
                                throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 遇到网络错误：" + request.error + "！");
                            }
                        }
                    }
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    loadingAction?.Invoke(1);
                    yield return null;

                    asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    if (asset)
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath + " ！");
                    }
                }
                else
                {
                    using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(_assetBundleRootPath + info.AssetBundleName, GetAssetBundleHash(info.AssetBundleName)))
                    {
                        request.SendWebRequest();
                        while (!request.isDone)
                        {
                            loadingAction?.Invoke(request.downloadProgress);
                            yield return null;
                        }
                        if (!request.isNetworkError && !request.isHttpError)
                        {
                            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                            if (bundle)
                            {
                                asset = bundle.LoadAsset<T>(info.AssetPath);
                                if (asset)
                                {
                                    if (isPrefab)
                                    {
                                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                                    }
                                }
                                else
                                {
                                    throw new HTFrameworkException(HTFrameworkModule.Resource, "加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath + " ！");
                                }

                                if (IsCacheAssetBundle)
                                {
                                    if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                        _assetBundles.Add(info.AssetBundleName, bundle);
                                }
                                else
                                {
                                    bundle.Unload(false);
                                }
                            }
                            else
                            {
                                throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 未下载到AB包！");
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 遇到网络错误：" + request.error + "！");
                        }
                    }
                }
#endif
            }

            DateTime endTime = DateTime.Now;
            
            GlobalTools.LogInfo(string.Format("异步加载资源{0}[{1}模式]：\r\n{2}\r\n等待耗时：{3}秒  加载耗时：{4}秒", asset ? "成功" : "失败", Mode
                , Mode == ResourceLoadMode.Resource ? info.GetResourceFullPath() : info.GetAssetBundleFullPath(_assetBundleRootPath)
                , (waitTime - beginTime).TotalSeconds, (endTime - waitTime).TotalSeconds));

            if (asset)
            {
                DataSetInfo dataSet = info as DataSetInfo;
                if (dataSet != null && dataSet.Data != null)
                {
                    asset.Cast<DataSetBase>().Fill(dataSet.Data);
                }

                loadDoneAction?.Invoke(asset as T);
            }
            asset = null;

            _isLoading = false;
        }
        //克隆预制体
        private GameObject ClonePrefab(GameObject prefabTem, Transform parent, bool isUI)
        {
            GameObject prefab = Instantiate(prefabTem) as GameObject;

            if (parent)
            {
                prefab.transform.SetParent(parent);
            }

            if (isUI)
            {
                prefab.rectTransform().anchoredPosition3D = prefabTem.rectTransform().anchoredPosition3D;
                prefab.rectTransform().sizeDelta = prefabTem.rectTransform().sizeDelta;
                prefab.rectTransform().anchorMin = prefabTem.rectTransform().anchorMin;
                prefab.rectTransform().anchorMax = prefabTem.rectTransform().anchorMax;
                prefab.transform.localRotation = Quaternion.identity;
                prefab.transform.localScale = Vector3.one;
            }
            else
            {
                prefab.transform.localPosition = prefabTem.transform.localPosition;
                prefab.transform.localRotation = Quaternion.identity;
                prefab.transform.localScale = Vector3.one;
            }

            prefab.SetActive(false);
            return prefab;
        }
        //异步加载依赖AB包
        private IEnumerator LoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (Mode == ResourceLoadMode.AssetBundle)
            {
#if UNITY_EDITOR
                if (!IsEditorMode)
                {
                    yield return Main.Current.StartCoroutine(LoadAssetBundleManifestAsync());

                    if (_assetBundleManifest)
                    {
                        string[] dependencies = _assetBundleManifest.GetAllDependencies(assetBundleName);
                        foreach (string item in dependencies)
                        {
                            if (_assetBundles.ContainsKey(item))
                            {
                                continue;
                            }

                            yield return Main.Current.StartCoroutine(LoadAssetBundleAsync(item));
                        }
                    }
                }
#else
                yield return Main.Current.StartCoroutine(LoadAssetBundleManifestAsync());

                if (_assetBundleManifest)
                {
                    string[] dependencies = _assetBundleManifest.GetAllDependencies(assetBundleName);
                    foreach (string item in dependencies)
                    {
                        if (_assetBundles.ContainsKey(item))
                        {
                            continue;
                        }

                        yield return Main.Current.StartCoroutine(LoadAssetBundleAsync(item));
                    }
                }
#endif
            }
            yield return null;
        }
        //异步加载AB包清单
        private IEnumerator LoadAssetBundleManifestAsync()
        {
            if (string.IsNullOrEmpty(AssetBundleManifestName) || AssetBundleManifestName == "")
            {
                throw new HTFrameworkException(HTFrameworkModule.Resource, "请设置资源管理模块的 Manifest Name 属性，为所有AB包提供依赖清单！");
            }
            else
            {
                if (_assetBundleManifest == null)
                {
                    yield return Main.Current.StartCoroutine(LoadAssetBundleAsync(AssetBundleManifestName, true));

                    if (_assetBundles.ContainsKey(AssetBundleManifestName))
                    {
                        _assetBundleManifest = _assetBundles[AssetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnLoadAsset(AssetBundleManifestName);
                    }
                }
            }
            yield return null;
        }
        //异步加载AB包
        private IEnumerator LoadAssetBundleAsync(string assetBundleName, bool isManifest = false)
        {
            if (!_assetBundles.ContainsKey(assetBundleName))
            {
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(_assetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(_assetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            _assetBundles.Add(assetBundleName, bundle);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 未下载到AB包！");
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, "请求：" + request.url + " 遇到网络错误：" + request.error + "！");
                    }
                }
            }
            yield return null;
        }
        //获取AB包的hash值
        private Hash128 GetAssetBundleHash(string assetBundleName)
        {
            if (_assetBundleHashs.ContainsKey(assetBundleName))
            {
                return _assetBundleHashs[assetBundleName];
            }
            else
            {
                Hash128 hash = _assetBundleManifest.GetAssetBundleHash(assetBundleName);
                _assetBundleHashs.Add(assetBundleName, hash);
                return hash;
            }
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