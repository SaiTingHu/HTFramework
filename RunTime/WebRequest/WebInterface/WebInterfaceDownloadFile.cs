using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口：下载文件
    /// </summary>
    public sealed class WebInterfaceDownloadFile : WebInterfaceBase
    {
        public HTFAction<float> LoadingHandler;
        public HTFAction<bool> FinishedHandler;
        public string Path;

        public void OnLoading(float progress)
        {
            LoadingHandler?.Invoke(progress);
        }

        public void OnFinished(bool result)
        {
            FinishedHandler?.Invoke(result);
        }

        public override void OnRequestFinished(DownloadHandler handler)
        {

        }

        public override void OnSetDownloadHandler(UnityWebRequest request)
        {
            request.downloadHandler = new DownloadHandlerFile(Path);
        }

        public override string OnGetDownloadString(DownloadHandler handler)
        {
            return "";
        }

        public override void Reset()
        {
            OfflineHandler = null;
            LoadingHandler = null;
            FinishedHandler = null;
        }
    }
}