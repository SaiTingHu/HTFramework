using System;
using System.IO;
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
    internal sealed class DefaultResourceHelper : IResourceHelper
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
        /// 资源定位
        /// </summary>
        private Dictionary<string, ResourceLocation> _resourceLocations = new Dictionary<string, ResourceLocation>();

        /// <summary>
        /// 所属的内置模块
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
        /// 是否打印资源加载细节日志
        /// </summary>
        public bool IsLogDetail { get; private set; }
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
        public AssetBundleManifest Manifest { get; private set; }
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
        /// <param name="isLogDetail">是否打印资源加载细节日志</param>
        public void SetLoader(ResourceLoadMode loadMode, bool isEditorMode, string manifestName, bool isLogDetail)
        {
            LoadMode = loadMode;
            IsEditorMode = isEditorMode;
            IsLogDetail = isLogDetail;
#if UNITY_WEBGL
            AssetBundleRootPath = Application.streamingAssetsPath + "/AssetBundles/";
#else
            AssetBundleRootPath = Application.persistentDataPath + "/AssetBundles/";
#endif
            AssetBundleManifestName = manifestName;
            _loadWait = new WaitUntil(() => { return !_isLoading; });

            InitResourceLocation();

            if (LoadMode == ResourceLoadMode.Addressables)
            {
                throw new HTFrameworkException(HTFrameworkModule.Resource, "DefaultResourceHelper：缺省的资源加载助手不支持 Addressables 模式，请更换助手!");
            }
        }
        /// <summary>
        /// 设置AssetBundle资源根路径（仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            AssetBundleRootPath = path;

            InitResourceLocation();
        }
        /// <summary>
        /// 通过名称获取指定的AssetBundle（仅限 AssetBundle 模式）
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
                    if (isPrefab && asset is GameObject)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：Resources文件夹中不存在资源 {info.ResourcePath}！");
                }
            }
            else if (LoadMode == ResourceLoadMode.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    onLoading?.Invoke(1);
                    yield return null;

                    asset = AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
                    if (asset)
                    {
                        if (isPrefab && asset is GameObject)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：路径中不存在资源 {info.AssetPath}！");
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
                            if (isPrefab && asset is GameObject)
                            {
                                asset = ClonePrefab(asset as GameObject, parent, isUI);
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：AB包 {info.AssetBundleName} 中不存在资源 {info.AssetPath}！");
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
                        if (isPrefab && asset is GameObject)
                        {
                            asset = ClonePrefab(asset as GameObject, parent, isUI);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：AB包 {info.AssetBundleName} 中不存在资源 {info.AssetPath}！");
                    }
                }
#endif
            }

            float endTime = Time.realtimeSinceStartup;

            LogResourceDetail(info, asset != null, beginTime, waitTime, endTime);

            if (asset)
            {
                DataSetInfo dataSet = info as DataSetInfo;
                if (dataSet != null && dataSet.Data != null)
                {
                    DataSetBase dsb = asset as DataSetBase;
                    if (dsb) dsb.Fill(dataSet.Data);
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
            if (string.IsNullOrEmpty(info.ResourcePath))
            {
                Log.Warning($"加载场景失败：场景名称不能为空！");
                yield break;
            }

            if (Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning($"加载场景失败：名为 {info.ResourcePath} 的场景已加载！");
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
            else if (LoadMode == ResourceLoadMode.AssetBundle)
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

            LogSceneDetail(info, beginTime, waitTime, endTime);

            onLoadDone?.Invoke();

            //本线路加载资源结束
            _isLoading = false;
        }
        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="type">资源标记类型</param>
        /// <param name="onLoading">加载中事件</param>
        /// <param name="onLoadDone">加载完成事件</param>
        /// <param name="isPrefab">是否是加载预制体</param>
        /// <param name="parent">预制体加载完成后的父级</param>
        /// <param name="isUI">是否是加载UI</param>
        /// <returns>加载协程迭代器</returns>
        public IEnumerator LoadAssetAsync<T>(string location, Type type, HTFAction<float> onLoading, HTFAction<T> onLoadDone, bool isPrefab, Transform parent, bool isUI) where T : UnityEngine.Object
        {
            ResourceInfoBase info = null;
            if (type == typeof(AssetInfo)) info = new AssetInfo(null, null, null);
            else if (type == typeof(DataSetInfo)) info = new DataSetInfo(null, null, null, null);
            else if (type == typeof(PrefabInfo)) info = new PrefabInfo(null, null, null);

            if (info != null)
            {
                if (LoadMode == ResourceLoadMode.Resource)
                {
                    info.AssetBundleName = "default";
                    info.AssetPath = null;
                    info.ResourcePath = location;

                    yield return LoadAssetAsync(info, onLoading, onLoadDone, isPrefab, parent, isUI);
                }
                else if (LoadMode == ResourceLoadMode.AssetBundle)
                {
                    ResourceLocation rl = GetResourceLocation(location);
                    if (rl != null)
                    {
                        info.AssetBundleName = rl.ABName;
                        info.AssetPath = rl.AssetPath;
                        info.ResourcePath = rl.Name;

                        yield return LoadAssetAsync(info, onLoading, onLoadDone, isPrefab, parent, isUI);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：未找到资源定位Key【{location}】所指向的资源文件！");
                    }
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源失败：未知的资源信息标记类型 {type}！");
            }
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <param name="onLoading">加载中事件</param>
        /// <param name="onLoadDone">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public IEnumerator LoadSceneAsync(string location, HTFAction<float> onLoading, HTFAction onLoadDone)
        {
            SceneInfo info = new SceneInfo(null, null, null);
            if (LoadMode == ResourceLoadMode.Resource)
            {
                info.AssetBundleName = "default";
                info.AssetPath = location;
                info.ResourcePath = Path.GetFileNameWithoutExtension(location);

                yield return LoadSceneAsync(info, onLoading, onLoadDone);
            }
            else if (LoadMode == ResourceLoadMode.AssetBundle)
            {
                ResourceLocation rl = GetResourceLocation(location);
                if (rl != null)
                {
                    info.AssetBundleName = rl.ABName;
                    info.AssetPath = rl.AssetPath;
                    info.ResourcePath = rl.Name;

                    yield return LoadSceneAsync(info, onLoading, onLoadDone);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载场景失败：未找到资源定位Key【{location}】所指向的场景！");
                }
            }
        }

        /// <summary>
        /// 卸载资源（仅限 Addressables 模式）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        public void UnLoadAsset(string location)
        { 
        
        }
        /// <summary>
        /// 卸载AB包（异步，仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAssetBundleAsync(string assetBundleName, bool unloadAllLoadedObjects)
        {
            if (LoadMode == ResourceLoadMode.AssetBundle)
            {
                if (AssetBundles.ContainsKey(assetBundleName))
                {
                    yield return AssetBundles[assetBundleName].UnloadAsync(unloadAllLoadedObjects);
                    AssetBundles.Remove(assetBundleName);
                }
                if (AssetBundleHashs.ContainsKey(assetBundleName))
                {
                    AssetBundleHashs.Remove(assetBundleName);
                }
            }
        }
        /// <summary>
        /// 卸载所有AB包（异步，仅限 AssetBundle 模式）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllAssetBundleAsync(bool unloadAllLoadedObjects)
        {
            if (LoadMode == ResourceLoadMode.Resource)
            {
                yield return Resources.UnloadUnusedAssets();
            }
            else if (LoadMode == ResourceLoadMode.AssetBundle)
            {
                foreach (var assetBundle in AssetBundles)
                {
                    yield return assetBundle.Value.UnloadAsync(unloadAllLoadedObjects);
                }
                AssetBundles.Clear();
                AssetBundleHashs.Clear();
                AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
            }
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadSceneAsync(SceneInfo info)
        {
            if (string.IsNullOrEmpty(info.ResourcePath))
            {
                Log.Warning($"卸载场景失败：场景名称不能为空！");
                yield break;
            }

            if (!Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning($"卸载场景失败：名为 {info.ResourcePath} 的场景还未加载！");
                yield break;
            }

            Scenes.Remove(info.ResourcePath);
            yield return SceneManager.UnloadSceneAsync(info.ResourcePath);
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="location">资源定位Key，可以为资源路径、或资源名称</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadSceneAsync(string location)
        {
            SceneInfo info = new SceneInfo(null, null, null);
            if (LoadMode == ResourceLoadMode.Resource)
            {
                info.AssetBundleName = "default";
                info.AssetPath = location;
                info.ResourcePath = Path.GetFileNameWithoutExtension(location);

                yield return UnLoadSceneAsync(info);
            }
            else if (LoadMode == ResourceLoadMode.AssetBundle)
            {
                ResourceLocation rl = GetResourceLocation(location);
                if (rl != null)
                {
                    info.AssetBundleName = rl.ABName;
                    info.AssetPath = rl.AssetPath;
                    info.ResourcePath = rl.Name;

                    yield return UnLoadSceneAsync(info);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, $"卸载场景失败：未找到资源定位Key【{location}】所指向的场景！");
                }
            }
        }
        /// <summary>
        /// 卸载所有场景（异步）
        /// </summary>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllSceneAsync()
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
        /// 初始化资源定位
        /// </summary>
        private void InitResourceLocation()
        {
#if !UNITY_WEBGL
            if (LoadMode == ResourceLoadMode.AssetBundle)
            {
                _resourceLocations.Clear();
                string locPath = AssetBundleRootPath + "ResourceLocation.loc";
                if (File.Exists(locPath))
                {
                    string[] contents = File.ReadAllLines(locPath);
                    for (int i = 0; i < contents.Length; i++)
                    {
                        string[] datas = contents[i].Split('|');
                        if (datas.Length == 2)
                        {
                            ResourceLocation rl = new ResourceLocation()
                            {
                                ABName = datas[0],
                                Name = Path.GetFileNameWithoutExtension(datas[1]),
                                AssetPath = datas[1]
                            };
                            _resourceLocations.Add(rl.AssetPath, rl);
                        }
                    }
                }
            }
#endif
        }
        /// <summary>
        /// 获取资源定位
        /// </summary>
        /// <param name="location">资源定位Key</param>
        private ResourceLocation GetResourceLocation(string location)
        {
            ResourceLocation rl = null;
            if (_resourceLocations.ContainsKey(location))
            {
                rl = _resourceLocations[location];
            }
            else
            {
                foreach (var item in _resourceLocations)
                {
                    if (item.Value.Name == location)
                    {
                        rl = item.Value;
                        break;
                    }
                }
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    if (rl == null)
                    {
                        rl = new ResourceLocation()
                        {
                            ABName = "default",
                            Name = Path.GetFileNameWithoutExtension(location),
                            AssetPath = location
                        };
                        _resourceLocations.Add(rl.AssetPath, rl);
                    }
                }
#endif
            }
            return rl;
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

                    if (Manifest != null)
                    {
                        string[] dependencies = Manifest.GetAllDependencies(assetBundleName);
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

                if (Manifest != null)
                {
                    string[] dependencies = Manifest.GetAllDependencies(assetBundleName);
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
                if (Manifest == null)
                {
                    yield return LoadAssetBundleAsync(AssetBundleManifestName, true);

                    if (AssetBundles.ContainsKey(AssetBundleManifestName))
                    {
                        Manifest = AssetBundles[AssetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        yield return UnLoadAssetBundleAsync(AssetBundleManifestName, false);
                    }
                }
            }
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
#if UNITY_WEBGL
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    yield return request.SendWebRequest();
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            AssetBundles.Add(assetBundleName, bundle);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{request.url} 未下载到AB包！");
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{request.url} 遇到网络错误：{request.error}！");
                    }
                }
#else
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(AssetBundleRootPath + assetBundleName);
                yield return request;
                if (request.assetBundle)
                {
                    AssetBundles.Add(assetBundleName, request.assetBundle);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{AssetBundleRootPath + assetBundleName} 未加载到AB包！");
                }
#endif
            }
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
#if UNITY_WEBGL
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
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            AssetBundles.Add(assetBundleName, bundle);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{request.url} 未下载到AB包！");
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{request.url} 遇到网络错误：{request.error}！");
                    }
                }
#else
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(AssetBundleRootPath + assetBundleName);
                while (!request.isDone)
                {
                    onLoading?.Invoke(request.progress);
                    yield return null;
                }
                if (request.assetBundle)
                {
                    AssetBundles.Add(assetBundleName, request.assetBundle);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Resource, $"请求：{AssetBundleRootPath + assetBundleName} 未加载到AB包！");
                }
#endif
            }
            onLoading?.Invoke(1);
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
                Hash128 hash = Manifest.GetAssetBundleHash(assetBundleName);
                AssetBundleHashs.Add(assetBundleName, hash);
                return hash;
            }
        }
        /// <summary>
        /// 打印资源加载细节
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="isSucceed">加载是否成功</param>
        /// <param name="beginTime">加载开始时间</param>
        /// <param name="waitTime">加载等待时间</param>
        /// <param name="endTime">加载结束时间</param>
        private void LogResourceDetail(ResourceInfoBase info, bool isSucceed, float beginTime, float waitTime, float endTime)
        {
            if (!IsLogDetail)
                return;

#if UNITY_EDITOR
            string result = isSucceed ? "<color=cyan>成功</color>" : "<color=red>失败</color>";
            string path;
            if (LoadMode == ResourceLoadMode.Resource)
            {
                path = $"<color=cyan>Resources/{info.ResourcePath}</color>";
            }
            else
            {
                path = Log.Hyperlink(info.AssetPath, info.AssetPath);
            }
            string wait = $"<color=cyan>{waitTime - beginTime}</color>";
            string load = $"<color=cyan>{endTime - waitTime}</color>";
#else
            string result = isSucceed ? "成功" : "失败";
            string path;
            if (LoadMode == ResourceLoadMode.Resource)
            {
                path = $"Resources/{info.ResourcePath}";
            }
            else
            {
                path = info.AssetPath;
            }
            string wait = (waitTime - beginTime).ToString();
            string load = (endTime - waitTime).ToString();
#endif
            Log.Info($"【加载资源{result}】资源路径：{path}，等待耗时：{wait}秒，加载耗时：{load}秒。");
        }
        /// <summary>
        /// 打印场景加载细节
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="beginTime">加载开始时间</param>
        /// <param name="waitTime">加载等待时间</param>
        /// <param name="endTime">加载结束时间</param>
        private void LogSceneDetail(ResourceInfoBase info, float beginTime, float waitTime, float endTime)
        {
            if (!IsLogDetail)
                return;

#if UNITY_EDITOR
            string path = Log.Hyperlink(info.AssetPath, info.AssetPath);
            string wait = $"<color=cyan>{waitTime - beginTime}</color>";
            string load = $"<color=cyan>{endTime - waitTime}</color>";
#else
            string path = info.AssetPath;
            string wait = (waitTime - beginTime).ToString();
            string load = (endTime - waitTime).ToString();
#endif
            Log.Info($"【加载场景完成】场景路径：{path}，等待耗时：{wait}秒，加载耗时：{load}秒。");
        }

        /// <summary>
        /// 资源定位
        /// </summary>
        public class ResourceLocation
        {
            /// <summary>
            /// 资源所在的AB包名称
            /// </summary>
            public string ABName;
            /// <summary>
            /// 资源名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 资源路径
            /// </summary>
            public string AssetPath;
        }
    }
}