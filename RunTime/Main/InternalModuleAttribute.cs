using System;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class InternalModuleAttribute : Attribute
    {
        public HTFrameworkModule ModuleName { get; private set; }

        public InternalModuleAttribute(HTFrameworkModule moduleName)
        {
            ModuleName = moduleName;
        }
    }
}