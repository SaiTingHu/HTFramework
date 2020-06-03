using System;

namespace HT.Framework
{
    /// <summary>
    /// ECS的系统名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SystemNameAttribute : Attribute
    {
        public string Name;

        public SystemNameAttribute(string name)
        {
            Name = name;
        }
    }
}