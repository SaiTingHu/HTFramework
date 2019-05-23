using System;
using UnityEngine.Networking;

namespace HT.Framework
{
    /// <summary>
    /// 网络接口基类
    /// </summary>
    public abstract class WebInterface
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
        public Action OfflineHandler;
        /// <summary>
        /// Get请求完成
        /// </summary>
        public abstract void GetRequestFinish(DownloadHandler handler);
    }
}
