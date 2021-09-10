using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// UGUI可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIDragObject")]
    public sealed class UIDragObject : HTBehaviour, ICanvasRaycastFilter
    {
        /// <summary>
        /// 被拖动目标
        /// </summary>
        public GameObject DragTarget;
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
        /// 水平拖动位置限制
        /// </summary>
        public bool HorizontalLimit = false;
        /// <summary>
        /// 垂直拖动位置限制
        /// </summary>
        public bool VerticalLimit = false;
        /// <summary>
        /// 左边界
        /// </summary>
        public float Left = 0;
        /// <summary>
        /// 右边界
        /// </summary>
        public float Right = 0;
        /// <summary>
        /// 上边界
        /// </summary>
        public float Up = 0;
        /// <summary>
        /// 下边界
        /// </summary>
        public float Down = 0;

        private RectTransform _target;
        private RectTransform _targetParent;

        protected override void Awake()
        {
            base.Awake();

            _target = DragTarget.rectTransform();
            _targetParent = _target.parent.rectTransform();
        }
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            bool input = false;
            switch (DragButton)
            {
                case PointerEventData.InputButton.Left:
                    input = Main.m_Input.GetButton(InputButtonType.MouseLeft);
                    break;
                case PointerEventData.InputButton.Middle:
                    input = Main.m_Input.GetButton(InputButtonType.MouseMiddle);
                    break;
                case PointerEventData.InputButton.Right:
                    input = Main.m_Input.GetButton(InputButtonType.MouseRight);
                    break;
                default:
                    input = false;
                    break;
            }

            if (input)
            {
                Vector2 local;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_targetParent, screenPoint, eventCamera, out local))
                {
                    _target.anchoredPosition = LimitPos(local);
                }
            }
            return true;
        }
        private Vector2 LimitPos(Vector2 pos)
        {
            pos.x = Horizontal ? pos.x : _target.anchoredPosition.x;
            pos.y = Vertical ? pos.y : _target.anchoredPosition.y;
            if (Horizontal && HorizontalLimit)
            {
                pos.x = Mathf.Clamp(pos.x, Left, Right);
            }
            if (Vertical && VerticalLimit)
            {
                pos.y = Mathf.Clamp(pos.y, Down, Up);
            }
            return pos;
        }
    }
}