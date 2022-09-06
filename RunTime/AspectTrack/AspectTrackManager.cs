﻿using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 切面代理追踪器
    /// </summary>
    [InternalModule(HTFrameworkModule.AspectTrack)]
    public sealed class AspectTrackManager : InternalModuleBase<IAspectTrackHelper>
    {
        /// <summary>
        /// 是否启用切面追踪【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableAspectTrack = false;
        /// <summary>
        /// 是否启用全局拦截（注意：只拦截无返回值方法的调用）
        /// </summary>
        public bool IsEnableIntercept = false;
        
        /// <summary>
        /// 全局拦截条件
        /// </summary>
        internal Dictionary<string, HTFFunc<MethodBase, object[], bool>> InterceptConditions
        {
            get
            {
                return _helper.InterceptConditions;
            }
        }
        
        /// <summary>
        /// 新增拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <param name="interceptCondition">拦截规则</param>
        public void AddInterceptCondition(string conditionName, HTFFunc<MethodBase, object[], bool> interceptCondition)
        {
            _helper.AddInterceptCondition(conditionName, interceptCondition);
        }
        /// <summary>
        /// 是否存在拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistCondition(string conditionName)
        {
            return _helper.IsExistCondition(conditionName);
        }
        /// <summary>
        /// 移除拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        public void RemoveInterceptCondition(string conditionName)
        {
            _helper.RemoveInterceptCondition(conditionName);
        }
        /// <summary>
        /// 清空所有拦截条件
        /// </summary>
        public void ClearInterceptCondition()
        {
            _helper.ClearInterceptCondition();
        }

        /// <summary>
        /// 创建代理者
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="proxyObject">代理者</param>
        /// <returns>代理对象</returns>
        public T CreateProxyer<T>(AspectProxyBase<T> proxyObject) where T : class, IAspectTrackObject
        {
            return _helper.CreateProxyer(proxyObject);
        }
        /// <summary>
        /// 移除代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        public void RemoveProxyer(IAspectTrackObject realObject)
        {
            _helper.RemoveProxyer(realObject);
        }
        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理对象</returns>
        public T GetProxyObject<T>(IAspectTrackObject realObject) where T : class, IAspectTrackObject
        {
            return _helper.GetProxyObject<T>(realObject);
        }
        /// <summary>
        /// 获取代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理者</returns>
        public object GetProxyer(IAspectTrackObject realObject)
        {
            return _helper.GetProxyer(realObject);
        }
        /// <summary>
        /// 清空所有代理者、代理对象
        /// </summary>
        public void ClearProxyer()
        {
            _helper.ClearProxyer();
        }
    }
}