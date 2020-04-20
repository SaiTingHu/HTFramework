using System;

namespace HT.Framework
{
    /// <summary>
    /// 快捷工具，仅可用于静态函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class LnkToolsAttribute : Attribute
    {
        /// <summary>
        /// 工具名称
        /// </summary>
        public string Tooltip { get; private set; }
        /// <summary>
        /// 工具优先级
        /// </summary>
        public int Priority;

        public LnkToolsAttribute(string tooltip)
        {
            Tooltip = tooltip;
            Priority = int.MaxValue;
        }
        public LnkToolsAttribute(string tooltip, int priority)
        {
            Tooltip = tooltip;
            Priority = priority;
        }
    }
}