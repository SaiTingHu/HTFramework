using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取字符串
    /// </summary>
    public sealed class WebInterfaceGetString : WebInterfaceBase
    {
        public HTFAction<string> Handler;

        public override void OnRequestFinished(DownloadHandler handler)
        {
            if (handler == null)
            {
                Handler?.Invoke("");
            }
            else
            {
                Handler?.Invoke(handler.text);
            }
        }
        public override void OnSetDownloadHandler(UnityWebRequest request)
        {

        }
        public override string OnGetDownloadString(DownloadHandler handler)
        {
            return handler.text;
        }
        public override void Reset()
        {
            OfflineHandler = null;
            Handler = null;
        }
    }
}