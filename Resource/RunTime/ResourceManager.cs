using System;
using System.Collections.Generic;
using UnityEngine;
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
            _isHttpPath = _assetBundlePath.Contains("http:");
        }

        public override void Termination()
        {
            UnLoadAllPrefab();
        }

        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        public void SetAssetBundlePath(string path)
        {
            _assetBundlePath = path;
            _isHttpPath = _assetBundlePath.Contains("http:");
        }

        /// <summary>
        /// 加载预制体（异步）
        /// </summary>
        public void LoadPrefab(PrefabInfo info, Action<GameObject, Transform> loadDoneAction, bool isUI = false)
        {
            StartCoroutine(LoadPrefabCoroutine(info, loadDoneAction, isUI));
        }
        private System.Collections.IEnumerator LoadPrefabCoroutine(PrefabInfo info, Action<GameObject, Transform> loadDoneAction, bool isUI)
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
                assetTem = AssetDatabase.LoadAssetAtPath<GameObject>(info.AssetBundlePath);
                yield return assetTem;
                if (assetTem)
                {
                    asset = Instantiate(assetTem) as GameObject;
                    yield return asset;
                }
                else
                {
                    GlobalTools.LogError("加载预制体失败：路径中不存在资源 " + info.AssetBundlePath);
                }
#else
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    assetTem = _assetBundles[info.AssetBundleName].LoadAsset(info.AssetBundlePath) as GameObject;
                    yield return assetTem;
                    if (assetTem)
                    {
                        asset = Instantiate(assetTem) as GameObject;
                        yield return asset;
                    }
                    else
                    {
                        GlobalTools.LogError("加载预制体失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetBundlePath);
                    }
                }
                else
                {
                    WWW www = new WWW((_isHttpPath ? "" : "file://") + _assetBundlePath + info.AssetBundleName);
                    yield return www;
                    if (www.assetBundle)
                    {
                        assetTem = www.assetBundle.LoadAsset(info.AssetBundlePath) as GameObject;
                        yield return assetTem;
                        if (assetTem)
                        {
                            asset = Instantiate(assetTem) as GameObject;
                            yield return asset;
                        }
                        else
                        {
                            GlobalTools.LogError("加载预制体失败：AB包 " + info.AssetBundleName + " 中不存在资源 " + info.AssetBundlePath);
                        }

                        if (IsCacheAssetBundle)
                        {
                            _assetBundles.Add(info.AssetBundleName, www.assetBundle);
                        }
                        else
                        {
                            www.assetBundle.Unload(false);
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("链接：" + _assetBundlePath + info.AssetBundleName + " 未下载到AB包！");
                    }
                    www.Dispose();
                    www = null;
                }
#endif
            }
            if (assetTem && asset)
            {
                loadDoneAction(asset, isUI ? assetTem.rectTransform() : assetTem.transform);
                assetTem = null;
                asset = null;
            }
        }

        /// <summary>
        /// 卸载预制体
        /// </summary>
        public void UnLoadPrefab(PrefabInfo info)
        {
            if (Mode == ResourceMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                if (_assetBundles.ContainsKey(info.AssetBundleName))
                {
                    _assetBundles[info.AssetBundleName].Unload(false);
                    _assetBundles.Remove(info.AssetBundleName);
                }
            }
        }

        /// <summary>
        /// 卸载所有预制体
        /// </summary>
        public void UnLoadAllPrefab()
        {
            if (Mode == ResourceMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                foreach (KeyValuePair<string, AssetBundle> asset in _assetBundles)
                {
                    asset.Value.Unload(false);
                }
                _assetBundles.Clear();
                AssetBundle.UnloadAllAssetBundles(false);
            }
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
