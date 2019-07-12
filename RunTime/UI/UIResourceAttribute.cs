using System;

namespace HT.Framework
{
    /// <summary>
    /// UI资源标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class UIResourceAttribute : Attribute
    {
        public string AssetBundleName { get; private set; }
        public string AssetPath { get; private set; }
        public string ResourcePath { get; private set; }
        public UIType EntityType { get; private set; }
        public string WorldUIDomainName { get; private set; }

        public UIResourceAttribute(string assetBundleName, string assetPath, string resourcePath, UIType entityType = UIType.Overlay, string worldUIDomainName = "World")
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
            EntityType = entityType;
            WorldUIDomainName = worldUIDomainName;
        }
    }

    /// <summary>
    /// UI类型
    /// </summary>
    public enum UIType
    {
        /// <summary>
        /// 屏幕UI
        /// </summary>
        Overlay,
        /// <summary>
        /// 摄像机UI
        /// </summary>
        Camera,
        /// <summary>
        /// 世界UI
        /// </summary>
        World
    }
}
