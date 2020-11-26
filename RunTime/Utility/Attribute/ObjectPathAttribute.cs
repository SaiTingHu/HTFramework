using System;

namespace HT.Framework
{
    /// <summary>
    /// 【自动化任务】对象路径定义，将根据设置的路径查找子对象用以初始化（仅可用于 EntityLogicBase 及 UILogicBase 及 HTBehaviour 子类中定义的非静态字段）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ObjectPathAttribute : Attribute
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; private set; }

        public ObjectPathAttribute(string path)
        {
            Path = path;
        }
    }
}