using System;

namespace HT.Framework
{
    /// <summary>
    /// 快捷工具，仅可标记静态函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class LnkToolsAttribute : Attribute
    {
        /// <summary>
        /// 工具名称
        /// </summary>
        public string Tooltip { get; private set; }
        /// <summary>
        /// 快捷工具触发类型
        /// </summary>
        public LnkToolsMode Mode { get; private set; }
        /// <summary>
        /// 工具优先级
        /// </summary>
        public int Priority { get; private set; }

        public LnkToolsAttribute(string tooltip, LnkToolsMode mode = LnkToolsMode.AllMode, int priority = int.MaxValue)
        {
            Tooltip = tooltip;
            Mode = mode;
            Priority = priority;
        }
    }

    /// <summary>
    /// 快捷工具触发类型
    /// </summary>
    public enum LnkToolsMode
    {
        /// <summary>
        /// 所有模式
        /// </summary>
        AllMode,
        /// <summary>
        /// 仅在运行时
        /// </summary>
        OnlyRuntime,
        /// <summary>
        /// 仅在编辑时
        /// </summary>
        OnlyEditor
    }
}