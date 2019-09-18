using System;

namespace HT.Framework
{
    /// <summary>
    /// CSDN博客链接
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CSDNBlogURLAttribute : Attribute
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string URL { get; private set; }

        public CSDNBlogURLAttribute(string url)
        {
            URL = url;
        }
    }
}
