using System;

namespace HT.Framework
{
    /// <summary>
    /// 自定义步骤助手的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomHelperAttribute : Attribute
    {
        public string Name;

        public CustomHelperAttribute(string name)
        {
            Name = name;
        }
    }
}
