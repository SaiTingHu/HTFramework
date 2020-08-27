using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 类成员场景处理器特性
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public abstract class SceneHandlerAttribute : Attribute
    {

    }

    /// <summary>
    /// 移动手柄处理器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class MoveHandlerAttribute : SceneHandlerAttribute
    {
        public string Display { get; private set; }

        public MoveHandlerAttribute(string display = null)
        {
            Display = display;
        }
    }

    /// <summary>
    /// 半径手柄处理器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class RadiusHandlerAttribute : SceneHandlerAttribute
    {
        public string Display { get; private set; }

        public RadiusHandlerAttribute(string display = null)
        {
            Display = display;
        }
    }

    /// <summary>
    /// 包围盒处理器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class BoundsHandlerAttribute : SceneHandlerAttribute
    {
        public string Display { get; private set; }

        public BoundsHandlerAttribute(string display = null)
        {
            Display = display;
        }
    }
}