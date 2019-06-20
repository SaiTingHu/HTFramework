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
    public sealed class WebRequestManager : ModuleManager
    {
        /// <summary>
        /// 离线状态
        /// </summary>
        public bool IsOfflineState = false;
        /// <summary>
        /// 下载音频的格式
        /// </summary>
        public AudioType DownloadAudioType = AudioType.WAV;

        private Dictionary<string, WebInterface> _interfaces = new Dictionary<string, WebInterface>();
        
        public override void Termination()
        {
            base.Termination();

            ClearInterface();
        }

        /// <summary>
        /// 注册接口（获取 string）
        /// </summary>
        public void RegisterInterface(string name, string url, HTFAction<string> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                WebInterfaceGetString wi = Main.m_ReferencePool.Spawn<WebInterfaceGetString>();
                wi.Name = name;
                wi.Url = url;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(name, wi);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 AssetBundle）
        /// </summary>
        public void RegisterInterface(string name, string url, HTFAction<AssetBundle> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                WebInterfaceGetAssetBundle wi = Main.m_ReferencePool.Spawn<WebInterfaceGetAssetBundle>();
                wi.Name = name;
                wi.Url = url;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(name, wi);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 Texture2D）
        /// </summary>
        public void RegisterInterface(string name, string url, HTFAction<Texture2D> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                WebInterfaceGetTexture2D wi = Main.m_ReferencePool.Spawn<WebInterfaceGetTexture2D>();
                wi.Name = name;
                wi.Url = url;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(name, wi);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（获取 AudioClip）
        /// </summary>
        public void RegisterInterface(string name, string url, HTFAction<AudioClip> handler, HTFAction offlineHandle = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                WebInterfaceGetAudioClip wi = Main.m_ReferencePool.Spawn<WebInterfaceGetAudioClip>();
                wi.Name = name;
                wi.Url = url;
                wi.OfflineHandler = offlineHandle;
                wi.Handler = handler;
                _interfaces.Add(name, wi);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（提交 表单）
        /// </summary>
        public void RegisterInterface(string name, string url)
        {
            if (!_interfaces.ContainsKey(name))
            {
                WebInterfacePost wi = Main.m_ReferencePool.Spawn<WebInterfacePost>();
                wi.Name = name;
                wi.Url = url;
                _interfaces.Add(name, wi);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 通过名称获取接口
        /// </summary>
        public WebInterface GetInterface(string name)
        {
            if (_interfaces.ContainsKey(name))
            {
                return _interfaces[name];
            }
            else
            {
                GlobalTools.LogError("获取接口失败：不存在名为 " + name + " 的网络接口！");
                return null;
            }
        }
        /// <summary>
        /// 是否存在指定名称的接口
        /// </summary>
        public bool IsExistInterface(string name)
        {
            return _interfaces.ContainsKey(name);
        }
        /// <summary>
        /// 取消注册接口
        /// </summary>
        public void UnRegisterInterface(string name)
        {
            if (_interfaces.ContainsKey(name))
            {
                Main.m_ReferencePool.Despawn(_interfaces[name]);
                _interfaces.Remove(name);
            }
            else
            {
                GlobalTools.LogError("移除接口失败：不存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 清空所有接口
        /// </summary>
        public void ClearInterface()
        {
            foreach (KeyValuePair<string, WebInterface> inter in _interfaces)
            {
                Main.m_ReferencePool.Despawn(inter.Value);
            }
            _interfaces.Clear();
        }
        
        /// <summary>
        /// 发起网络请求
        /// </summary>
        public void SendRequest(string interfaceName, params string[] parameter)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_interfaces[interfaceName].OfflineHandler != null)
                    {
                        _interfaces[interfaceName].OfflineHandler();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestCoroutine(interfaceName, parameter));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestCoroutine(string interfaceName, params string[] parameter)
        {
            string url = _interfaces[interfaceName].Url + (parameter.Length > 0 ? ("?" + parameter[0]) : "");
            for (int i = 1; i < parameter.Length; i++)
            {
                url += "&" + parameter[i];
            }

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Get(url);
            _interfaces[interfaceName].SetDownloadHandler(request);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                string info = string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 收到回复：{4}字节  string:{5}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length, request.downloadHandler.text);
                GlobalTools.LogInfo(info);

                _interfaces[interfaceName].GetRequestFinish(request.downloadHandler);
            }
            else
            {
                string info = string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 网络请求出错：{4}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.error);
                GlobalTools.LogError(info);
            }

            request.downloadHandler.Dispose();
            request.Dispose();
        }
        /// <summary>
        /// 发起网络请求
        /// </summary>
        public void SendRequest(string interfaceName, WWWForm form)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_interfaces[interfaceName].OfflineHandler != null)
                    {
                        _interfaces[interfaceName].OfflineHandler();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestCoroutine(interfaceName, form));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestCoroutine(string interfaceName, WWWForm form)
        {
            string url = _interfaces[interfaceName].Url;

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            _interfaces[interfaceName].SetDownloadHandler(request);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                string info = string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 收到回复：{4}字节  string:{5}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length, request.downloadHandler.text);
                GlobalTools.LogInfo(info);

                _interfaces[interfaceName].GetRequestFinish(request.downloadHandler);
            }
            else
            {
                string info = string.Format("[{0}] 发起网络请求：[{1}] {2}\r\n[{3}] 网络请求出错：{4}", begin.ToString("mm:ss:fff"), interfaceName, url, end.ToString("mm:ss:fff"), request.error);
                GlobalTools.LogError(info);
            }

            request.downloadHandler.Dispose();
            request.Dispose();
        }
    }
}
