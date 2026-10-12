using System;

namespace HT.Framework
{
    /// <summary>
    /// 自动填充参数特性（仅用于自定义步骤助手）：自动将步骤编辑器中添加的参数填充到字段中
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class PopulateParameterAttribute : Attribute
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 自动填充参数特性（仅用于自定义步骤助手）：自动将步骤编辑器中添加的参数填充到字段中
        /// </summary>
        /// <param name="name">参数名称</param>
        public PopulateParameterAttribute(string name)
        {
            Name = name;
        }
    }
}