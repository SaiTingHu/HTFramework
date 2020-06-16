using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 版本号
    /// </summary>
    [Serializable]
    internal sealed class Version
    {
        /// <summary>
        /// 主版本号
        /// </summary>
        public int MajorNumber = 0;
        /// <summary>
        /// 次版本号
        /// </summary>
        public int MinorNumber = 0;
        /// <summary>
        /// 修订版本号
        /// </summary>
        public int ReviseNumber = 0;
        /// <summary>
        /// 发行说明
        /// </summary>
        public string ReleaseNotes;
        /// <summary>
        /// 版本号支持的平台
        /// </summary>
        public List<Platform> Platforms = new List<Platform>();
        /// <summary>
        /// 版本号支持的Unity版本
        /// </summary>
        public string UnityVersions;
        /// <summary>
        /// 版本号支持的运行时编码.Net版本
        /// </summary>
        public string ScriptingVersions;
        /// <summary>
        /// 版本号支持的.Net API版本
        /// </summary>
        public string APIVersions;

        /// <summary>
        /// 获取完成的版本号
        /// </summary>
        /// <returns>版本号</returns>
        public string GetFullNumber()
        {
            return string.Format("{0}.{1}.{2}", MajorNumber, MinorNumber, ReviseNumber);
        }
    }

    /// <summary>
    /// 版本支持的平台
    /// </summary>
    internal enum Platform
    {
        PC = 1,
        Android = 2,
        WebGL = 3
    }
}