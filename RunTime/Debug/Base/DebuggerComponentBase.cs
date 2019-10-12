using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试器组件基类
    /// </summary>
    public abstract class DebuggerComponentBase
    {
        /// <summary>
        /// 当前目标
        /// </summary>
        protected Component Target;

        public abstract void OnEnable();
        public abstract void OnDebuggerGUI();
    }
}