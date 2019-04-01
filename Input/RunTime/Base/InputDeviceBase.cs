namespace HT.Framework
{
    /// <summary>
    /// 输入设备基类
    /// </summary>
    public abstract class InputDeviceBase
    {
        /// <summary>
        /// 设备启动
        /// </summary>
        public abstract void OnStartUp();

        /// <summary>
        /// 设备运作
        /// </summary>
        public abstract void OnRun();

        /// <summary>
        /// 设备关闭
        /// </summary>
        public abstract void OnShutdown();
    }
}
