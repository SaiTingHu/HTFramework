namespace HT.Framework
{
    /// <summary>
    /// 预制体
    /// </summary>
    public class PrefabInfo
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName;
        /// <summary>
        /// AssetBundle中的路径
        /// </summary>
        public string AssetBundlePath;
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath;

        public PrefabInfo(string assetBundleName, string assetBundlePath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetBundlePath = assetBundlePath;
            ResourcePath = resourcePath;
        }

        public PrefabInfo(UIResourceAttribute att)
        {
            AssetBundleName = att.AssetBundleName;
            AssetBundlePath = att.AssetBundlePath;
            ResourcePath = att.ResourcePath;
        }
    }
}
