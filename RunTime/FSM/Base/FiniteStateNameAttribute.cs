using System;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FiniteStateNameAttribute : Attribute
    {
        public string Name;

        public FiniteStateNameAttribute(string name)
        {
            Name = name;
        }
    }
}
