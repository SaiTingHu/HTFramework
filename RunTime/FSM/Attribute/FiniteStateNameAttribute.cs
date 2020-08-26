using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class FiniteStateNameAttribute : Attribute
    {
        public string Name;

        public FiniteStateNameAttribute(string name)
        {
            Name = name;
        }
    }
}
