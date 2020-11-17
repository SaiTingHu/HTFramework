using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// Web请求管理器的助手接口
    /// </summary>
    public interface IWebRequestHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有网络接口
        /// </summary>
        Dictionary<string, WebInterfaceBase> WebInterfaces { get; }
        
        /// <summary>
        /// 注册接口（获取 string）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 string 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<string> handler, HTFAction offlineHandle);
        /// <summary>
        /// 注册接口（获取 Texture2D）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 Texture2D 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<Texture2D> handler, HTFAction offlineHandle);
        /// <summary>
        /// 注册接口（获取 AudioClip）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 AudioClip 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<AudioClip> handler, HTFAction offlineHandle);
        /// <summary>
        /// 注册接口（获取 File）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="loadingHandler">下载过程中回调</param>
        /// <param name="finishedHandler">下载完成回调</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        void RegisterInterface(string interfaceName, string interfaceUrl, string savePath, HTFAction<float> loadingHandler, HTFAction<bool> finishedHandler, HTFAction offlineHandle);
        /// <summary>
        /// 注册接口（提交 表单）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        void RegisterInterface(string interfaceName, string interfaceUrl);
        /// <summary>
        /// 通过名称获取接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>网络接口</returns>
        WebInterfaceBase GetInterface(string interfaceName);
        /// <summary>
        /// 是否存在指定名称的接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>是否存在</returns>
        bool IsExistInterface(string interfaceName);
        /// <summary>
        /// 取消注册接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        void UnRegisterInterface(string interfaceName);
        /// <summary>
        /// 清空所有接口
        /// </summary>
        void ClearInterface();

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        Coroutine SendRequest(string interfaceName, params string[] parameter);
        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="form">参数表单</param>
        /// <returns>请求的协程</returns>
        Coroutine SendRequest(string interfaceName, WWWForm form);
        /// <summary>
        /// 发起下载文件请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        Coroutine SendDownloadFile(string interfaceName, params string[] parameter);
    }
}