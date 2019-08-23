using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ResourceManager : ModuleManager
    {
        /// <summary>
        /// 资源加载模式【请勿在代码中修改】
        /// </summary>
        public ResourceLoadMode Mode = ResourceLoadMode.Resource;
        /// <summary>
        /// 是否是编辑器模式【请勿在代码中修改】
        /// </summary>
        public bool IsEditorMode = true;
        /// <summary>
        /// 是否缓存AB包【请勿在代码中修改】
        /// </summary>
        public bool IsCacheAssetBundle = true;
        /// <summary>
        /// 所有AssetBundle资源包清单的名称【请勿在代码中修改】
        /// </summary>
        public string AssetBundleManifestName;

        //AssetBundle资源加载根路径
        private string _assetBundleRootPath;
        //缓存的所有AssetBundle包
        private Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();
        //所有AssetBundle资源包清单
        private AssetBundleManifest _assetBundleManifest;
        //单线下载中
        private bool _isLoading = false;
        //单线下载等待
        private WaitUntil _loadWait;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _assetBundleRootPath = Application.streamingAssetsPath + "/";
            _loadWait = new WaitUntil(() => { return !_isLoading; });

            LoadAssetBundleManifest();
        }

        public override void OnTermination()
        {
            base.OnTermination();

            UnLoadAllAsset();
            ClearMemory();
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源配置信息</param>
        /// <param name="loadingAction">资源加载中回调</param>
        /// <param name="loadDoneAction">资源加载完成回调</param>
        public void LoadAssetAsync<T>(AssetInfo info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAssetCoroutine(info, loadingAction, loadDoneAction));
        }
        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="info">数据集配置信息</param>
        /// <param name="loadingAction">数据集加载中回调</param>
        /// <param name="loadDoneAction">数据集加载完成回调</param>
        public void LoadDataSetAsync<T>(DataSetInfo info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction) where T : DataSet
        {
            StartCoroutine(LoadAssetCoroutine(info, loadingAction, loadDoneAction));
        }
        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        /// <param name="info">预制体配置信息</param>
        /// <param name="parent">预制体的预设父物体</param>
        /// <param name="loadingAction">预制体加载中回调</param>
        /// <param name="loadDoneAction">预制体加载完成回调</param>
        /// <param name="isUI">预制体是否是UI</param>
        public void LoadPrefabAsync(PrefabInfo info, Transform parent, HTFAction<float> loadingAction, HTFAction<GameObject> loadDoneAction, bool isUI = false)
        {
            StartCoroutine(LoadAssetCoroutine(info, loadingAction, loadDoneAction, true, parent, isUI));
        }
        /// <summary>
        /// 加载资源（同步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源配置信息</param>
        /// <returns>加载完成的资源</returns>
        public T LoadAssetSynch<T>(AssetInfo info) where T : UnityEngine.Object
        {
            return LoadAsset<T>(info);
        }
        /// <summary>
        /// 加载数据集（同步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="info">数据集配置信息</param>
        /// <returns>加载完成的数据集</returns>
        public T LoadDataSetSynch<T>(DataSetInfo info) where T : DataSet
        {
            return LoadAsset<T>(info);
        }
        /// <summary>
        /// 加载预制体（同步）
        /// </summary>
        /// <param name="info">预制体配置信息</param>
        /// <param name="parent">预制体的预设父物体</param>
        /// <param name="isUI">预制体是否是UI</param>
        /// <returns>加载完成的预制体</returns>
        public GameObject LoadPrefabSynch(PrefabInfo info, Transform parent, bool isUI = false)
        {
            return LoadAsset<GameObject>(info, true, parent, isUI);
        }
        
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        public void SetAssetBundlePath(string path)
        {
            _assetBundleRootPath = path;
        }
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
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
            }
        }
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            if (Mode == ResourceLoadMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                foreach (KeyValuePair<string, AssetBundle> asset in _assetBundles)
                {
                    asset.Value.Unload(unloadAllLoadedObjects);
                }
                _assetBundles.Clear();
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
        private IEnumerator LoadAssetCoroutine<T>(ResourceInfoBase info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            DateTime beginTime = DateTime.Now;

            if (_isLoading)
            {
                yield return _loadWait;
            }

            LoadDependenciesAssetBundle(info.AssetBundleName);

            DateTime waitTime = DateTime.Now;

            _isLoading = true;

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
                    GlobalTools.LogError(string.Format("加载资源失败：Resources文件夹中不存在资源 {0}！", info.ResourcePath));
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
                        GlobalTools.LogError(string.Format("加载资源失败：路径中不存在资源 {0}！", info.AssetPath));
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
                            GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                        }
                    }
                    else
                    {
                        UnityWebRequest request = UnityWebRequest.Get(_assetBundleRootPath + info.AssetBundleName);
                        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                        request.downloadHandler = handler;
                        request.SendWebRequest();
                        while (!request.isDone)
                        {
                            loadingAction?.Invoke(request.downloadProgress);
                            yield return null;
                        }
                        if (!request.isNetworkError && !request.isHttpError)
                        {
                            if (handler.assetBundle)
                            {
                                asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                                if (asset)
                                {
                                    if (isPrefab)
                                    {
                                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                                    }
                                }
                                else
                                {
                                    GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                                }

                                if (IsCacheAssetBundle)
                                {
                                    if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                        _assetBundles.Add(info.AssetBundleName, handler.assetBundle);
                                }
                                else
                                {
                                    handler.assetBundle.Unload(false);
                                }
                            }
                            else
                            {
                                GlobalTools.LogError(string.Format("请求：{0} 未下载到AB包！", request.url));
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("请求：{0} 遇到网络错误：{1}！", request.url, request.error));
                        }
                        request.Dispose();
                        handler.Dispose();
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
                        GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get(_assetBundleRootPath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                    request.downloadHandler = handler;
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        loadingAction?.Invoke(request.downloadProgress);
                        yield return null;
                    }
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                            if (asset)
                            {
                                if (isPrefab)
                                {
                                    asset = ClonePrefab(asset as GameObject, parent, isUI);
                                }
                            }
                            else
                            {
                                GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                            }

                            if (IsCacheAssetBundle)
                            {
                                if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                    _assetBundles.Add(info.AssetBundleName, handler.assetBundle);
                            }
                            else
                            {
                                handler.assetBundle.Unload(false);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("请求：{0} 未下载到AB包！", request.url));
                        }
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("请求：{0} 遇到网络错误：{1}！", request.url, request.error));
                    }
                    request.Dispose();
                    handler.Dispose();
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
                    asset.Cast<DataSet>().Fill(dataSet.Data);
                }

                loadDoneAction?.Invoke(asset as T);
            }
            asset = null;

            _isLoading = false;
        }
        //同步加载资源
        private T LoadAsset<T>(ResourceInfoBase info, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            DateTime beginTime = DateTime.Now;

            LoadDependenciesAssetBundle(info.AssetBundleName);

            UnityEngine.Object asset = null;

            if (Mode == ResourceLoadMode.Resource)
            {
                asset = Resources.Load<T>(info.ResourcePath);
                if (asset)
                {
                    if (isPrefab)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("加载资源失败：Resources文件夹中不存在资源 {0}！", info.ResourcePath));
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
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
                        GlobalTools.LogError(string.Format("加载资源失败：路径中不存在资源 {0}！", info.AssetPath));
                    }
                }
                else
                {
                    if (_assetBundles.ContainsKey(info.AssetBundleName))
                    {
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
                            GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                        }
                    }
                    else
                    {
                        UnityWebRequest request = UnityWebRequest.Get(_assetBundleRootPath + info.AssetBundleName);
                        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                        request.downloadHandler = handler;
                        request.SendWebRequest();
                        while (!request.isDone)
                        {
                        }
                        if (!request.isNetworkError && !request.isHttpError)
                        {
                            if (handler.assetBundle)
                            {
                                asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                                if (asset)
                                {
                                    if (isPrefab)
                                    {
                                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                                    }
                                }
                                else
                                {
                                    GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                                }

                                if (IsCacheAssetBundle)
                                {
                                    if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                        _assetBundles.Add(info.AssetBundleName, handler.assetBundle);
                                }
                                else
                                {
                                    handler.assetBundle.Unload(false);
                                }
                            }
                            else
                            {
                                GlobalTools.LogError(string.Format("请求：{0} 未下载到AB包！", request.url));
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("请求：{0} 遇到网络错误：{1}！", request.url, request.error));
                        }
                        request.Dispose();
                        handler.Dispose();
                    }
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
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
                        GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get(_assetBundleRootPath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                    request.downloadHandler = handler;
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                    }
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                            if (asset)
                            {
                                if (isPrefab)
                                {
                                    asset = ClonePrefab(asset as GameObject, parent, isUI);
                                }
                            }
                            else
                            {
                                GlobalTools.LogError(string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                            }

                            if (IsCacheAssetBundle)
                            {
                                if (!_assetBundles.ContainsKey(info.AssetBundleName))
                                    _assetBundles.Add(info.AssetBundleName, handler.assetBundle);
                            }
                            else
                            {
                                handler.assetBundle.Unload(false);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("请求：{0} 未下载到AB包！", request.url));
                        }
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("请求：{0} 遇到网络错误：{1}！", request.url, request.error));
                    }
                    request.Dispose();
                    handler.Dispose();
                }
