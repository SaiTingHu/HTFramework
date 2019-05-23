using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 切面代码追踪者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AspectTracker : ModuleManager
    {
        public bool IsEnableAspectTrack = false;
        
        /// <summary>
        /// 创建追踪者
        /// </summary>
        public T CreateTracker<T>(AspectProxy<T> proxyObject) where T : class, IAspectTrackObject
        {
            return proxyObject.GetTransparentProxy() as T;
        }
    }
}