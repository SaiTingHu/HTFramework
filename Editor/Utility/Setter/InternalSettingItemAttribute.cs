using System;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块的设置项
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal sealed class InternalSettingItemAttribute : Attribute
    {
        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public HTFrameworkModule Module { get; private set; }

        public InternalSettingItemAttribute(HTFrameworkModule module)
        {
            Module = module;
        }
    }
}