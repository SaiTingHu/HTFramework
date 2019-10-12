using System;

namespace HT.Framework
{
    /// <summary>
    /// 自定义调试器组件检索的系统组件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CustomDebuggerAttribute : Attribute
    {
        public Type InspectedType { private set; get; }

        public CustomDebuggerAttribute(Type type)
        {
            InspectedType = type;
        }
    }
}