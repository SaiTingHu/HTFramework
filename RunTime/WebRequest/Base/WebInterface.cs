using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口基类
    /// </summary>
    public abstract class WebInterface : IReference
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 接口Url
        /// </summary>
        public string Url;
        /// <summary>
        /// 离线模式处理者
        /// </summary>
        public HTFAction OfflineHandler;
        /// <summary>
        /// 请求完成
        /// </summary>
        /// <param name="handler">下载助手</param>
        public abstract void GetRequestFinish(DownloadHandler handler);
        /// <summary>
        /// 设置下载助手
        /// </summary>
        /// <param name="request">网络请求的实例</param>
        public abstract void SetDownloadHandler(UnityWebRequest request);
        /// <summary>
        /// 重置
        /// </summary>
        public abstract void Reset();
    }
}
