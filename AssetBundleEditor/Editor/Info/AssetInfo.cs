using UnityEngine;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源
    /// </summary>
    public abstract class AssetInfo
    {
        /// <summary>
        /// 资源全路径
        /// </summary>
        public string FullPath;
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path;
        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 在AB包编辑面板中资源是否勾选
        /// </summary>
        public bool IsChecked;
        
        public AssetInfo(string fullPath, string name)
        {
            FullPath = fullPath;
            Path = "Assets" + fullPath.Replace(Application.dataPath.Replace("/", "\\"), "");
            Name = name;
            IsChecked = false;
        }
    }
}
