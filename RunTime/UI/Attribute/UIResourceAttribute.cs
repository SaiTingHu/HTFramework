using System;

namespace HT.Framework
{
    /// <summary>
    /// UI资源标记（仅可标记UI逻辑类【UILogicBase】）
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class UIResourceAttribute : Attribute
    {
        public string AssetBundleName { get; private set; }
        public string AssetPath { get; private set; }
        public string ResourcePath { get; private set; }
        public UIType EntityType { get; private set; }
        public string WorldUIDomainName { get; private set; }

        /// <summary>
        /// UI资源标记（仅可标记UI逻辑类【UILogicBase】）
        /// </summary>
        /// <param name="assetBundleName">UI预制体打入的AB包名称</param>
        /// <param name="assetPath">UI预制体资源路径</param>
        /// <param name="resourcePath">UI预制体Resource路径（仅限Resource加载）</param>
        /// <param name="entityType">UI类型</param>
        /// <param name="worldUIDomainName">UI所属域名（仅限World类型）</param>
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
        Camera
    }
}