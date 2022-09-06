using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 自定义步骤助手的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class CustomHelperAttribute : Attribute
    {
        public string Name;

        public CustomHelperAttribute(string name)
        {
            Name = name;
        }
    }
}