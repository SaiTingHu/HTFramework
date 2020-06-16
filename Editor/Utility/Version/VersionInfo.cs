using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 版本信息
    /// </summary>
    internal sealed class VersionInfo : ScriptableObject
    {
        /// <summary>
        /// 当前的版本号
        /// </summary>
        public Version CurrentVersion;
        /// <summary>
        /// 以往的所有版本号
        /// </summary>
        public List<Version> PreviousVersions = new List<Version>();
    }
}