using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 鼠标射线可捕获的物体目标
    /// </summary>
    [AddComponentMenu("HTFramework/Camera Control/Mouse Ray Target")]
    public sealed class MouseRayTarget : MouseRayTargetBase
    {
        /// <summary>
        /// 是否开启鼠标左键双击时焦点目标的功能
        /// </summary>
        public bool IsDoubleClickFocus = false;
    }
}