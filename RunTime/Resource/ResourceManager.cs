using System;
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
        public ResourceMode Mode = ResourceMode.Resource;
        /// <summary>
        /// 是否缓存AB包
        /// </summary>
        public bool IsCacheAssetBundle = false;

        //AssetBundle资源根路径
        private string _assetBundlePath;
        //缓存的所有AssetBundle资源
        private Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();
        //单线下载中
        private bool _isLoading = false;
        //单线下载等待
        private WaitUntil _loadWait;

        public override void Initialization()
        {
            base.Initialization();

            _assetBundlePath = Application.streamingAssetsPath + "/";
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }

        public override void Termination()
        {
            base.Termination();

            UnLoadAllAsset();
            ClearMemory();
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        public void LoadAsset<T>(AssetInfo info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction) where T : UnityEngine.Object
        {
            StartCoroutine(LoadCoroutine(info, loadingAction, loadDoneAction));
        }
        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        public void LoadDataSet<T>(DataSetInfo info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction) where T : DataSet
        {
            StartCoroutine(LoadCoroutine(info, loadingAction, loadDoneAction));
        }
        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        public void LoadPrefab(PrefabInfo info, Transform parent, HTFAction<float> loadingAction, HTFAction<GameObject> loadDoneAction, bool isUI = false)
        {
            StartCoroutine(LoadCoroutine(info, loadingAction, loadDoneAction, true, parent, isUI));
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
            if (Mode == ResourceMode.Resource)
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
            if (Mode == ResourceMode.Resource)
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

        private System.Collections.IEnumerator LoadCoroutine<T>(ResourceInfoBase info, HTFAction<float> loadingAction, HTFAction<T> loadDoneAction, bool isPrefab = false, Transform parent = null, bool isUI = false) where T : UnityEngine.Object
        {
            if (!_isLoading)
            {
                _isLoading = true;
            }
            else
            {
                yield return _loadWait;
            }

            UnityEngine.Object asset = null;

            if (Mode == ResourceMode.Resource)
            {
                ResourceRequest request = Resources.LoadAsync<T>(info.ResourcePath);
                while (!request.isDone)
                {
                    if (loadingAction != null)
                        loadingAction(request.progress);
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
                if (loadingAction != null)
                    loadingAction(1);
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
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    if (loadingAction != null)
                        loadingAction(1);
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
                        if (loadingAction != null)
                            loadingAction(request.downloadProgress);
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
                    (asset as DataSet).Fill(dataSet.Data);
                }

                if (loadDoneAction != null)
                    loadDoneAction(asset as T);
            }
            asset = null;

            _isLoading = false;
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
    /// 资源模式
    /// </summary>
    public enum ResourceMode
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
