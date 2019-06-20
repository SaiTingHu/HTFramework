using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取Texture2D
    /// </summary>
    public sealed class WebInterfaceGetTexture2D : WebInterface
    {
        public HTFAction<Texture2D> Handler;

        public override void GetRequestFinish(DownloadHandler handler)
        {
            DownloadHandlerTexture downloadHandler = handler as DownloadHandlerTexture;

            if (Handler != null)
            {
                Handler(downloadHandler.texture);
            }
        }

        public override void SetDownloadHandler(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerTexture(true);
        }

        public override void Reset()
        {
            OfflineHandler = null;
            Handler = null;
        }
    }
}