using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 切面代理追踪器的助手接口
    /// </summary>
    public interface IAspectTrackHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有的代理对象【真实对象、代理对象】
        /// </summary>
        Dictionary<IAspectTrackObject, IAspectTrackObject> ProxyObjects { get; }
        /// <summary>
        /// 所有的代理者【真实对象，代理者】
        /// </summary>
        Dictionary<IAspectTrackObject, object> Proxys { get; }
        /// <summary>
        /// 全局拦截条件
        /// </summary>
        Dictionary<string, HTFFunc<MethodBase, object[], bool>> InterceptConditions { get; }

        /// <summary>
        /// 新增拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <param name="interceptCondition">拦截规则</param>
        void AddInterceptCondition(string conditionName, HTFFunc<MethodBase, object[], bool> interceptCondition);
        /// <summary>
        /// 是否存在拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <returns>是否存在</returns>
        bool IsExistCondition(string conditionName);
        /// <summary>
        /// 移除拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        void RemoveInterceptCondition(string conditionName);
        /// <summary>
        /// 清空所有拦截条件
        /// </summary>
        void ClearInterceptCondition();

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