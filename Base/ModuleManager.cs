using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 模块管理者基类
    /// </summary>
    public abstract class ModuleManager : MonoBehaviour, IAspectProxyModule
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialization()
        { }
        /// <summary>
        /// 刷新模块
        /// </summary>
        public virtual void Refresh()
        { }
        /// <summary>
        /// 终结模块
        /// </summary>
        public virtual void Termination()
        { }
        /// <summary>
        /// 追踪
        /// </summary>
        public virtual void Track()
        { }
    }
}
