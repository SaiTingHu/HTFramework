using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取Texture2D
    /// </summary>
    public sealed class WebInterfaceGetTexture2D : WebInterfaceBase
    {
        public HTFAction<Texture2D> Handler;

        public override void OnRequestFinished(DownloadHandler handler)
        {
            DownloadHandlerTexture downloadHandler = handler as DownloadHandlerTexture;

            Handler?.Invoke(downloadHandler.texture);
        }

        public override void OnSetDownloadHandler(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerTexture(true);
        }

        public override string OnGetDownloadString(DownloadHandler handler)
        {
            return "";
        }

        public override void Reset()
        {
            OfflineHandler = null;
            Handler = null;
        }
    }
}