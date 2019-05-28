using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取AssetBundle
    /// </summary>
    public sealed class WebInterfaceGetAssetBundle : WebInterface
    {
        public HTFAction<AssetBundle> Handler;

        public override void GetRequestFinish(DownloadHandler handler)
        {
            DownloadHandlerAssetBundle downloadHandler = handler as DownloadHandlerAssetBundle;

            if (Handler != null)
            {
                Handler(downloadHandler.assetBundle);
            }
        }
    }
}