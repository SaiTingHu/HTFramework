using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// Web请求管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.WebRequest)]
    public sealed class WebRequestManager : InternalModuleBase
    {
        /// <summary>
        /// 当前是否是离线状态
        /// </summary>
        public bool IsOfflineState = false;
        /// <summary>
        /// 下载音频的格式
        /// </summary>
        public AudioType DownloadAudioType = AudioType.WAV;
        
        private IWebRequestHelper _helper;

        private WebRequestManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IWebRequestHelper;
        }

        /// <summary>
        /// 注册接口（获取 string）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 string 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<string> handler, HTFAction offlineHandle = null)
        {
            _helper.RegisterInterface(interfaceName, interfaceUrl, handler, offlineHandle);
        }
        /// <summary>
        /// 注册接口（获取 Texture2D）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 Texture2D 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<Texture2D> handler, HTFAction offlineHandle = null)
        {
            _helper.RegisterInterface(interfaceName, interfaceUrl, handler, offlineHandle);
        }
        /// <summary>
        /// 注册接口（获取 AudioClip）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="handler">获取 AudioClip 之后的处理者</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, HTFAction<AudioClip> handler, HTFAction offlineHandle = null)
        {
            _helper.RegisterInterface(interfaceName, interfaceUrl, handler, offlineHandle);
        }
        /// <summary>
        /// 注册接口（获取 File）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="loadingHandler">下载过程中回调</param>
        /// <param name="finishedHandler">下载完成回调</param>
        /// <param name="offlineHandle">离线模式处理者</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl, string savePath, HTFAction<float> loadingHandler = null, HTFAction<bool> finishedHandler = null, HTFAction offlineHandle = null)
        {
            _helper.RegisterInterface(interfaceName, interfaceUrl, savePath, loadingHandler, finishedHandler, offlineHandle);
        }
        /// <summary>
        /// 注册接口（提交 表单）
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="interfaceUrl">接口url</param>
        public void RegisterInterface(string interfaceName, string interfaceUrl)
        {
            _helper.RegisterInterface(interfaceName, interfaceUrl);
        }
        /// <summary>
        /// 通过名称获取接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>网络接口</returns>
        public WebInterfaceBase GetInterface(string interfaceName)
        {
            return _helper.GetInterface(interfaceName);
        }
        /// <summary>
        /// 是否存在指定名称的接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistInterface(string interfaceName)
        {
            return _helper.IsExistInterface(interfaceName);
        }
        /// <summary>
        /// 取消注册接口
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        public void UnRegisterInterface(string interfaceName)
        {
            _helper.UnRegisterInterface(interfaceName);
        }
        /// <summary>
        /// 清空所有接口
        /// </summary>
        public void ClearInterface()
        {
            _helper.ClearInterface();
        }

        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendRequest(string interfaceName, params string[] parameter)
        {
            return _helper.SendRequest(interfaceName, parameter);
        }
        /// <summary>
        /// 发起网络请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="form">参数表单</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendRequest(string interfaceName, WWWForm form)
        {
            return _helper.SendRequest(interfaceName, form);
        }
        /// <summary>
        /// 发起下载文件请求
        /// </summary>
        /// <param name="interfaceName">接口名称</param>
        /// <param name="parameter">可选参数（要同时传入参数名和参数值，例：name='张三'）</param>
        /// <returns>请求的协程</returns>
        public Coroutine SendDownloadFile(string interfaceName, params string[] parameter)
        {
            return _helper.SendDownloadFile(interfaceName, parameter);
        }
    }
}