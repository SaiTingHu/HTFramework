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
        /// Asset的路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath;

        public PrefabInfo(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }

        public PrefabInfo(UIResourceAttribute att)
        {
            AssetBundleName = att.AssetBundleName;
            AssetPath = att.AssetPath;
            ResourcePath = att.ResourcePath;
        }
    }
}
