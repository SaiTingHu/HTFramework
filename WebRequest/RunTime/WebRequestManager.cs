using System.Collections;
using UnityEngine;
using System;

namespace HT.Framework
{
    /// <summary>
    /// Web请求管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WebRequestManager : ModuleManager
    {
        /// <summary>
        /// 发起网络请求，并处理接收到的数据
        /// </summary>
        public void SendRequest(string url, Action<string> handleAction)
        {
            StartCoroutine(SendRequestCoroutine(url, handleAction));
        }
        private static IEnumerator SendRequestCoroutine(string url, Action<string> handleAction)
        {
            DateTime begin = DateTime.Now;

            WWW www = new WWW(url);
            yield return www;

            DateTime end = DateTime.Now;

            GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：" + www.text);

            if (handleAction != null)
                handleAction(www.text);

            www.Dispose();
            www = null;
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的数据
        /// </summary>
        public void SendRequest(string url, WWWForm form, Action<string> handleAction)
        {
            StartCoroutine(SendRequestCoroutine(url, form, handleAction));
        }
        private static IEnumerator SendRequestCoroutine(string url, WWWForm form, Action<string> handleAction)
        {
            DateTime begin = DateTime.Now;

            WWW www = new WWW(url, form);
            yield return www;

            DateTime end = DateTime.Now;

            GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：" + www.text);

            if (handleAction != null)
                handleAction(www.text);

            www.Dispose();
            www = null;
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的AB包
        /// </summary>
        public void SendRequest(string url, Action<AssetBundle> handleAction)
        {
            StartCoroutine(SendRequestCoroutine(url, handleAction));
        }
        private static IEnumerator SendRequestCoroutine(string url, Action<AssetBundle> handleAction)
        {
            DateTime begin = DateTime.Now;

            WWW www = new WWW(url);
            yield return www;

            DateTime end = DateTime.Now;

            if (www.assetBundle)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：一个AB包，" + www.bytes.Length + "字节");

                if (handleAction != null)
                    handleAction(www.assetBundle);
            }
            else
            {
                GlobalTools.LogError("链接：" + url + " 未下载到AB包！");
            }            

            www.Dispose();
            www = null;
        }

        /// <summary>
        /// 发起网络请求，并处理接收到的图片
        /// </summary>
        public void SendRequest(string url, Action<Texture2D> handleAction)
        {
            StartCoroutine(SendRequestCoroutine(url, handleAction));
        }
        private static IEnumerator SendRequestCoroutine(string url, Action<Texture2D> handleAction)
        {
            DateTime begin = DateTime.Now;

            WWW www = new WWW(url);
            yield return www;

            DateTime end = DateTime.Now;

            if (www.texture)
            {
                GlobalTools.LogInfo("[" + begin.ToString("mm:ss:fff") + "] 发起网络请求：" + url + "\r\n"
                + "[" + end.ToString("mm:ss:fff") + "] 收到回复：一张图片，" + www.bytes.Length + "字节");

                if (handleAction != null)
                    handleAction(www.texture);
            }
            else
            {
                GlobalTools.LogError("链接：" + url + " 未下载到图片！");
            }

            www.Dispose();
            www = null;
        }
    }
}
