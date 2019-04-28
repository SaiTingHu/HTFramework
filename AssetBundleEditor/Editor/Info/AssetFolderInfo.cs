using System.Collections.Generic;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源文件夹
    /// </summary>
    public sealed class AssetFolderInfo : AssetInfoBase
    {
        /// <summary>
        /// 文件夹是否展开
        /// </summary>
        public bool IsExpanding;
        /// <summary>
        /// 文件夹内的资源
        /// </summary>
        public List<AssetInfoBase> ChildAssetInfo;

        /// <summary>
        /// 是否已读取子级资源
        /// </summary>
        private bool _isReadChildAsset = false;

        public AssetFolderInfo(string fullPath, string name) : base(fullPath, name)
        {
            IsExpanding = false;
            ChildAssetInfo = new List<AssetInfoBase>();
        }

        /// <summary>
        /// 读取子级资源
        /// </summary>
        public void ReadChildAsset()
        {
            if (!_isReadChildAsset)
            {
                _isReadChildAsset = true;
                AssetBundleEditorUtility.ReadAssetsInChildren(this);
            }
        }
    }
}
