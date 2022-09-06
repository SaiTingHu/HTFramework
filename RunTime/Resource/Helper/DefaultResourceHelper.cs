﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 默认的资源管理器助手
    /// </summary>
    public sealed class DefaultResourceHelper : IResourceHelper
    {
        /// <summary>
        /// 单线下载中
        /// </summary>
        private bool _isLoading = false;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil _loadWait;

        /// <summary>
        /// 资源管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 当前的资源加载模式
        /// </summary>
        public ResourceLoadMode LoadMode { get; private set; }
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        public bool IsEditorMode { get; private set; }
        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        public string AssetBundleRootPath { get; private set; }
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        public string AssetBundleManifestName { get; private set; }
        /// <summary>
        /// 缓存的所有AssetBundle包【AB包名称、AB包】
        /// </summary>
        public Dictionary<string, AssetBundle> AssetBundles { get; private set; } = new Dictionary<string, AssetBundle>();
        /// <summary>
        /// 所有AssetBundle资源包清单
        /// </summary>
        public AssetBundleManifest AssetBundleManifest { get; private set; }
        /// <summary>
        /// 所有AssetBundle的Hash128值【AB包名称、Hash128值】
        /// </summary>
        public Dictionary<string, Hash128> AssetBundleHashs { get; private set; } = new Dictionary<string, Hash128>();
        /// <summary>
        /// 已加载的所有场景【场景名称、场景】
        /// </summary>
        public Dictionary<string, Scene> Scenes { get; private set; } = new Dictionary<string, Scene>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {

        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {

        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            UnLoadAllAsset(true);
            ClearMemory();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 设置加载器
        /// </summary>
        /// <param name="loadMode">加载模式</param>
        /// <param name="isEditorMode">是否是编辑器模式</param>
        /// <param name="manifestName">AB包清单名称</param>
        public void SetLoader(ResourceLoadMode loadMode, bool isEditorMode, string manifestName)
        {
            LoadMode = loadMode;
            IsEditorMode = isEditorMode;
            AssetBundleRootPath = Application.persistentDataPath + "/";
            AssetBundleManifestName = manifestName;
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            AssetBundleRootPath = path;
        }
        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            if (AssetBundles.ContainsKey(assetBundleName))
            {
                return AssetBundles[assetBundleName];
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
        /// <param name="info">资源信息标记</param>
        /// <param name="onLoading">加载中事件</param>
        /// <param name="onLoadDone">加载完成事件</param>
        /// <param name="isPrefab">是否是加载预制体</param>
        /// <param name="parent">预制体加载完成后的父级</param>
        /// <param name="isUI">是否是加载UI</param>
        /// <returns>加载协程迭代器</returns>
        public IEnumerator LoadAssetAsync<T>(ResourceInfoBase info, HTFAction<float> onLoading, HTFAction<T> onLoadDone, bool isPrefab, Transform parent, bool isUI) where T : UnityEngine.Object
        {
            float beginTime = Time.realtimeSinceStartup;

            //单线加载，如果其他地方在加载资源，则等待
            if (_isLoading)
            {
                yield return _loadWait;
            }

            //轮到本线路加载资源
            _isLoading = true;

            //等待相关依赖资源的加载
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);

            float waitTime = Time.realtimeSinceStartup;

            UnityEngine.Object asset = null;

            if (LoadMode == ResourceLoadMode.Resource)
            {
                ResourceRequest request = Resources.LoadAsync<T>(info.ResourcePath);
                while (!request.isDone)
                {
                    onLoading?.Invoke(request.progress);
                    yield return null;
                }
                onLoading?.Invoke(1);
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
                    throw new HTFrameworkException(HTFrameworkModule.Resource, string.Format("加载资源失败：Resources文件夹中不存在资源 {0}！", info.ResourcePath));
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    onLoading?.Invoke(1);
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
                        throw new HTFrameworkException(HTFrameworkModule.Resource, string.Format("加载资源失败：路径中不存在资源 {0}！", info.AssetPath));
                    }
                }
                else
                {
                    yield return LoadAssetBundleAsync(info.AssetBundleName, onLoading);

                    if (AssetBundles.ContainsKey(info.AssetBundleName))
                    {
                        asset = AssetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                        if (asset)
                        {
                            if (isPrefab)
                            {
                                asset = ClonePrefab(asset as GameObject, parent, isUI);
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                        }
                    }
                }
#else
                yield return LoadAssetBundleAsync(info.AssetBundleName, onLoading);

                if (AssetBundles.ContainsKey(info.AssetBundleName))
                {
                    asset = AssetBundles[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    if (asset)
                    {
                        if (isPrefab)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, string.Format("加载资源失败：AB包 {0} 中不存在资源 {1}！", info.AssetBundleName, info.AssetPath));
                    }
                }
#endif
            }

            float endTime = Time.realtimeSinceStartup;

            Log.Info(string.Format("异步加载资源{0}[{1}模式]：\r\n{2}\r\n等待耗时：{3}秒  加载耗时：{4}秒"
                , asset ? "成功" : "失败"
                , LoadMode.ToString()
                , LoadMode == ResourceLoadMode.Resource ? info.GetResourceFullPath() : info.GetAssetBundleFullPath(AssetBundleRootPath)
                , waitTime - beginTime
                , endTime - waitTime));

            if (asset)
            {
                DataSetInfo dataSet = info as DataSetInfo;
                if (dataSet != null && dataSet.Data != null)
                {
                    asset.Cast<DataSetBase>().Fill(dataSet.Data);
                }

                onLoadDone?.Invoke(asset as T);
            }
            else
            {
                onLoadDone?.Invoke(null);
            }
            asset = null;

            //本线路加载资源结束
            _isLoading = false;
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="onLoading">加载中事件</param>
        /// <param name="onLoadDone">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public IEnumerator LoadSceneAsync(SceneInfo info, HTFAction<float> onLoading, HTFAction onLoadDone)
        {
            if (Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning(string.Format("加载场景失败：名为 {0} 的场景已加载！", info.ResourcePath));
                yield break;
            }

            float beginTime = Time.realtimeSinceStartup;

            //单线加载，如果其他地方在加载资源，则等待
            if (_isLoading)
            {
                yield return _loadWait;
            }

            //轮到本线路加载资源
            _isLoading = true;

            //等待相关依赖资源的加载
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);

            float waitTime = Time.realtimeSinceStartup;

            if (LoadMode == ResourceLoadMode.Resource)
            {
                if (Main.Current.IsAllowSceneAddBuild)
                {
                    Scene scene = SceneManager.GetSceneByPath(info.AssetPath);
                    Scenes.Add(info.ResourcePath, scene);
                    AsyncOperation ao = SceneManager.LoadSceneAsync(info.ResourcePath, LoadSceneMode.Additive);
                    while (!ao.isDone)
                    {
                        onLoading?.Invoke(ao.progress);
                        yield return null;
                    }
                    onLoading?.Invoke(1);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, "加载场景失败：若要在Resource模式下加载其他场景，请在Main模块的检视面板勾选 Allow Scene Add Build！");
                }
            }
            else
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    LoadSceneParameters parameters = new LoadSceneParameters()
                    {
                        loadSceneMode = LoadSceneMode.Additive,
                        localPhysicsMode = LocalPhysicsMode.None
                    };
                    Scene scene = SceneManager.GetSceneByPath(info.AssetPath);
                    Scenes.Add(info.ResourcePath, scene);
                    AsyncOperation ao = EditorSceneManager.LoadSceneAsyncInPlayMode(info.AssetPath, parameters);
                    while (!ao.isDone)
                    {
                        onLoading?.Invoke(ao.progress);
                        yield return null;
                    }
                    onLoading?.Invoke(1);
                }
                else
                {
                    Scene scene = SceneManager.GetSceneByPath(info.AssetPath);
                    Scenes.Add(info.ResourcePath, scene);
                    yield return LoadAssetBundleAsync(info.AssetBundleName, onLoading);
                    AsyncOperation ao = SceneManager.LoadSceneAsync(info.ResourcePath, LoadSceneMode.Additive);
                    while (!ao.isDone)
                    {
                        onLoading?.Invoke(ao.progress);
                        yield return null;
                    }
                    onLoading?.Invoke(1);
                }
#else
                Scene scene = SceneManager.GetSceneByPath(info.AssetPath);
                Scenes.Add(info.ResourcePath, scene);
                yield return LoadAssetBundleAsync(info.AssetBundleName, onLoading);
                AsyncOperation ao = SceneManager.LoadSceneAsync(info.ResourcePath, LoadSceneMode.Additive);
                while (!ao.isDone)
                {
                    onLoading?.Invoke(ao.progress);
                    yield return null;
                }
                onLoading?.Invoke(1);
#endif
            }

            float endTime = Time.realtimeSinceStartup;

            Log.Info(string.Format("异步加载场景完成[{0}模式]：{1}\r\n等待耗时：{2}秒  加载耗时：{3}秒"
                , LoadMode.ToString()
                , info.ResourcePath
                , waitTime - beginTime
                , endTime - waitTime));

            onLoadDone?.Invoke();

            //本线路加载资源结束
            _isLoading = false;
        }
        /// <summary>
        /// 卸载资源（异步，Resource模式：卸载未使用的资源，AssetBundle模式：卸载AB包）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (LoadMode == ResourceLoadMode.Resource)
            {
                yield return Resources.UnloadUnusedAssets();
            }
            else
            {
                if (AssetBundles.ContainsKey(assetBundleName))
                {
                    AssetBundles[assetBundleName].Unload(unloadAllLoadedObjects);
                    AssetBundles.Remove(assetBundleName);
                }
                if (AssetBundleHashs.ContainsKey(assetBundleName))
                {
                    AssetBundleHashs.Remove(assetBundleName);
                }
                yield return null;
            }
        }
        /// <summary>
        /// 卸载所有资源（异步，Resource模式：卸载未使用的资源，AssetBundle模式：卸载AB包）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            if (LoadMode == ResourceLoadMode.Resource)
            {
                yield return Resources.UnloadUnusedAssets();
            }
            else
            {
                foreach (var assetBundle in AssetBundles)
                {
                    assetBundle.Value.Unload(unloadAllLoadedObjects);
                }
                AssetBundles.Clear();
                AssetBundleHashs.Clear();
                AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
                yield return null;
            }
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadScene(SceneInfo info)
        {
            if (!Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning(string.Format("卸载场景失败：名为 {0} 的场景还未加载！", info.ResourcePath));
                yield break;
            }

            Scenes.Remove(info.ResourcePath);
            yield return SceneManager.UnloadSceneAsync(info.ResourcePath);
        }
        /// <summary>
        /// 卸载所有场景（异步）
        /// </summary>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllScene()
        {
            foreach (var scene in Scenes)
            {
                yield return SceneManager.UnloadSceneAsync(scene.Key);
            }
            Scenes.Clear();
        }
        /// <summary>
        /// 清理内存，释放空闲内存（异步）
        /// </summary>
        /// <returns>协程迭代器</returns>
        public IEnumerator ClearMemory()
        {
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// 克隆预制体
        /// </summary>
        /// <param name="prefabTem">预制体模板</param>
        /// <param name="parent">克隆后的父级</param>
        /// <param name="isUI">是否是UI</param>
        /// <returns>克隆后的预制体</returns>
        private GameObject ClonePrefab(GameObject prefabTem, Transform parent, bool isUI)
        {
            GameObject prefab = UnityEngine.Object.Instantiate(prefabTem);

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
        /// <summary>
        /// 异步加载依赖AB包
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator LoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (LoadMode == ResourceLoadMode.AssetBundle)
            {
#if UNITY_EDITOR
                if (!IsEditorMode)
                {
                    yield return LoadAssetBundleManifestAsync();

                    if (AssetBundleManifest != null)
                    {
                        string[] dependencies = AssetBundleManifest.GetAllDependencies(assetBundleName);
                        foreach (string item in dependencies)
                        {
                            if (AssetBundles.ContainsKey(item))
                            {
                                continue;
                            }

                            yield return LoadAssetBundleAsync(item);
                        }
                    }
                }
#else
                yield return LoadAssetBundleManifestAsync();

                if (AssetBundleManifest != null)
                {
                    string[] dependencies = AssetBundleManifest.GetAllDependencies(assetBundleName);
                    foreach (string item in dependencies)
                    {
                        if (AssetBundles.ContainsKey(item))
                        {
                            continue;
                        }

                        yield return LoadAssetBundleAsync(item);
                    }
                }
#endif
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator LoadAssetBundleManifestAsync()
        {
            if (string.IsNullOrEmpty(AssetBundleManifestName))
            {
                throw new HTFrameworkException(HTFrameworkModule.Resource, "请设置资源管理模块的 Manifest Name 属性，为所有AB包提供依赖清单！");
            }
            else
            {
                if (AssetBundleManifest == null)
                {
                    yield return LoadAssetBundleAsync(AssetBundleManifestName, true);

                    if (AssetBundles.ContainsKey(AssetBundleManifestName))
                    {
                        AssetBundleManifest = AssetBundles[AssetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnLoadAsset(AssetBundleManifestName);
                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="isManifest">是否是加载清单</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator LoadAssetBundleAsync(string assetBundleName, bool isManifest = false)
        {
            if (!AssetBundles.ContainsKey(assetBundleName))
            {
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    yield return request.SendWebRequest();
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            AssetBundles.Add(assetBundleName, bundle);
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
        /// <summary>
        /// 异步加载AB包（提供进度回调）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="onLoading">加载中事件</param>
        /// <param name="isManifest">是否是加载清单</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator LoadAssetBundleAsync(string assetBundleName, HTFAction<float> onLoading, bool isManifest = false)
        {
            if (!AssetBundles.ContainsKey(assetBundleName))
            {
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        onLoading?.Invoke(request.downloadProgress);
                        yield return null;
                    }
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            AssetBundles.Add(assetBundleName, bundle);
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
            onLoading?.Invoke(1);
            yield return null;
        }
        /// <summary>
        /// 获取AB包的hash值
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <returns>hash值</returns>
        private Hash128 GetAssetBundleHash(string assetBundleName)
        {
            if (AssetBundleHashs.ContainsKey(assetBundleName))
            {
                return AssetBundleHashs[assetBundleName];
            }
            else
            {
                Hash128 hash = AssetBundleManifest.GetAssetBundleHash(assetBundleName);
                AssetBundleHashs.Add(assetBundleName, hash);
                return hash;
            }
        }
    }
}