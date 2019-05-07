using System;
using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源文件
    /// </summary>
    public sealed class AssetFileInfo : AssetInfoBase
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public Type AssetType;
        /// <summary>
        /// 显式打入的AB包
        /// </summary>
        public string Bundled;
        /// <summary>
        /// 隐式打入的AB包【AB包名称，打入次数】
        /// </summary>
        public Dictionary<string, int> IndirectBundled;
        /// <summary>
        /// 隐式打入的AB包关系【被隐式打入的依赖资源路径，AB包名称】
        /// </summary>
        public Dictionary<string, string> IndirectBundledRelation;
        /// <summary>
        /// 依赖的资源文件
        /// </summary>
        public List<string> Dependencies;
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

        /// <summary>
        /// 是否已读取依赖资源文件
        /// </summary>
        private bool _isReadDependenciesFile = false;

        public AssetFileInfo(string fullPath, string name, string extension) : base(fullPath, name)
        {
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(AssetPath);
            Bundled = "";
            IndirectBundled = new Dictionary<string, int>();
            IndirectBundledRelation = new Dictionary<string, string>();
            Dependencies = new List<string>();
            IsValid = AssetBundleEditorUtility.IsValidFile(extension);
            IsRedundant = false;
            Extension = extension;
        }

        /// <summary>
        /// 读取依赖的资源文件
        /// </summary>
        public void ReadDependenciesFile()
        {
            if (!_isReadDependenciesFile)
            {
                _isReadDependenciesFile = true;
                AssetBundleEditorUtility.ReadAssetDependencies(AssetPath);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
