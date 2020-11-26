using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// UGUI可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIDragObject")]
    [DisallowMultipleComponent]
    public sealed class UIDragObject : HTBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 被拖动目标
        /// </summary>
        public GameObject DragTarget;
        /// <summary>
        /// 拖动模式
        /// </summary>
        public UIType Mode = UIType.Overlay;
        /// <summary>
        /// 水平拖动
        /// </summary>
        public bool Horizontal = true;
        /// <summary>
        /// 垂直拖动
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

        private Transform _transform;
        private RectTransform _rectTransform;
        private bool _isDrag = false;
        private Vector3 _lastPos;
        private HTFAction Draging;

        protected override void Awake()
        {
            base.Awake();

            _transform = DragTarget.transform;
            _rectTransform = DragTarget.rectTransform();

            switch (Mode)
            {
                case UIType.Overlay:
                    Draging = OverlayDraging;
                    break;
                case UIType.Camera:
                case UIType.World:
                    Draging = WorldDraging;
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDrag = true;
            _lastPos = Main.m_Input.MousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDrag = false;
        }

        private void Update()
        {
            if (_isDrag)
            {
                Draging();
            }
        }

        private void WorldDraging()
        {
            Vector2 direction = Main.m_Input.MousePosition - _lastPos;
            if (!Horizontal) direction.x = 0;
            if (!Vertical) direction.y = 0;
            _rectTransform.anchoredPosition += direction;
            WorldLimitPos();
            _lastPos = Main.m_Input.MousePosition;
        }

        private void OverlayDraging()
        {
            Vector3 direction = Main.m_Input.MousePosition - _lastPos;
            if (!Horizontal) direction.x = 0;
            if (!Vertical) direction.y = 0;
            _transform.position += direction;
            OverlayLimitPos();
            _lastPos = Main.m_Input.MousePosition;
        }

        private void WorldLimitPos()
        {
            Vector2 pos = _rectTransform.anchoredPosition;
            if (HorizontalLimit)
            {
                if (pos.x < Left) pos.x = Left;
                else if (pos.x > Right) pos.x = Right;
            }
            if (VerticalLimit)
            {
                if (pos.y < Down) pos.y = Down;
                else if (pos.y > Up) pos.y = Up;
            }
            _rectTransform.anchoredPosition = pos;
        }

        private void OverlayLimitPos()
        {
            Vector2 pos = _transform.position;
            if (HorizontalLimit)
            {
                if (pos.x < Left) pos.x = Left;
                else if (pos.x > Right) pos.x = Right;
            }
            if (VerticalLimit)
            {
                if (pos.y < Down) pos.y = Down;
                else if (pos.y > Up) pos.y = Up;
            }
            _transform.position = pos;
        }
    }
}