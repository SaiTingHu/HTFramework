using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口基类
    /// </summary>
    public abstract class WebInterfaceBase : IReference
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
        /// 是否是离线接口
        /// </summary>
        public bool IsOffline = false;
        /// <summary>
        /// 离线模式处理者
        /// </summary>
        public HTFAction OfflineHandler;

        /// <summary>
        /// 请求完成
        /// </summary>
        /// <param name="handler">下载助手</param>
        public abstract void OnRequestFinished(DownloadHandler handler);
        /// <summary>
        /// 设置下载助手
        /// </summary>
        /// <param name="request">网络请求的实例</param>
        public abstract void OnSetDownloadHandler(UnityWebRequest request);
        /// <summary>
        /// 获取下载的字符串
        /// </summary>
        /// <param name="handler">下载助手</param>
        /// <returns>下载的字符串</returns>
        public abstract string OnGetDownloadString(DownloadHandler handler);
        /// <summary>
        /// 重置
        /// </summary>
        public abstract void Reset();
    }
}