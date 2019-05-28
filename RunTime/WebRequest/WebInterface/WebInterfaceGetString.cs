using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：获取字符串
    /// </summary>
    public sealed class WebInterfaceGetString : WebInterface
    {
        public HTFAction<string> Handler;

        public override void GetRequestFinish(DownloadHandler handler)
        {
            if (Handler != null)
            {
                Handler(handler.text);
            }
        }
    }
}
