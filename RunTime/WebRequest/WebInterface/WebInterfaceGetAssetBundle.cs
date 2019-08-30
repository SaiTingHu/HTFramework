using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取AssetBundle
    /// </summary>
    public sealed class WebInterfaceGetAssetBundle : WebInterfaceBase
    {
        public HTFAction<AssetBundle> Handler;

        public override void OnRequestFinished(DownloadHandler handler)
        {
            if (handler == null)
            {
                Handler?.Invoke(null);
            }
            else
            {
                DownloadHandlerAssetBundle downloadHandler = handler as DownloadHandlerAssetBundle;
                Handler?.Invoke(downloadHandler.assetBundle);
            }
        }

        public override void OnSetDownloadHandler(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerAssetBundle(request.url, 0);
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