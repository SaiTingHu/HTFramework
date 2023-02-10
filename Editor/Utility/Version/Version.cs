using System;

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
        /// 发行日期
        /// </summary>
        public string ReleaseDate = null;
        /// <summary>
        /// 发行说明
        /// </summary>
        public string ReleaseNotes = "";
        /// <summary>
        /// 版本号支持的Unity版本
        /// </summary>
        public string UnityVersions = "2021.3.15";
        /// <summary>
        /// 版本号支持的.Net API版本
        /// </summary>
        public string APIVersions = ".NET Framework";
        
        /// <summary>
        /// 获取完整的版本号
        /// </summary>
        /// <returns>版本号</returns>
        public string GetFullNumber()
        {
            return $"{MajorNumber}.{MinorNumber}.{ReviseNumber}";
        }
    }
}