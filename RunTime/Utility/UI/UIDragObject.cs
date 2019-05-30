using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UGUI可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIDragObject")]
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public sealed class UIDragObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 被拖动目标
        /// </summary>
        public GameObject DragTarget;
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

        private RectTransform _transform;
        private bool _isDrag = false;
        private Vector3 _lastPos;

        private void Awake()
        {
            _transform = DragTarget.rectTransform();
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
                Vector2 direction = Main.m_Input.MousePosition - _lastPos;
                if (!Horizontal) direction.x = 0;
                if (!Vertical) direction.y = 0;
                _transform.anchoredPosition += direction;
                LimitPos();
                _lastPos = Main.m_Input.MousePosition;
            }
        }

        private void LimitPos()
        {
            Vector2 pos = _transform.anchoredPosition;
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
            _transform.anchoredPosition = pos;
        }
    }
}
