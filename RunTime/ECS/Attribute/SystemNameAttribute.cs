using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// ECS的系统名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class SystemNameAttribute : Attribute
    {
#if UNITY_EDITOR
        public string Name;
#endif

        public SystemNameAttribute(string name)
        {
#if UNITY_EDITOR
            Name = name;
#endif
        }
    }
}