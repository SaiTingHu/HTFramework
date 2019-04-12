namespace HT.Framework
{
    /// <summary>
    /// 数据集信息
    /// </summary>
    public sealed class DataSetInfo
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
        /// <summary>
        /// 数据集的Json数据
        /// </summary>
        public JsonData Data;

        public DataSetInfo(string assetBundleName, string assetPath, string resourcePath, JsonData data)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
            Data = data;
        }
    }
}