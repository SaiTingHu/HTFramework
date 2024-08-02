namespace HT.Framework
{
    /// <summary>
    /// 数据集信息标记
    /// </summary>
    public sealed class DataSetInfo : ResourceInfoBase
    {
        /// <summary>
        /// 数据集的Json数据
        /// </summary>
        public JsonData Data;

        public DataSetInfo(string assetBundleName, string assetPath, string resourcePath, JsonData data) : base(assetBundleName, assetPath, resourcePath)
        {
            Data = data;
        }
    }
}