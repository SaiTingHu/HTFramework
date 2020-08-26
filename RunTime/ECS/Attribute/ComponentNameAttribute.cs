using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// ECS的组件名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ComponentNameAttribute : Attribute
    {
        public string Name;

        public ComponentNameAttribute(string name)
        {
            Name = name;
        }
    }
}