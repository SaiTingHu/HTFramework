using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// UGUI可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIDragObject")]
    [DisallowMultipleComponent]
    public sealed class UIDragObject : HTBehaviour, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        private Vector3 _delta;
        private HTFAction _draging;

        protected override void Awake()
        {
            base.Awake();

            _transform = DragTarget.transform;
            _rectTransform = DragTarget.rectTransform();

            switch (Mode)
            {
                case UIType.Overlay:
                    _draging = OverlayDraging;
                    break;
                case UIType.Camera:
                case UIType.World:
                    _draging = WorldDraging;
                    break;
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDrag = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _delta = eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDrag = false;
        }

        private void Update()
        {
            if (_isDrag)
            {
                _draging();
            }
        }

        private void WorldDraging()
        {
            if (!Horizontal) _delta.x = 0;
            if (!Vertical) _delta.y = 0;
            _rectTransform.anchoredPosition3D += _delta;
            _delta = Vector3.zero;
            WorldLimitPos();
        }

        private void OverlayDraging()
        {
            if (!Horizontal) _delta.x = 0;
            if (!Vertical) _delta.y = 0;
            _transform.position += _delta;
            _delta = Vector3.zero;
            OverlayLimitPos();
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