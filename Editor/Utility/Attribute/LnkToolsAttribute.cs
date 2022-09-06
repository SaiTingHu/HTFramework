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
        /// 颜色r值
        /// </summary>
        public float R { get; private set; }
        /// <summary>
        /// 颜色g值
        /// </summary>
        public float G { get; private set; }
        /// <summary>
        /// 颜色b值
        /// </summary>
        public float B { get; private set; }
        /// <summary>
        /// 颜色a值
        /// </summary>
        public float A { get; private set; }
        /// <summary>
        /// 工具使用的内置图标
        /// </summary>
        public string BuiltinIcon { get; private set; }
        /// <summary>
        /// 快捷工具触发类型
        /// </summary>
        public LnkToolsMode Mode { get; private set; }
        /// <summary>
        /// 工具优先级（越小越靠前）
        /// </summary>
        public int Priority { get; private set; }

        public LnkToolsAttribute(string tooltip, string builtinIcon = "CustomTool", LnkToolsMode mode = LnkToolsMode.AllMode, int priority = int.MaxValue)
        {
            Tooltip = tooltip;
            R = 1;
            G = 1;
            B = 1;
            A = 1;
            BuiltinIcon = builtinIcon;
            Mode = mode;
            Priority = priority;
        }
        public LnkToolsAttribute(string tooltip, float r, float g, float b, float a, string builtinIcon = "CustomTool", LnkToolsMode mode = LnkToolsMode.AllMode, int priority = int.MaxValue)
        {
            Tooltip = tooltip;
            R = r;
            G = g;
            B = b;
            A = a;
            BuiltinIcon = builtinIcon;
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