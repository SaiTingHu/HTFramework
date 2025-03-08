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
        public int Depth { get; private set; }

        /// <summary>
        /// UI资源标记（仅可标记UI逻辑类【UILogicBase】）
        /// </summary>
        /// <param name="assetBundleName">UI预制体打入的AB包名称</param>
        /// <param name="assetPath">UI预制体资源路径</param>
        /// <param name="resourcePath">UI预制体Resource路径（仅限Resource加载）</param>
        /// <param name="entityType">UI类型</param>
        /// <param name="depth">常驻UI的深度（depth == -1：不计算深度，且层级显示在所有计算深度UI之上；depth >= 0：计算深度，深度值越大显示层级越高）</param>
        public UIResourceAttribute(string assetBundleName, string assetPath, string resourcePath, UIType entityType = UIType.Overlay, int depth = -1)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
            EntityType = entityType;
            Depth = depth;
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