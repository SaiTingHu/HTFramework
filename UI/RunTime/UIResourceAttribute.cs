using System;

namespace HT.Framework
{
    /// <summary>
    /// UI资源标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class UIResourceAttribute : Attribute
    {
        public string AssetBundleName;
        public string AssetPath;
        public string ResourcePath;

        public UIResourceAttribute(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
    }
}
