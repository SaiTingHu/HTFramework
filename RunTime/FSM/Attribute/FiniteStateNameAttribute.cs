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
#if UNITY_EDITOR
        public string Name;
#endif

        public FiniteStateNameAttribute(string name)
        {
#if UNITY_EDITOR
            Name = name;
#endif
        }
    }
}
