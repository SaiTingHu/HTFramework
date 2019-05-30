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
                WebInterfaceGetString wi = new WebInterfaceGetString();
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
                WebInterfaceGetAssetBundle wi = new WebInterfaceGetAssetBundle();
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
                WebInterfaceGetTexture2D wi = new WebInterfaceGetTexture2D();
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
                WebInterfaceGetAudioClip wi = new WebInterfaceGetAudioClip();
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
            SetDownloadHandler(request, _interfaces[interfaceName]);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                string info = string.Format("[{0}] 发起网络请求：{1}\r\n[{2}] 收到回复：{3}字节", begin.ToString("mm:ss:fff"), url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length);
                GlobalTools.LogInfo(info);

                _interfaces[interfaceName].GetRequestFinish(request.downloadHandler);
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
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
            SetDownloadHandler(request, _interfaces[interfaceName]);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                string info = string.Format("[{0}] 发起网络请求：{1}\r\n[{2}] 收到回复：{3}字节", begin.ToString("mm:ss:fff"), url, end.ToString("mm:ss:fff"), request.downloadHandler.data.Length);
                GlobalTools.LogInfo(info);

                _interfaces[interfaceName].GetRequestFinish(request.downloadHandler);
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            request.downloadHandler.Dispose();
            request.Dispose();
        }

        private void SetDownloadHandler(UnityWebRequest request, WebInterface wi)
        {
            if (wi is WebInterfaceGetAssetBundle)
            {
                request.downloadHandler = new DownloadHandlerAssetBundle(request.url, 0);
            }
            else if (wi is WebInterfaceGetAudioClip)
            {
                request.downloadHandler = new DownloadHandlerAudioClip(request.url, DownloadAudioType);
            }
            else if (wi is WebInterfaceGetString)
            {
                return;
            }
            else if (wi is WebInterfaceGetTexture2D)
            {
                request.downloadHandler = new DownloadHandlerTexture(true);
            }
        }
    }
}
