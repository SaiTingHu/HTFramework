using System;

namespace HT.Framework
{
    /// <summary>
    /// 自定义工具，仅可用于静态函数（将会附加至菜单：HTFramework -> Tools -> CustomTool）
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CustomToolAttribute : Attribute
    {
        
    }
}