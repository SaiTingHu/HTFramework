namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 资源基类
    /// </summary>
    public abstract class AssetInfoBase
    {
        /// <summary>
        /// 资源全路径
        /// </summary>
        public string FullPath;
        /// <summary>
        /// 资源路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name;

        public AssetInfoBase(string fullPath, string name)
        {
            FullPath = fullPath;
            AssetPath = AssetBundleEditorUtility.GetAssetPathByFullPath(fullPath);
            Name = name;
        }
    }
}
