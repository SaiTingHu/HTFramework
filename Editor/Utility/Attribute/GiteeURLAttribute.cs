using System;

namespace HT.Framework
{
    /// <summary>
    /// Gitee链接
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GiteeURLAttribute : Attribute
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string URL { get; private set; }

        public GiteeURLAttribute(string url)
        {
            URL = url;
        }
    }
}