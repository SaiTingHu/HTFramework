using System;
using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源文件
    /// </summary>
    public sealed class FileInfo : AssetInfo
    {
        /// <summary>
        /// 文件的GUID
        /// </summary>
        public string GUID;
        /// <summary>
        /// 文件类型
        /// </summary>
        public Type AssetType;
        /// <summary>
        /// 显式打入的AB包
        /// </summary>
        public BundleInfo Bundled;
        /// <summary>
        /// 隐式打入的AB包
        /// </summary>
        public Dictionary<BundleInfo, int> IndirectBundled;
        /// <summary>
        /// 依赖的资源文件
        /// </summary>
        public List<FileInfo> Dependencies;
        /// <summary>
        /// 被依赖的资源文件
        /// </summary>
        public List<FileInfo> BeDependencies;
        /// <summary>
        /// 是否是有效资源
        /// </summary>
        public bool IsValid;
        /// <summary>
        /// 是否是冗余资源
        /// </summary>
        public bool IsRedundant;
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string Extension;

        public FileInfo(string fullPath, string name, string extension) : base(fullPath, name)
        {
            GUID = AssetDatabase.AssetPathToGUID(Path);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(Path);
            Bundled = null;
            IndirectBundled = new Dictionary<BundleInfo, int>();
            Dependencies = new List<FileInfo>();
            BeDependencies = new List<FileInfo>();
            IsValid = AssetBundleTool.IsValidFile(extension);
            IsRedundant = false;
            Extension = extension;
        }
    }
}
