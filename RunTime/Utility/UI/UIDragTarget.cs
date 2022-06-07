using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// UGUI可拖动目标
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIDragTarget")]
    public sealed class UIDragTarget : HTBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 拖动触发键
        /// </summary>
        public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;
        /// <summary>
        /// 启用水平拖动
        /// </summary>
        public bool Horizontal = true;
        /// <summary>
        /// 启用垂直拖动
        /// </summary>
        public bool Vertical = true;

        /// <summary>
        /// 是否拖拽中
        /// </summary>
        public bool IsDraging { get; private set; } = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == DragButton)
            {
                IsDraging = true;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (IsDraging)
            {
                transform.position += LimitDrag(eventData.delta);
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            IsDraging = false;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            IsDraging = false;
        }

        private Vector3 LimitDrag(Vector3 drag)
        {
            drag.x = Horizontal ? drag.x : 0;
            drag.y = Vertical ? drag.y : 0;
            drag.z = 0;
            return drag;
        }
    }
}