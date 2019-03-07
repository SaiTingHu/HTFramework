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
        public string Name;
    }
}
