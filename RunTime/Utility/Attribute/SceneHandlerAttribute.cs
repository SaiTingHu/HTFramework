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
    /// 移动手柄处理器（支持 Vector2、Vector3 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class MoveHandlerAttribute : SceneHandlerAttribute
    {
#if UNITY_EDITOR
        public string Display { get; private set; }
#endif

        /// <summary>
        /// 移动手柄处理器（支持 Vector2、Vector3 类型）
        /// </summary>
        /// <param name="display">显示名称</param>
        public MoveHandlerAttribute(string display = null)
        {
#if UNITY_EDITOR
            Display = display;
#endif
        }
    }

    /// <summary>
    /// 半径手柄处理器（支持 float、int 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class RadiusHandlerAttribute : SceneHandlerAttribute
    {
#if UNITY_EDITOR
        public string Display { get; private set; }
#endif

        /// <summary>
        /// 半径手柄处理器（支持 float、int 类型）
        /// </summary>
        /// <param name="display">显示名称</param>
        public RadiusHandlerAttribute(string display = null)
        {
#if UNITY_EDITOR
            Display = display;
#endif
        }
    }

    /// <summary>
    /// 包围盒处理器（支持 Bounds 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class BoundsHandlerAttribute : SceneHandlerAttribute
    {
#if UNITY_EDITOR
        public string Display { get; private set; }
#endif

        /// <summary>
        /// 包围盒处理器（支持 Bounds 类型）
        /// </summary>
        /// <param name="display">显示名称</param>
        public BoundsHandlerAttribute(string display = null)
        {
#if UNITY_EDITOR
            Display = display;
#endif
        }
    }

    /// <summary>
    /// 方向处理器（支持 Vector2、Vector3 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class DirectionHandlerAttribute : SceneHandlerAttribute
    {
#if UNITY_EDITOR
        public bool IsDynamic { get; private set; }
#endif

        /// <summary>
        /// 方向处理器（支持 Vector2、Vector3 类型）
        /// </summary>
        /// <param name="isDynamic">是否动态模式</param>
        public DirectionHandlerAttribute(bool isDynamic = false)
        {
#if UNITY_EDITOR
            IsDynamic = isDynamic;
#endif
        }
    }

    /// <summary>
    /// 圆形区域处理器（支持 float 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class CircleAreaHandlerAttribute : SceneHandlerAttribute
    {
#if UNITY_EDITOR
        public Axis Direction { get; private set; }
        public bool IsDynamic { get; private set; }
#endif

        /// <summary>
        /// 圆形区域处理器（支持 float 类型）
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="isDynamic">是否动态模式</param>
        public CircleAreaHandlerAttribute(Axis direction = Axis.Y, bool isDynamic = false)
        {
#if UNITY_EDITOR
            Direction = direction;
            IsDynamic = isDynamic;
#endif
        }

        public enum Axis
        {
            X,
            Y,
            Z
        }
    }
}