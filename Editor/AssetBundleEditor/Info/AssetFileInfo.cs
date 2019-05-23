using System;
using System.Collections.Generic;
using System.IO;
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
        /// 文件对象
        /// </summary>
        public UnityEngine.Object AssetObject;
        /// <summary>
        /// 文件占硬盘内存大小
        /// </summary>
        public long MemorySize;
        /// <summary>
        /// 文件占硬盘内存大小（显示格式）
        /// </summary>
        public string MemorySizeFormat;
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
            AssetObject = AssetDatabase.LoadAssetAtPath(AssetPath, AssetType);
            MemorySize = new FileInfo(FullPath).Length;
            MemorySizeFormat = EditorUtility.FormatBytes(MemorySize);
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

        /// <summary>
        /// 刷新冗余状态
        /// </summary>
        public void UpdateRedundantState()
        {
            if (Bundled != "")
            {
                if (IndirectBundled.Count < 1)
                {
                    IsRedundant = false;
                }
                else if (IndirectBundled.Count == 1)
                {
                    IsRedundant = !IndirectBundled.ContainsKey(Bundled);
                }
                else if (IndirectBundled.Count > 1)
                {
                    IsRedundant = true;
                }
            }
            else
            {
                IsRedundant = IndirectBundled.Count > 1;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