#endif
            }

            DateTime endTime = DateTime.Now;

            GlobalTools.LogInfo(string.Format("同步加载资源{0}[{1}模式]：\r\n{2}\r\n加载耗时：{3}秒", asset ? "成功" : "失败", Mode
                , Mode == ResourceLoadMode.Resource ? info.GetResourceFullPath() : info.GetAssetBundleFullPath(_assetBundleRootPath)
                , (endTime - beginTime).TotalSeconds));

            if (asset)
            {
                DataSetInfo dataSet = info as DataSetInfo;
                if (dataSet != null && dataSet.Data != null)
                {
                    asset.Cast<DataSet>().Fill(dataSet.Data);
                }

                return asset as T;
            }
            else
            {
                return null;
            }
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
        //加载AB包清单
        private void LoadAssetBundleManifest()
        {
            if (Mode == ResourceLoadMode.AssetBundle)
            {
                if (AssetBundleManifestName == "")
                {
                    GlobalTools.LogError("请设置资源管理模块的 Manifest Name 属性，为所有AB包提供依赖清单！");
                    return;
                }

#if UNITY_EDITOR
                if (!IsEditorMode)
                {
                    AssetBundle assetBundle = LoadAssetBundle(AssetBundleManifestName);
                    _assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    UnLoadAsset(AssetBundleManifestName);
                }
#else
                AssetBundle assetBundle = LoadAssetBundle(AssetBundleManifestName);
                _assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                UnLoadAsset(AssetBundleManifestName);
#endif
            }
        }
        //加载依赖AB包
        private void LoadDependenciesAssetBundle(string assetBundleName)
        {
            if (Mode == ResourceLoadMode.AssetBundle)
            {
                if (!IsEditorMode)
                {
                    string[] dependencies = _assetBundleManifest.GetAllDependencies(assetBundleName);
                    foreach (string item in dependencies)
                    {
                        if (_assetBundles.ContainsKey(item))
                        {
                            continue;
                        }

                        LoadAssetBundle(item);
                    }
                }
            }
        }
        //同步加载AB包
        private AssetBundle LoadAssetBundle(string assetBundleName)
        {
            AssetBundle assetBundle = null;

            if (_assetBundles.ContainsKey(assetBundleName))
            {
                assetBundle = _assetBundles[assetBundleName];
            }
            else
            {
                UnityWebRequest request = UnityWebRequest.Get(_assetBundleRootPath + assetBundleName);
                DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                request.downloadHandler = handler;
                request.SendWebRequest();
                while (!request.isDone)
                {
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (handler.assetBundle)
                    {
                        _assetBundles.Add(assetBundleName, handler.assetBundle);
                        assetBundle = handler.assetBundle;
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("请求：{0} 未下载到AB包！", request.url));
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("请求：{0} 遇到网络错误：{1}！", request.url, request.error));
                }
                request.Dispose();
                handler.Dispose();
            }

            return assetBundle;
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
