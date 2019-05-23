using System;

namespace HT.Framework
{
    /// <summary>
    /// 备注特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class RemarkAttribute : Attribute
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; private set; }

        public RemarkAttribute(string remark)
        {
            Remark = remark;
        }
    }
}
