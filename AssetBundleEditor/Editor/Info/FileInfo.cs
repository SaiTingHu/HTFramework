using System;
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
        /// 所属AB包
        /// </summary>
        public BundleInfo Bundled;
        /// <summary>
        /// 文件是否是有效资源
        /// </summary>
        public bool IsValid;
        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string Extension;

        public FileInfo(string fullPath, string name, string extension) : base(fullPath, name)
        {
            GUID = AssetDatabase.AssetPathToGUID(Path);
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(Path);
            Bundled = null;
            IsValid = AssetBundleTool.IsValidFile(extension);
            Extension = extension;
        }
    }
}
