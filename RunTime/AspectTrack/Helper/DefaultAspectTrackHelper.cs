using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的切面代理追踪器助手
    /// </summary>
    public sealed class DefaultAspectTrackHelper : IAspectTrackHelper
    {
        //所有的代理对象 <真实对象、代理对象>
        private Dictionary<IAspectTrackObject, IAspectTrackObject> _proxyObjects = new Dictionary<IAspectTrackObject, IAspectTrackObject>();
        //所有的代理者 <真实对象，代理者>
        private Dictionary<IAspectTrackObject, object> _proxys = new Dictionary<IAspectTrackObject, object>();

        /// <summary>
        /// 切面代理追踪器
        /// </summary>
        public InternalModuleBase Module { get; set; }

        /// <summary>
        /// 创建代理者
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="proxyObject">代理者</param>
        /// <returns>代理对象</returns>
        public T CreateProxyer<T>(AspectProxyBase<T> proxyObject) where T : class, IAspectTrackObject
        {
            IAspectTrackObject proxyObj = proxyObject.GetTransparentProxy() as IAspectTrackObject;
            IAspectTrackObject realObj = proxyObject.RealObject;

            if (!_proxyObjects.ContainsKey(realObj))
            {
                _proxyObjects.Add(realObj, proxyObj);
            }
            if (!_proxys.ContainsKey(realObj))
            {
                _proxys.Add(realObj, proxyObject);
            }

            return proxyObj as T;
        }
        /// <summary>
        /// 移除代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        public void RemoveProxyer(IAspectTrackObject realObject)
        {
            if (_proxyObjects.ContainsKey(realObject))
            {
                _proxyObjects.Remove(realObject);
            }
            if (_proxys.ContainsKey(realObject))
            {
                _proxys.Remove(realObject);
            }
        }
        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <typeparam name="T">代理对象类型</typeparam>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理对象</returns>
        public T GetProxyObject<T>(IAspectTrackObject realObject) where T : class, IAspectTrackObject
        {
            if (_proxyObjects.ContainsKey(realObject))
            {
                return _proxyObjects[realObject] as T;
            }
            else
            {
                Log.Warning("获取代理对象失败：真实对象 " + realObject.ToString() + " 并不存在代理对象！");
                return null;
            }
        }
        /// <summary>
        /// 获取代理者
        /// </summary>
        /// <param name="realObject">真实对象</param>
        /// <returns>代理者</returns>
        public object GetProxyer(IAspectTrackObject realObject)
        {
            if (_proxys.ContainsKey(realObject))
            {
                return _proxys[realObject];
            }
            else
            {
                Log.Warning("获取代理者失败：真实对象 " + realObject.ToString() + " 并不存在代理者！");
                return null;
            }
        }
        /// <summary>
        /// 清空所有代理者、代理对象
        /// </summary>
        public void ClearProxyer()
        {
            _proxyObjects.Clear();
            _proxys.Clear();
        }
    }
}