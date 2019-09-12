using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// Web请求管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WebRequestManager : ModuleManagerBase
    {
        /// <summary>
        /// 当前是否是离线状态
        /// </summary>
        public bool IsOfflineState = false;
        /// <summary>
        /// 下载音频的格式
        /// </summary>
        public AudioType DownloadAudioType = AudioType.WAV;

        private Dictionary<string, WebInterfaceBase> _interfaces = new Dictionary<string, WebInterfaceBase>();
        
        public override void OnTermination()
        {
            base.OnTermination();

            ClearInterface();
        }

        /// <summary>
        /// 注册接口（获取 string）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 string 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<string> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetString wi = Main.m_ReferencePool.Spawn<WebInterfaceGetString>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(interfaceName, wi);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "添加接口失败：已存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 Texture2D）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 Texture2D 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<Texture2D> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetTexture2D wi = Main.m_ReferencePool.Spawn<WebInterfaceGetTexture2D>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(interfaceName, wi);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "添加接口失败：已存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 AudioClip）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 AudioClip 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<AudioClip> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(interfaceName))
            {
                WebInterfaceGetAudioClip wi = Main.m_ReferencePool.Spawn<WebInterfaceGetAudioClip>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(interfaceName, wi);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "添加接口失败：已存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（提交 表单）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl)
        {
            if (!_interfaces.ContainsKey(interfaceName))
            {
                WebInterfacePost wi = Main.m_ReferencePool.Spawn<WebInterfacePost>();
                wi.Name = interfaceName;
                wi.Url = interfaceUrl;
                _interfaces.Add(interfaceName, wi);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "添加接口失败：已存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 通过名称获取接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>网络接口</returns>
        public WebInterfaceBase GetInterface(string interfaceName)
        {
            if (_interfaces.ContainsKey(interfaceName))
            {
                return _interfaces[interfaceName];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "获取接口失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 是否存在指定名称的接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistInterface(string interfaceName)
        {
            return _interfaces.ContainsKey(interfaceName);
        }
        /// <summary>
        /// 取消注册接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        public void UnRegisterInterface(string interfaceName)
        {
            if (_interfaces.ContainsKey(interfaceName))
            {
                Main.m_ReferencePool.Despawn(_interfaces[interfaceName]);
                _interfaces.Remove(interfaceName);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "移除接口失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        /// <summary>
        /// 清空所有接口
        /// </summary>
        public void ClearInterface()
        {
            foreach (KeyValuePair<string, WebInterfaceBase> inter in _interfaces)
            {
                Main.m_ReferencePool.Despawn(inter.Value);
            }
            _interfaces.Clear();
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendRequest(string interfaceName, params string[] parameter)
        {
            if (_interfaces.ContainsKey(interfaceName))
            {
                if (IsOfflineState || _interfaces[interfaceName].IsOffline)
                {
                    _interfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendRequestCoroutine(interfaceName, parameter));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
            return null;
        }
        private IEnumerator SendRequestCoroutine(string interfaceName, params string[] parameter)
        {
            string url = _interfaces[interfaceName].Url + (parameter.Length > 0 ? ("?" + parameter[0]) : "");
            for (int i = 1; i < parameter.Length; i++)
            {
                url += "&" + parameter[i];
            }
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                DateTime begin = DateTime.Now;

                _interfaces[interfaceName].OnSetDownloadHandler(request);
                yield return request.SendWebRequest();

                DateTime end = DateTime.Now;

                if (!request.isNetworkError && !request.isHttpError)
                {
                    GlobalTools.LogInfo(string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 收到回复：{4}字节  string:{5}"
                        , begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length, _interfaces[interfaceName].OnGetDownloadString(request.downloadHandler)));

                    _interfaces[interfaceName].OnRequestFinished(request.downloadHandler);
                }
                else
                {
                    GlobalTools.LogError(string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 网络请求出错：{4}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.error));

                    _interfaces[interfaceName].OnRequestFinished(null);
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
            if (_interfaces.ContainsKey(interfaceName))
            {
                if (IsOfflineState || _interfaces[interfaceName].IsOffline)
                {
                    _interfaces[interfaceName].OfflineHandler?.Invoke();
                }
                else
                {
                    return Main.Current.StartCoroutine(SendRequestCoroutine(interfaceName, form));
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.WebRequest, "发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
            return null;
        }
        private IEnumerator SendRequestCoroutine(string interfaceName, WWWForm form)
        {
            string url = _interfaces[interfaceName].Url;
            
            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                DateTime begin = DateTime.Now;

                _interfaces[interfaceName].OnSetDownloadHandler(request);
                yield return request.SendWebRequest();

                DateTime end = DateTime.Now;

                if (!request.isNetworkError && !request.isHttpError)
                {
                    GlobalTools.LogInfo(string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 收到回复：{4}字节  string:{5}"
                        , begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length, _interfaces[interfaceName].OnGetDownloadString(request.downloadHandler)));

                    _interfaces[interfaceName].OnRequestFinished(request.downloadHandler);
                }
                else
                {
                    GlobalTools.LogError(string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 网络请求出错：{4}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.error));

                    _interfaces[interfaceName].OnRequestFinished(null);
                }
            }
        }
    }
}