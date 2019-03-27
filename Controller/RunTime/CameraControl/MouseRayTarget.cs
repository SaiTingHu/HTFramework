using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 可捕获的射线目标
    /// </summary>
    [AddComponentMenu("HTFramework/Camera Control/Mouse Ray Target")]
    [DisallowMultipleComponent]
    public sealed class MouseRayTarget : MonoBehaviour
    {
        /// <summary>
        /// 目标显示的名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 目标是否为步骤目标
        /// </summary>
        public bool IsStepTarget = true;
    }
}
