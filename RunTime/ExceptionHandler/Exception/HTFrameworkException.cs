using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// HTFramework框架异常
    /// </summary>
    public sealed class HTFrameworkException : UnityException
    {
        /// <summary>
        /// 异常模块
        /// </summary>
        public HTFrameworkModule Module;

        /// <summary>
        /// HTFramework框架异常
        /// </summary>
        /// <param name="module">异常发起的模块</param>
        /// <param name="message">异常信息</param>
        public HTFrameworkException(HTFrameworkModule module, string message) : base("[" + module.ToString() + "]" + message)
        {
            Module = module;
        }
    }
}