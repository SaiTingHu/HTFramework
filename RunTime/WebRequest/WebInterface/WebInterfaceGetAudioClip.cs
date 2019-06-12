using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取AudioClip
    /// </summary>
    public sealed class WebInterfaceGetAudioClip : WebInterface
    {
        public HTFAction<AudioClip> Handler;

        public override void GetRequestFinish(DownloadHandler handler)
        {
            DownloadHandlerAudioClip downloadHandler = handler as DownloadHandlerAudioClip;

            if (Handler != null)
            {
                Handler(downloadHandler.audioClip);
            }
        }

        public override void Reset()
        {
            OfflineHandler = null;
            Handler = null;
        }
    }
}