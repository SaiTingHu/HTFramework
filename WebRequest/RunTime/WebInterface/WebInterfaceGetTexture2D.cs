using System;
using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取Texture2D
    /// </summary>
    public sealed class WebInterfaceGetTexture2D : WebInterface
    {
        public Action<Texture2D> Handler;

        public override void GetRequestFinish(DownloadHandler handler)
        {
            DownloadHandlerTexture downloadHandler = handler as DownloadHandlerTexture;

            if (Handler != null)
            {
                Handler(downloadHandler.texture);
            }
        }
    }
}