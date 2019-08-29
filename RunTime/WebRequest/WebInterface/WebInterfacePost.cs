using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：提交表单
    /// </summary>
    public sealed class WebInterfacePost : WebInterfaceBase
    {
        public override void OnRequestFinished(DownloadHandler handler)
        {
            
        }

        public override void OnSetDownloadHandler(UnityWebRequest request)
        {

        }

        public override string OnGetDownloadString(DownloadHandler handler)
        {
            return "";
        }

        public override void Reset()
        {
            
        }
    }
}
