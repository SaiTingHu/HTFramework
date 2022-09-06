namespace HT.Framework
{
    /// <summary>
    /// 预制体信息
    /// </summary>
    public sealed class PrefabInfo : ResourceInfoBase
    {
        public PrefabInfo(string assetBundleName, string assetPath, string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {

        }
        public PrefabInfo(UIResourceAttribute att) : base(att.AssetBundleName, att.AssetPath, att.ResourcePath)
        {

        }
        public PrefabInfo(EntityResourceAttribute att) : base(att.AssetBundleName, att.AssetPath, att.ResourcePath)
        {

        }
    }
}