using System;

namespace HT.Framework
{
    /// <summary>
    /// 自定义模块标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomModuleAttribute : Attribute
    {
        public string ModuleName { get; private set; }
        public bool IsEnable { get; private set; }

        public CustomModuleAttribute(string moduleName, bool isEnable)
        {
            ModuleName = moduleName;
            IsEnable = isEnable;
        }
    }
}