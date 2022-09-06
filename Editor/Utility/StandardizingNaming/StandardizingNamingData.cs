using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 标准化命名-配置数据集
    /// </summary>
    internal sealed class StandardizingNamingData : ScriptableObject
    {
        public string NameMatch = "#NAME#";
        public List<NamingSign> HierarchyNamingSigns = new List<NamingSign>();
        public List<NamingSign> ProjectNamingSigns = new List<NamingSign>();
    }

    /// <summary>
    /// 命名标记
    /// </summary>
    [Serializable]
    internal sealed class NamingSign
    {
        /// <summary>
        /// 目标类型名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 目标类型全称
        /// </summary>
        public string FullName;
        /// <summary>
        /// 命名标记
        /// </summary>
        public string Sign;

        /// <summary>
        /// 目标类型
        /// </summary>
        public Type Target { get; set; }

        public NamingSign(Type type, string sign)
        {
            Name = type != null ? type.Name : null;
            FullName = type != null ? type.FullName : null;
            Sign = sign;
        }
    }
}