namespace HT.Framework
{
    /// <summary>
    /// 切面代理追踪器的助手接口
    /// </summary>
    public interface IAspectTrackHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 创建代理者
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="proxyObject">代理者</param>
        /// <returns>代理对象</returns>
        T CreateProxyer<T>(AspectProxyBase<T> proxyObject) where T : class, IAspectTrackObject;
        /// <summary>
        /// 移除代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        void RemoveProxyer(IAspectTrackObject realObject);
        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理对象</returns>
        T GetProxyObject<T>(IAspectTrackObject realObject) where T : class, IAspectTrackObject;
        /// <summary>
        /// 获取代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理者</returns>
        object GetProxyer(IAspectTrackObject realObject);
        /// <summary>
        /// 清空所有代理者、代理对象
        /// </summary>
        void ClearProxyer();
    }
}