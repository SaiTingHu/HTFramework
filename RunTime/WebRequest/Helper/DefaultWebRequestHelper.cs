using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 默认的Web请求管理器助手
    /// </summary>
    internal sealed class DefaultWebRequestHelper : IWebRequestHelper
    {
        private WebRequestManager _module;

        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有网络接口
        /// </summary>
        public Dictionary<string, WebInterfaceBase> WebInterfaces { get; private set; } = new Dictionary<string, WebInterfaceBase>();
        /// <summary>
        /// 是否已连接到因特网
        /// </summary>
        public bool IsConnectedInternet
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            _module = Module as WebRequestManager;
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
            ClearInterface();
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
        /// 注册接口（获取 string）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 string 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        /// <param name="errorHandle">出现错误时的处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<string> handler, HTFAction offlineHandle, HTFAction<long, string> errorHandle)
        {
            if (!WebInterfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetString wi = Main.m_ReferencePool.Spawn<WebInterfaceGetString>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.ErrorHandler = errorHandle;
                wi.Handler = handler;
                WebInterfaces.Add(interfaceName, wi);
            }
            else
            {
                Log.Error($"注册接口失败：已存在名为 {interfaceName} 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 Texture2D）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 Texture2D 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        /// <param name="errorHandle">出现错误时的处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<Texture2D> handler, HTFAction offlineHandle, HTFAction<long, string> errorHandle)
        {
            if (!WebInterfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetTexture2D wi = Main.m_ReferencePool.Spawn<WebInterfaceGetTexture2D>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.ErrorHandler = errorHandle;
                wi.Handler = handler;
                WebInterfaces.Add(interfaceName, wi);
            }
            else
            {
                Log.Error($"注册接口失败：已存在名为 {interfaceName} 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 AudioClip）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 AudioClip 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        /// <param name="errorHandle">出现错误时的处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<AudioClip> handler, HTFAction offlineHandle, HTFAction<long, string> errorHandle)
        {
            if (!WebInterfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetAudioClip wi = Main.m_ReferencePool.Spawn<WebInterfaceGetAudioClip>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.ErrorHandler = errorHandle;
                wi.Handler = handler;
                WebInterfaces.Add(interfaceName, wi);
            }
            else
            {
                Log.Error($"注册接口失败：已存在名为 {interfaceName} 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 File）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="loadingHandler">下载过程中回调</param>
        /// <param name="finishedHandler">下载完成回调</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        /// <param name="errorHandle">出现错误时的处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, string savePath, HTFAction<float> loadingHandler, HTFAction<bool> finishedHandler, HTFAction offlineHandle, HTFAction<long, string> errorHandle)
        {
            if (!WebInterfaces.ContainsKey(interfaceName))
            {
                WebInterfaceDownloadFile wi = Main.m_ReferencePool.Spawn<WebInterfaceDownloadFile>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.ErrorHandler = errorHandle;
                wi.LoadingHandler = loadingHandler;
                wi.FinishedHandler = finishedHandler;
                wi.Path = savePath;
                WebInterfaces.Add(interfaceName, wi);
            }
            else
            {
                Log.Error($"注册接口失败：已存在名为 {interfaceName} 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（提交 表单）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="errorHandle">出现错误时的处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<long, string> errorHandle)
        {
            if (!WebInterfaces.ContainsKey(interfaceName))
            {
                WebInterfacePost wi = Main.m_ReferencePool.Spawn<WebInterfacePost>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.ErrorHandler = errorHandle;
                WebInterfaces.Add(interfaceName, wi);
            }
            else
            {
                Log.Error($"注册接口失败：已存在名为 {interfaceName} 的网络接口！");
            }
        }
        /// <summary>
        /// 通过名称获取接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>网络接口</returns>
        public WebInterfaceBase GetInterface(string interfaceName)
        {
            if (WebInterfaces.ContainsKey(interfaceName))
            {
                return WebInterfaces[interfaceName];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 是否存在指定名称的接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistInterface(string interfaceName)
        {
            return WebInterfaces.ContainsKey(interfaceName);
        }
        /// <summary>
        /// 取消注册接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        public void UnRegisterInterface(string interfaceName)
        {
            if (WebInterfaces.ContainsKey(interfaceName))
            {
                Main.m_ReferencePool.Despawn(WebInterfaces[interfaceName]);
                WebInterfaces.Remove(interfaceName);
            }
        }
        /// <summary>
        /// 清空所有接口
        /// </summary>
        public void ClearInterface()
        {
            foreach (var inter in WebInterfaces)
            {
                Main.m_ReferencePool.Despawn(inter.Value);
            }
            WebInterfaces.Clear();
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendRequest(string interfaceName, params string[] parameter)
        {
            if (WebInterfaces.ContainsKey(interfaceName))
            {
                if (_module.IsOfflineState || WebInterfaces[interfaceName].IsOffline || !IsConnectedInternet)
                {
                    WebInterfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendRequestCoroutine(WebInterfaces[interfaceName], parameter));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, $"发起网络请求失败：不存在名为 {interfaceName} 的网络接口！");
            }
            return null;
        }
        private IEnumerator SendRequestCoroutine(WebInterfaceBase wif, params string[] parameter)
        {
            StringToolkit.BeginConcat();
            StringToolkit.Concat(wif.Url);
            if (parameter.Length > 0)
            {
                StringToolkit.Concat("?");
                StringToolkit.Concat(parameter[0]);
            }
            for (int i = 1; i < parameter.Length; i++)
            {
                StringToolkit.Concat("&");
                StringToolkit.Concat(parameter[i]);
            }
            string url = StringToolkit.EndConcat();

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                DateTime begin = DateTime.Now;

                if (wif.RequestHeaders != null)
                {
                    foreach (var header in wif.RequestHeaders)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                wif.OnSetDownloadHandler(request);
                yield return request.SendWebRequest();

                DateTime end = DateTime.Now;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    LogSuccessDetail(wif, request.downloadHandler, url, begin, end);

                    wif.OnRequestFinished(request.downloadHandler);
                }
                else
                {
                    LogFailDetail(wif, request.error, url, begin, end);

                    wif.OnRequestFinished(null);
                    wif.ErrorHandler?.Invoke(request.responseCode, request.error);
                }
            }
        }
        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="form">参数表单</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendRequest(string interfaceName, WWWForm form)
        {
            if (WebInterfaces.ContainsKey(interfaceName))
            {
                if (_module.IsOfflineState || WebInterfaces[interfaceName].IsOffline || !IsConnectedInternet)
                {
                    WebInterfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendRequestCoroutine(WebInterfaces[interfaceName], form));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, $"发起网络请求失败：不存在名为 {interfaceName} 的网络接口！");
            }
            return null;
        }
        private IEnumerator SendRequestCoroutine(WebInterfaceBase wif, WWWForm form)
        {
            string url = wif.Url;

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                DateTime begin = DateTime.Now;

                if (wif.RequestHeaders != null)
                {
                    foreach (var header in wif.RequestHeaders)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                wif.OnSetDownloadHandler(request);
                yield return request.SendWebRequest();

                DateTime end = DateTime.Now;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    LogSuccessDetail(wif, request.downloadHandler, url, begin, end);

                    wif.OnRequestFinished(request.downloadHandler);
                }
                else
                {
                    LogFailDetail(wif, request.error, url, begin, end);

                    wif.OnRequestFinished(null);
                    wif.ErrorHandler?.Invoke(request.responseCode, request.error);
                }
            }
        }
        /// <summary>
        /// 发起提交Json数据请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="jsonData">json数据</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendPostJson(string interfaceName, string jsonData)
        {
            if (WebInterfaces.ContainsKey(interfaceName))
            {
                if (_module.IsOfflineState || WebInterfaces[interfaceName].IsOffline || !IsConnectedInternet)
                {
                    WebInterfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendPostJsonCoroutine(WebInterfaces[interfaceName], jsonData));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, $"发起提交Json数据请求失败：不存在名为 {interfaceName} 的网络接口！");
            }
            return null;
        }
        private IEnumerator SendPostJsonCoroutine(WebInterfaceBase wif, string jsonData)
        {
            string url = wif.Url;
            using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData, "application/json"))
            {
                DateTime begin = DateTime.Now;

                if (wif.RequestHeaders != null)
                {
                    foreach (var header in wif.RequestHeaders)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                wif.OnSetDownloadHandler(request);
                yield return request.SendWebRequest();

                DateTime end = DateTime.Now;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    LogSuccessDetail(wif, request.downloadHandler, url, begin, end);

                    wif.OnRequestFinished(request.downloadHandler);
                }
                else
                {
                    LogFailDetail(wif, request.error, url, begin, end);

                    wif.OnRequestFinished(null);
                    wif.ErrorHandler?.Invoke(request.responseCode, request.error);
                }
            }
        }
        /// <summary>
        /// 发起下载文件请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendDownloadFile(string interfaceName, params string[] parameter)
        {
            if (WebInterfaces.ContainsKey(interfaceName) && WebInterfaces[interfaceName] is WebInterfaceDownloadFile)
            {
                if (_module.IsOfflineState || WebInterfaces[interfaceName].IsOffline || !IsConnectedInternet)
                {
                    WebInterfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendDownloadFileCoroutine(WebInterfaces[interfaceName] as WebInterfaceDownloadFile, parameter));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, $"发起下载文件请求失败：不存在名为 {interfaceName} 的文件请求接口！");
            }
            return null;
        }
        private IEnumerator SendDownloadFileCoroutine(WebInterfaceDownloadFile wif, params string[] parameter)
        {
            StringToolkit.BeginConcat();
            StringToolkit.Concat(wif.Url);
            if (parameter.Length > 0)
            {
                StringToolkit.Concat("?");
                StringToolkit.Concat(parameter[0]);
            }
            for (int i = 1; i < parameter.Length; i++)
            {
                StringToolkit.Concat("&");
                StringToolkit.Concat(parameter[i]);
            }
            string url = StringToolkit.EndConcat();
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                DateTime begin = DateTime.Now;

                if (wif.RequestHeaders != null)
                {
                    foreach (var header in wif.RequestHeaders)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                wif.OnSetDownloadHandler(request);
                request.SendWebRequest();
                while (!request.isDone)
                {
                    wif.OnLoading(request.downloadProgress);
                    yield return null;
                }
                wif.OnLoading(1f);

                DateTime end = DateTime.Now;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    LogLoadFileSuccessDetail(wif, url, begin, end);

                    wif.OnFinished(true);
                }
                else
                {
                    LogLoadFileFailDetail(wif, request.error, url, begin, end);

                    wif.OnFinished(false);
                    wif.ErrorHandler?.Invoke(request.responseCode, request.error);
                }
            }
        }

        /// <summary>
        /// 打印网络请求细节（请求成功）
        /// </summary>
        /// <param name="wif">网络接口</param>
        /// <param name="handler">请求下载处理器</param>
        /// <param name="url">请求链接</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void LogSuccessDetail(WebInterfaceBase wif, DownloadHandler handler, string url, DateTime beginTime, DateTime endTime)
        {
            if (!_module.IsLogDetail)
                return;

#if UNITY_EDITOR
            string apiStr = $"<color=cyan>{wif.Name}</color>";
            string urlStr = $"<color=cyan>{url}</color>";
            string dataStr = $"<color=cyan>{handler.data.Length}</color>";
            string begin = $"<color=cyan>{beginTime.ToString("mm:ss:fff")}</color>";
            string end = $"<color=cyan>{endTime.ToString("mm:ss:fff")}</color>";
            string content = wif.OnGetDownloadString(handler);
#else
            string apiStr = wif.Name;
            string urlStr = url;
            string dataStr = handler.data.Length.ToString();
            string begin = beginTime.ToString("mm:ss:fff");
            string end = endTime.ToString("mm:ss:fff");
            string content = wif.OnGetDownloadString(handler);
#endif
            Log.Info($"【发起网络请求】接口：{apiStr}，URL：{urlStr}，收到回复：{dataStr}字节，开始时间：{begin}，结束时间：{end}，回复内容：{content}。");
        }
        /// <summary>
        /// 打印网络请求细节（请求失败）
        /// </summary>
        /// <param name="wif">网络接口</param>
        /// <param name="error">错误信息</param>
        /// <param name="url">请求链接</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void LogFailDetail(WebInterfaceBase wif, string error, string url, DateTime beginTime, DateTime endTime)
        {
            if (!_module.IsLogDetail)
                return;

#if UNITY_EDITOR
            string apiStr = $"<color=cyan>{wif.Name}</color>";
            string urlStr = $"<color=cyan>{url}</color>";
            string errorStr = $"<color=cyan>{error}</color>";
            string begin = $"<color=cyan>{beginTime.ToString("mm:ss:fff")}</color>";
            string end = $"<color=cyan>{endTime.ToString("mm:ss:fff")}</color>";
#else
            string apiStr = wif.Name;
            string urlStr = url;
            string errorStr = error;
            string begin = beginTime.ToString("mm:ss:fff");
            string end = endTime.ToString("mm:ss:fff");
#endif
            Log.Error($"【发起网络请求】接口：{apiStr}，URL：{urlStr}，请求失败：{errorStr}，开始时间：{begin}，结束时间：{end}。");
        }
        /// <summary>
        /// 打印下载文件细节（请求成功）
        /// </summary>
        /// <param name="wif">网络接口</param>
        /// <param name="url">请求链接</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void LogLoadFileSuccessDetail(WebInterfaceDownloadFile wif, string url, DateTime beginTime, DateTime endTime)
        {
            if (!_module.IsLogDetail)
                return;

#if UNITY_EDITOR
            string apiStr = $"<color=cyan>{wif.Name}</color>";
            string urlStr = $"<color=cyan>{url}</color>";
            string path = $"<color=cyan>{wif.Path}</color>";
            string begin = $"<color=cyan>{beginTime.ToString("mm:ss:fff")}</color>";
            string end = $"<color=cyan>{endTime.ToString("mm:ss:fff")}</color>";
#else
            string apiStr = wif.Name;
            string urlStr = url;
            string path = wif.Path;
            string begin = beginTime.ToString("mm:ss:fff");
            string end = endTime.ToString("mm:ss:fff");
#endif
            Log.Info($"【发起下载文件请求】接口：{apiStr}，URL：{urlStr}，保存路径：{path}，开始时间：{begin}，结束时间：{end}。");
        }
        /// <summary>
        /// 打印下载文件细节（请求失败）
        /// </summary>
        /// <param name="wif">网络接口</param>
        /// <param name="error">错误信息</param>
        /// <param name="url">请求链接</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        private void LogLoadFileFailDetail(WebInterfaceDownloadFile wif, string error, string url, DateTime beginTime, DateTime endTime)
        {
            if (!_module.IsLogDetail)
                return;

#if UNITY_EDITOR
            string apiStr = $"<color=cyan>{wif.Name}</color>";
            string urlStr = $"<color=cyan>{url}</color>";
            string errorStr = $"<color=cyan>{error}</color>";
            string begin = $"<color=cyan>{beginTime.ToString("mm:ss:fff")}</color>";
            string end = $"<color=cyan>{endTime.ToString("mm:ss:fff")}</color>";
#else
            string apiStr = wif.Name;
            string urlStr = url;
            string errorStr = error;
            string begin = beginTime.ToString("mm:ss:fff");
            string end = endTime.ToString("mm:ss:fff");
#endif
            Log.Error($"【发起下载文件请求】接口：{apiStr}，URL：{urlStr}，下载失败：{errorStr}，开始时间：{begin}，结束时间：{end}。");
        }
    }
}