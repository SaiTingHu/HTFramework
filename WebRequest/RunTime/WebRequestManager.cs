using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
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

        private Dictionary<string, string> _interfaces = new Dictionary<string, string>();
        private Dictionary<string, Action> _offlineHandler = new Dictionary<string, Action>();
        private Dictionary<string, Action<string>> _stringHandler = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<AssetBundle>> _assetBundleHandler = new Dictionary<string, Action<AssetBundle>>();
        private Dictionary<string, Action<Texture2D>> _texture2DHandler = new Dictionary<string, Action<Texture2D>>();

        public override void Termination()
        {
            base.Termination();

            ClearInterface();
        }

        /// <summary>
        /// 注册接口（接口的返回值为 string）
        /// </summary>
        public void RegisterInterface(string name, string url, Action<string> handleAction, Action offlineHandleAction = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                _interfaces.Add(name, url);
                _stringHandler.Add(name, handleAction);
                _offlineHandler.Add(name, offlineHandleAction);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（接口的返回值为 AssetBundle）
        /// </summary>
        public void RegisterInterface(string name, string url, Action<AssetBundle> handleAction, Action offlineHandleAction = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                _interfaces.Add(name, url);
                _assetBundleHandler.Add(name, handleAction);
                _offlineHandler.Add(name, offlineHandleAction);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 注册接口（接口的返回值为 Texture2D）
        /// </summary>
        public void RegisterInterface(string name, string url, Action<Texture2D> handleAction, Action offlineHandleAction = null)
        {
            if (!_interfaces.ContainsKey(name))
            {
                _interfaces.Add(name, url);
                _texture2DHandler.Add(name, handleAction);
                _offlineHandler.Add(name, offlineHandleAction);
            }
            else
            {
                GlobalTools.LogError("添加接口失败：已存在名为 " + name + " 的网络接口！");
            }
        }
        /// <summary>
        /// 获取接口的url
        /// </summary>
        public string GetInterface(string name)
        {
            if (_interfaces.ContainsKey(name))
            {
                return _interfaces[name];
            }
            else
            {
                GlobalTools.LogError("获取接口失败：不存在名为 " + name + " 的网络接口！");
                return "";
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
        /// 清空所有接口
        /// </summary>
        public void ClearInterface()
        {
            _interfaces.Clear();
            _offlineHandler.Clear();
            _stringHandler.Clear();
            _assetBundleHandler.Clear();
            _texture2DHandler.Clear();
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的string
        /// </summary>
        public void SendRequestGetString(string interfaceName, params string[] parameter)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetStringCoroutine(interfaceName, parameter));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetStringCoroutine(string interfaceName, params string[] parameter)
        {
            string url = _interfaces[interfaceName] + (parameter.Length > 0 ? ("?" + parameter[0]) : "");
            for (int i = 1; i < parameter.Length; i++)
            {
                url += "&" + parameter[i];
            }

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：" + request.downloadHandler.text);

                if (_stringHandler[interfaceName] != null)
                {
                    _stringHandler[interfaceName](request.downloadHandler.text);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            request.downloadHandler.Dispose();
            request.Dispose();
        }
        /// <summary>
        /// 发起网络请求，并处理接收到的string
        /// </summary>
        public void SendRequestGetString(string interfaceName, WWWForm form)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetStringCoroutine(interfaceName, form));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetStringCoroutine(string interfaceName, WWWForm form)
        {
            string url = _interfaces[interfaceName];

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：" + request.downloadHandler.text);

                if (_stringHandler[interfaceName] != null)
                {
                    _stringHandler[interfaceName](request.downloadHandler.text);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            request.downloadHandler.Dispose();
            request.Dispose();
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的AssetBundle
        /// </summary>
        public void SendRequestGetAssetBundle(string interfaceName, params string[] parameter)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetAssetBundleCoroutine(interfaceName, parameter));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetAssetBundleCoroutine(string interfaceName, params string[] parameter)
        {
            string url = _interfaces[interfaceName] + (parameter.Length > 0 ? ("?" + parameter[0]) : "");
            for (int i = 1; i < parameter.Length; i++)
            {
                url += "&" + parameter[i];
            }

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Get(url);
            DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
            request.downloadHandler = handler;
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：AssetBundle 字节长度 " + handler.data.Length);

                if (_assetBundleHandler[interfaceName] != null)
                {
                    _assetBundleHandler[interfaceName](handler.assetBundle);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            handler.Dispose();
            request.Dispose();
        }
        /// <summary>
        /// 发起网络请求，并处理接收到的AssetBundle
        /// </summary>
        public void SendRequestGetAssetBundle(string interfaceName, WWWForm form)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetAssetBundleCoroutine(interfaceName, form));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetAssetBundleCoroutine(string interfaceName, WWWForm form)
        {
            string url = _interfaces[interfaceName];

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url, uint.MaxValue);
            request.downloadHandler = handler;
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：AssetBundle 字节长度 " + handler.data.Length);

                if (_assetBundleHandler[interfaceName] != null)
                {
                    _assetBundleHandler[interfaceName](handler.assetBundle);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            handler.Dispose();
            request.Dispose();
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的Texture2D
        /// </summary>
        public void SendRequestGetTexture2D(string interfaceName, params string[] parameter)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetTexture2DCoroutine(interfaceName, parameter));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetTexture2DCoroutine(string interfaceName, params string[] parameter)
        {
            string url = _interfaces[interfaceName] + (parameter.Length > 0 ? ("?" + parameter[0]) : "");
            for (int i = 1; i < parameter.Length; i++)
            {
                url += "&" + parameter[i];
            }

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Get(url);
            DownloadHandlerTexture handler = new DownloadHandlerTexture(true);
            request.downloadHandler = handler;
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：Texture2D 字节长度 " + handler.data.Length);

                if (_texture2DHandler[interfaceName] != null)
                {
                    _texture2DHandler[interfaceName](handler.texture);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            handler.Dispose();
            request.Dispose();
        }
        /// <summary>
        /// 发起网络请求，并处理接收到的Texture2D
        /// </summary>
        public void SendRequestGetTexture2D(string interfaceName, WWWForm form)
        {
            if (IsExistInterface(interfaceName))
            {
                if (IsOfflineState)
                {
                    if (_offlineHandler[interfaceName] != null)
                    {
                        _offlineHandler[interfaceName]();
                    }
                }
                else
                {
                    StartCoroutine(SendRequestGetTexture2DCoroutine(interfaceName, form));
                }
            }
            else
            {
                GlobalTools.LogError("发起网络请求失败：不存在名为 " + interfaceName + " 的网络接口！");
            }
        }
        private IEnumerator SendRequestGetTexture2DCoroutine(string interfaceName, WWWForm form)
        {
            string url = _interfaces[interfaceName];

            DateTime begin = DateTime.Now;

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            DownloadHandlerTexture handler = new DownloadHandlerTexture(true);
            request.downloadHandler = handler;
            yield return request.SendWebRequest();

            DateTime end = DateTime.Now;

            if (!request.isNetworkError && !request.isHttpError)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：Texture2D 字节长度 " + handler.data.Length);

                if (_texture2DHandler[interfaceName] != null)
                {
                    _texture2DHandler[interfaceName](handler.texture);
                }
            }
            else
            {
                GlobalTools.LogError("网络请求出错：" + request.error);
            }

            handler.Dispose();
            request.Dispose();
        }
    }
}
