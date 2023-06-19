using System;

namespace HT.Framework
{
    /// <summary>
    /// 自定义指令标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomInstructionAttribute : Attribute
    {
        public string Keyword { get; private set; }

        /// <summary>
        /// 自定义指令标记
        /// </summary>
        /// <param name="keyword">指令关键字</param>
        public CustomInstructionAttribute(string keyword)
        {
            Keyword = keyword;
        }
    }
}