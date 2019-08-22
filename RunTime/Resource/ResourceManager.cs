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
        /// 资源加载模式
        /// </summary>
        public ResourceLoadMode Mode = ResourceLoadMode.Resource;
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        public bool IsEditorMode = true;
        /// <summary>
        /// 是否缓存AB包
        /// </summary>
        public bool IsCacheAssetBundle = true;

        //AssetBundle资源根路径
        private string _assetBundlePath;
        //缓存的所有AssetBundle包
        private Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();
        //单线下载中
        private bool _isLoading = false;
        //单线下载等待
        private WaitUntil _loadWait;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _assetBundlePath = Application.streamingAssetsPath + "/";
            _loadWait = new WaitUntil(() => { return !_isLoading; });
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
            StartCoroutine(LoadCoroutineAsync(info, loadingAction, loadDoneAction));
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
            StartCoroutine(LoadCoroutineAsync(info, loadingAction, loadDoneAction));
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
            StartCoroutine(LoadCoroutineAsync(info, loadingAction, loadDoneAction, true, parent, isUI));
        }
        /// <summary>
        /// 加载资源（同步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源配置信息</param>
        /// <returns>加载完成的资源</returns>
        public T LoadAssetSynch<T>(AssetInfo info) where T : UnityEngine.Object
        {
            return LoadSynch<T>(info);
        }
        /// <summary>
        /// 加载数据集（同步）
        /// </summary>
        /// <typeparam name="T">数据集类型</typeparam>
        /// <param name="info">数据集配置信息</param>
        /// <returns>加载完成的数据集</returns>
        public T LoadDataSetSynch<T>(DataSetInfo info) where T : DataSet
        {
            return LoadSynch<T>(info);
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
            return LoadSynch<GameObject>(info, true, parent, isUI);
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        public void SetAssetBundlePath(string path)
        {
            _assetBundlePath = path;
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

        private IEnumerator LoadCoroutineAsync<T>(ResourceInfoBase info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            if (_isLoading)
            {
                yield return _loadWait;
            }

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
                if (!asset)
                {
                    GlobalTools.LogError("加载资源失败：Resources文件夹中不存在 " + typeof(T) + " 资源 " + info.ResourcePath);
                }
                else
                {
                    if (isPrefab)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
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
                    if (!asset)
                    {
                        GlobalTools.LogError("加载资源失败：路径中不存在资源 " + info.AssetPath);
                    }
                    else
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                }
                else
                {
                    if (_assetBundles.ContainsKey(info.AssetBundleName))
                    {
                        loadingAction?.Invoke(1);
                        yield return null;

                        asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                        if (!asset)
                        {
                            GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                        }
                        else
                        {
                            if (isPrefab)
                            {
                                asset = ClonePrefab(asset as GameObject, parent, isUI);
                            }
                        }
                    }
                    else
                    {
                        UnityWebRequest request = UnityWebRequest.Get(_assetBundlePath + info.AssetBundleName);
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
                                if (!asset)
                                {
                                    GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                                }
                                else
                                {
                                    if (isPrefab)
                                    {
                                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                                    }
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
                                GlobalTools.LogError("请求：" + request.url + " 未下载到AB包！");
                            }
                        }
                        else
                        {
                            GlobalTools.LogError("请求：" + request.url + " 遇到网络错误：" + request.error);
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
                    if (!asset)
                    {
                        GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                    }
                    else
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get(_assetBundlePath + info.AssetBundleName);
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
                            if (!asset)
                            {
                                GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                            }
                            else
                            {
                                if (isPrefab)
                                {
                                    asset = ClonePrefab(asset as GameObject, parent, isUI);
                                }
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
                            GlobalTools.LogError("请求：" + request.url + " 未下载到AB包！");
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("请求：" + request.url + " 遇到网络错误：" + request.error);
                    }
                    request.Dispose();
                    handler.Dispose();
                }
#endif
            }

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
        private T LoadSynch<T>(ResourceInfoBase info, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            UnityEngine.Object asset = null;

            if (Mode == ResourceLoadMode.Resource)
            {
                asset = Resources.Load<T>(info.ResourcePath);
                if (!asset)
                {
                    GlobalTools.LogError("加载资源失败：Resources文件夹中不存在 " + typeof(T) + " 资源 " + info.ResourcePath);
                }
                else
                {
                    if (isPrefab)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    asset = AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
                    if (!asset)
                    {
                        GlobalTools.LogError("加载资源失败：路径中不存在资源 " + info.AssetPath);
                    }
                    else
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                }
                else
                {
                    if (_assetBundles.ContainsKey(info.AssetBundleName))
                    {
                        asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                        if (!asset)
                        {
                            GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                        }
                        else
                        {
                            if (isPrefab)
                            {
                                asset = ClonePrefab(asset as GameObject, parent, isUI);
                            }
                        }
                    }
                    else
                    {
                        UnityWebRequest request = UnityWebRequest.Get(_assetBundlePath + info.AssetBundleName);
                        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                        request.downloadHandler = handler;
                        request.SendWebRequest();
                        if (!request.isNetworkError && !request.isHttpError)
                        {
                            if (handler.assetBundle)
                            {
                                asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                                if (!asset)
                                {
                                    GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                                }
                                else
                                {
                                    if (isPrefab)
                                    {
                                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                                    }
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
                                GlobalTools.LogError("请求：" + request.url + " 未下载到AB包！");
                            }
                        }
                        else
                        {
                            GlobalTools.LogError("请求：" + request.url + " 遇到网络错误：" + request.error);
                        }
                        request.Dispose();
                        handler.Dispose();
                    }
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    if (!asset)
                    {
                        GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                    }
                    else
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get(_assetBundlePath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, 0);
                    request.downloadHandler = handler;
                    request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                            if (!asset)
                            {
                                GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                            }
                            else
                            {
                                if (isPrefab)
                                {
                                    asset = ClonePrefab(asset as GameObject, parent, isUI);
                                }
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
                            GlobalTools.LogError("请求：" + request.url + " 未下载到AB包！");
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("请求：" + request.url + " 遇到网络错误：" + request.error);
                    }
                    request.Dispose();
                    handler.Dispose();
                }
#endif
            }

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
