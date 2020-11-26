using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UGUI点击按钮
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIClickButton")]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public sealed class UIClickButton : HTBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 启用左键双击事件
        /// </summary>
        public bool IsEnableDoubleClick = false;
        /// <summary>
        /// 左键双击时间间隔
        /// </summary>
        public float DoubleClickInterval = 0.5f;
        /// <summary>
        /// 鼠标左键点击事件
        /// </summary>
        public UnityEvent OnMouseLeftClick;
        /// <summary>
        /// 鼠标中键点击事件
        /// </summary>
        public UnityEvent OnMouseMiddleClick;
        /// <summary>
        /// 鼠标右键点击事件
        /// </summary>
        public UnityEvent OnMouseRightClick;
        /// <summary>
        /// 鼠标左键双击事件
        /// </summary>
        public UnityEvent OnMouseLeftDoubleClick;

        private float _timer = 0;

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (IsEnableDoubleClick)
                    {
                        if (_timer <= 0)
                        {
                            _timer = DoubleClickInterval;
                        }
                        else
                        {
                            _timer = 0;
                            OnMouseLeftDoubleClick.Invoke();
                        }
                    }
                    else
                    {
                        OnMouseLeftClick.Invoke();
                    }
                    break;
                case PointerEventData.InputButton.Middle:
                    OnMouseMiddleClick.Invoke();
                    break;
                case PointerEventData.InputButton.Right:
                    OnMouseRightClick.Invoke();
                    break;
            }
        }

        private void Update()
        {
            if (IsEnableDoubleClick)
            {
                if (_timer > 0)
                    _timer -= Time.deltaTime;
            }
        }
    }
}