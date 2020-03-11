using System;

namespace HT.Framework
{
    /// <summary>
    /// 运行时程序集，仅可标记 string 类型的静态字段，可将该字段值代表的程序集加入到框架的运行时程序域中
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class RunTimeAssemblyAttribute : Attribute
    {
        
    }
}