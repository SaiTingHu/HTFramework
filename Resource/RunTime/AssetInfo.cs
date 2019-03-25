namespace HT.Framework
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public sealed class AssetInfo
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName;
        /// <summary>
        /// Asset的路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath;

        public AssetInfo(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
    }
}
