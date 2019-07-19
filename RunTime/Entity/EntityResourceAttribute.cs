using System;

namespace HT.Framework
{
    /// <summary>
    /// 实体资源标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EntityResourceAttribute : Attribute
    {
        public string AssetBundleName { get; private set; }
        public string AssetPath { get; private set; }
        public string ResourcePath { get; private set; }

        public EntityResourceAttribute(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
    }
}
