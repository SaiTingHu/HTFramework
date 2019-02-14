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
        public string AssetBundlePath;
        public string ResourcePath;

        public UIResourceAttribute(string assetBundleName, string assetBundlePath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetBundlePath = assetBundlePath;
            ResourcePath = resourcePath;
        }
    }
}
