using System.Collections.Generic;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源文件夹
    /// </summary>
    public sealed class FolderInfo : AssetInfo
    {
        /// <summary>
        /// 文件夹是否展开
        /// </summary>
        public bool IsExpanding;
        /// <summary>
        /// 文件夹是否为空
        /// </summary>
        public bool IsEmpty;
        /// <summary>
        /// 文件夹内的资源
        /// </summary>
        public List<AssetInfo> ChildAssetInfo;

        public FolderInfo(string fullPath, string name, string extension) : base(fullPath, name)
        {
            IsExpanding = false;
            IsEmpty = false;
            ChildAssetInfo = new List<AssetInfo>();
        }
    }
}
