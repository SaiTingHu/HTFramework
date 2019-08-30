using UnityEngine;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取AudioClip
    /// </summary>
    public sealed class WebInterfaceGetAudioClip : WebInterfaceBase
    {
        public HTFAction<AudioClip> Handler;

        public override void OnRequestFinished(DownloadHandler handler)
        {
            if (handler == null)
            {
                Handler?.Invoke(null);
            }
            else
            {
                DownloadHandlerAudioClip downloadHandler = handler as DownloadHandlerAudioClip;
                Handler?.Invoke(downloadHandler.audioClip);
            }
        }

        public override void OnSetDownloadHandler(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerAudioClip(request.url, Main.m_WebRequest.DownloadAudioType);
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