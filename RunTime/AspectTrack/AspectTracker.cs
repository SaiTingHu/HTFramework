using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 切面代理追踪器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.AspectTrack)]
    public sealed class AspectTracker : InternalModuleBase
    {
        /// <summary>
        /// 是否启用切面追踪【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableAspectTrack = false;
        /// <summary>
        /// 是否启用全局拦截（注意：只拦截无返回值的方法的调用）
        /// </summary>
        public bool IsEnableIntercept = false;
        /// <summary>
        /// 全局拦截条件
        /// </summary>
        internal Dictionary<string, HTFFunc<MethodBase, object[], bool>> InterceptConditions { get; private set; } = new Dictionary<string, HTFFunc<MethodBase, object[], bool>>();
        
        private IAspectTrackHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IAspectTrackHelper;
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            ClearInterceptCondition();
            ClearProxyer();
        }

        /// <summary>
        /// 新增拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <param name="interceptCondition">拦截规则</param>
        public void AddInterceptCondition(string conditionName, HTFFunc<MethodBase, object[], bool> interceptCondition)
        {
            if (!InterceptConditions.ContainsKey(conditionName))
            {
                InterceptConditions.Add(conditionName, interceptCondition);
            }
            else
            {
                Log.Warning("新增拦截条件失败：已存在同名拦截条件 " + conditionName + " ！");
            }
        }
        /// <summary>
        /// 是否存在拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistCondition(string conditionName)
        {
            return InterceptConditions.ContainsKey(conditionName);
        }
        /// <summary>
        /// 移除拦截条件
        /// </summary>
        /// <param name="conditionName">条件名称</param>
        public void RemoveInterceptCondition(string conditionName)
        {
            if (InterceptConditions.ContainsKey(conditionName))
            {
                InterceptConditions.Remove(conditionName);
            }
        }
        /// <summary>
        /// 清空所有拦截条件
        /// </summary>
        public void ClearInterceptCondition()
        {
            InterceptConditions.Clear();
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