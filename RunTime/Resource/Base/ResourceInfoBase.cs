namespace HT.Framework
{
    /// <summary>
    /// 资源信息基类
    /// </summary>
    public abstract class ResourceInfoBase
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

        public ResourceInfoBase(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }

        /// <summary>
        /// 获取资源的Resource全路径
        /// </summary>
        public string GetResourceFullPath()
        {
            return $"ResourcesPath: Resources/{ResourcePath}";
        }
        /// <summary>
        /// 获取资源的AssetBundle全路径
        /// </summary>
        public string GetAssetBundleFullPath(string assetBundleRootPath)
        {
            return $"AssetBundlePath: {assetBundleRootPath}{AssetBundleName}  AssetPath:{AssetPath}";
        }
    }
}