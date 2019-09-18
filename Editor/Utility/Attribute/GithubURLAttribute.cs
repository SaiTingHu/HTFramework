using System;

namespace HT.Framework
{
    /// <summary>
    /// Github链接
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class GithubURLAttribute : Attribute
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string URL { get; private set; }

        public GithubURLAttribute(string url)
        {
            URL = url;
        }
    }
}
