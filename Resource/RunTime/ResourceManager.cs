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
        public ResourceMode Mode = ResourceMode.Resource;
        public bool IsCacheAssetBundle = false;

        //AssetBundle资源根路径
        private string _assetBundlePath;
        //AssetBundle资源根路径是否是网络路径
        private bool _isHttpPath = false;
        //缓存的所有AssetBundle资源
        private Dictionary<string, AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();

        public override void Initialization()
        {
            _assetBundlePath = Application.streamingAssetsPath;
            _isHttpPath = _assetBundlePath.Contains("http");
        }

        public override void Termination()
        {
            UnLoadAllAsset();
            ClearMemory();
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        public void SetAssetBundlePath(string path)
        {
            _assetBundlePath = path;
            _isHttpPath = _assetBundlePath.Contains("http");
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        public void LoadAsset<T>(AssetInfo info, Action<T> loadDoneAction) where T: UnityEngine.Object
        {
            StartCoroutine(LoadAssetCoroutine(info, loadDoneAction));
        }
        private System.Collections.IEnumerator LoadAssetCoroutine<T>(AssetInfo info, Action<T> loadDoneAction) where T : UnityEngine.Object
        {
            T asset = null;

            if (Mode == ResourceMode.Resource)
            {
                asset = Resources.Load<T>(info.ResourcePath);
                yield return asset;
                if (!asset)
                {
                    GlobalTools.LogError("加载资源失败：Resources文件夹中不存在" + typeof(T) + "资源 " + info.ResourcePath);
                }
            }
            else
            {
#if UNITY_EDITOR
                asset = AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
                yield return asset;
                if (!asset)
                {
                    GlobalTools.LogError("加载资源失败：路径中不存在资源 " + info.AssetPath);
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    yield return asset;
                    if (!asset)
                    {
                        GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get((_isHttpPath ? "" : "file://") + _assetBundlePath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
                    request.downloadHandler = handler;
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                            yield return asset;
                            if (!asset)
                            {
                                GlobalTools.LogError("加载资源失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                            }

                            if (IsCacheAssetBundle)
                            {
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
                loadDoneAction(asset);
            }

            asset = null;
        }

        /// <summary>
        /// 加载数据集（异步）
        /// </summary>
        public void LoadDataSet<T>(DataSetInfo info, Action<T> loadDoneAction) where T : DataSet
        {
            StartCoroutine(LoadDataSetCoroutine(info, loadDoneAction));
        }
        private System.Collections.IEnumerator LoadDataSetCoroutine<T>(DataSetInfo info, Action<T> loadDoneAction) where T : DataSet
        {
            T asset = null;

            if (Mode == ResourceMode.Resource)
            {
                asset = Resources.Load<T>(info.ResourcePath);
                yield return asset;
                if (!asset)
                {
                    GlobalTools.LogError("加载数据集失败：Resources文件夹中不存在" + typeof(T) + "数据集 " + info.ResourcePath);
                }
            }
            else
            {
#if UNITY_EDITOR
                asset = AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
                yield return asset;
                if (!asset)
                {
                    GlobalTools.LogError("加载数据集失败：路径中不存在数据集 " + info.AssetPath);
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    asset = _assetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    yield return asset;
                    if (!asset)
                    {
                        GlobalTools.LogError("加载数据集失败：AB包 " + info.AssetBundleName + " 中不存在数据集 " + info.AssetPath);
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get((_isHttpPath ? "" : "file://") + _assetBundlePath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
                    request.downloadHandler = handler;
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            asset = handler.assetBundle.LoadAsset<T>(info.AssetPath);
                            yield return asset;
                            if (!asset)
                            {
                                GlobalTools.LogError("加载数据集失败：AB包 " + info.AssetBundleName + " 中不存在数据集 " + info.AssetPath);
                            }

                            if (IsCacheAssetBundle)
                            {
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
                loadDoneAction(asset);
            }

            asset = null;
        }

        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        public void LoadPrefab(PrefabInfo info, Transform parent, Action<GameObject> loadDoneAction, bool isUI = false)
        {
            StartCoroutine(LoadPrefabCoroutine(info, parent, loadDoneAction, isUI));
        }
        private System.Collections.IEnumerator LoadPrefabCoroutine(PrefabInfo info, Transform parent, Action<GameObject> loadDoneAction, bool isUI)
        {
            GameObject assetTem = null;
            GameObject asset = null;

            if (Mode == ResourceMode.Resource)
            {
                assetTem = Resources.Load<GameObject>(info.ResourcePath);
                yield return assetTem;
                if (assetTem)
                {
                    asset = Instantiate(assetTem) as GameObject;
                    yield return asset;
                }
                else
                {
                    GlobalTools.LogError("加载预制体失败：Resources文件夹中不存在资源 " + info.ResourcePath);
                }
            }
            else
            {
#if UNITY_EDITOR
                assetTem = AssetDatabase.LoadAssetAtPath<GameObject>(info.AssetPath);
                yield return assetTem;
                if (assetTem)
                {
                    asset = Instantiate(assetTem) as GameObject;
                    yield return asset;
                }
                else
                {
                    GlobalTools.LogError("加载预制体失败：路径中不存在资源 " + info.AssetPath);
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    assetTem = _assetBundles[info.AssetBundleName].LoadAsset(info.AssetPath) as GameObject;
                    yield return assetTem;
                    if (assetTem)
                    {
                        asset = Instantiate(assetTem) as GameObject;
                        yield return asset;
                    }
                    else
                    {
                        GlobalTools.LogError("加载预制体失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                    }
                }
                else
                {
                    UnityWebRequest request = UnityWebRequest.Get((_isHttpPath ? "" : "file://") + _assetBundlePath + info.AssetBundleName);
                    DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
                    request.downloadHandler = handler;
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (handler.assetBundle)
                        {
                            assetTem = handler.assetBundle.LoadAsset(info.AssetPath) as GameObject;
                            yield return assetTem;
                            if (assetTem)
                            {
                                asset = Instantiate(assetTem) as GameObject;
                                yield return asset;
                            }
                            else
                            {
                                GlobalTools.LogError("加载预制体失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetPath);
                            }

                            if (IsCacheAssetBundle)
                            {
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
                if (parent)
                {
                    asset.transform.SetParent(parent);
                }

                if (isUI)
                {
                    asset.rectTransform().anchoredPosition3D = assetTem.rectTransform().anchoredPosition3D;
                    asset.rectTransform().sizeDelta = assetTem.rectTransform().sizeDelta;
                    asset.rectTransform().anchorMin = assetTem.rectTransform().anchorMin;
                    asset.rectTransform().anchorMax = assetTem.rectTransform().anchorMax;
                    asset.transform.localRotation = Quaternion.identity;
                    asset.transform.localScale = Vector3.one;
                }
                else
                {
                    asset.transform.localPosition = assetTem.transform.localPosition;
                    asset.transform.localRotation = Quaternion.identity;
                    asset.transform.localScale = Vector3.one;
                }

                asset.SetActive(false);
                loadDoneAction(asset);
            }

            assetTem = null;
            asset = null;
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
