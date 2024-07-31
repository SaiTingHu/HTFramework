using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES_1_20
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
#endif

namespace HT.Framework
{
    /// <summary>
    /// Addressables资源管理器助手
    /// </summary>
    internal sealed class AddressablesHelper : IResourceHelper
    {
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
#if UNITY_ADDRESSABLES_1_20
        /// <summary>
        /// 已加载的所有场景【场景名称、场景】
        /// </summary>
        public Dictionary<string, SceneInstance> Scenes { get; private set; } = new Dictionary<string, SceneInstance>();
#endif

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
            AssetBundleRootPath = Application.persistentDataPath + "/";
            AssetBundleManifestName = manifestName;

            if (LoadMode != ResourceLoadMode.Addressables)
            {
                throw new HTFrameworkException(HTFrameworkModule.Resource, "AddressablesHelper：此资源加载助手仅支持 Addressables 模式，请更换助手!");
            }
        }
        /// <summary>
        /// 设置AssetBundle资源根路径
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
            return null;
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

            UnityEngine.Object asset = null;

#if UNITY_ADDRESSABLES_1_20
            var handle = Addressables.LoadAssetAsync<T>(info.AssetPath);
            while (!handle.IsDone)
            {
                onLoading?.Invoke(handle.PercentComplete);
                yield return null;
            }
            onLoading?.Invoke(1);
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                asset = handle.Result;
                if (asset)
                {
                    if (isPrefab)
                    {
                        asset = ClonePrefab(asset as GameObject, parent, isUI);
                    }
                }
                Addressables.Release(handle);
            }
            else
            {
                string msg = handle.OperationException.Message;
                Addressables.Release(handle);
                throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载资源 {info.AssetPath} 出错：{msg}");
            }
#else
            yield return null;
#endif

            float endTime = Time.realtimeSinceStartup;

            LogResourceDetail(info, asset != null, beginTime, endTime);

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

#if UNITY_ADDRESSABLES_1_20
            if (Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning($"加载场景失败：名为 {info.ResourcePath} 的场景已加载！");
                yield break;
            }
#endif

            float beginTime = Time.realtimeSinceStartup;

#if UNITY_ADDRESSABLES_1_20
            var handle = Addressables.LoadSceneAsync(info.AssetPath, LoadSceneMode.Additive);
            while (!handle.IsDone)
            {
                onLoading?.Invoke(handle.PercentComplete);
                yield return null;
            }
            onLoading?.Invoke(1);
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Scenes.Add(info.ResourcePath, handle.Result);
            }
            else
            {
                string msg = handle.OperationException.Message;
                Addressables.Release(handle);
                throw new HTFrameworkException(HTFrameworkModule.Resource, $"加载场景 {info.AssetPath} 出错：{msg}");
            }
#else
            yield return null;
#endif

            float endTime = Time.realtimeSinceStartup;

            LogSceneDetail(info, beginTime, endTime);

            onLoadDone?.Invoke();
        }

        /// <summary>
        /// 卸载AB包（异步）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAssetBundleAsync(string assetBundleName, bool unloadAllLoadedObjects)
        {
            yield return null;
        }
        /// <summary>
        /// 卸载所有AB包（异步）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllAssetBundleAsync(bool unloadAllLoadedObjects)
        {
            yield return null;
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

#if UNITY_ADDRESSABLES_1_20
            if (!Scenes.ContainsKey(info.ResourcePath))
            {
                Log.Warning($"卸载场景失败：名为 {info.ResourcePath} 的场景还未加载！");
                yield break;
            }

            yield return Addressables.UnloadSceneAsync(Scenes[info.ResourcePath]);

            Scenes.Remove(info.ResourcePath);
#endif
        }
        /// <summary>
        /// 卸载所有场景（异步）
        /// </summary>
        /// <returns>卸载协程迭代器</returns>
        public IEnumerator UnLoadAllSceneAsync()
        {
#if UNITY_ADDRESSABLES_1_20
            foreach (var scene in Scenes)
            {
                yield return Addressables.UnloadSceneAsync(scene.Value);
            }
            Scenes.Clear();
#else
            yield return null;
#endif
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
        /// 打印资源加载细节
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="isSucceed">加载是否成功</param>
        /// <param name="beginTime">加载开始时间</param>
        /// <param name="waitTime">加载等待时间</param>
        /// <param name="endTime">加载结束时间</param>
        private void LogResourceDetail(ResourceInfoBase info, bool isSucceed, float beginTime, float endTime)
        {
            if (!IsLogDetail)
                return;

#if UNITY_EDITOR
            string result = isSucceed ? "<color=cyan>成功</color>" : "<color=red>失败</color>";
            string path = Log.Hyperlink(info.AssetPath, info.AssetPath);
            string load = $"<color=cyan>{endTime - beginTime}</color>";
#else
            string result = isSucceed ? "成功" : "失败";
            string path = info.AssetPath;
            string load = (endTime - beginTime).ToString();
#endif
            Log.Info($"【加载资源{result}】资源路径：{path}，加载耗时：{load}秒。");
        }
        /// <summary>
        /// 打印场景加载细节
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="beginTime">加载开始时间</param>
        /// <param name="waitTime">加载等待时间</param>
        /// <param name="endTime">加载结束时间</param>
        private void LogSceneDetail(ResourceInfoBase info, float beginTime, float endTime)
        {
            if (!IsLogDetail)
                return;

#if UNITY_EDITOR
            string path = Log.Hyperlink(info.AssetPath, info.AssetPath);
            string load = $"<color=cyan>{endTime - beginTime}</color>";
#else
            string path = info.AssetPath;
            string load = (endTime - beginTime).ToString();
#endif
            Log.Info($"【加载场景完成】场景路径：{path}，加载耗时：{load}秒。");
        }
    }
}