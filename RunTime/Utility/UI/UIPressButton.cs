﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UGUI按住按钮
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIPressButton")]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public sealed class UIPressButton : HTBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 鼠标按住事件
        /// </summary>
        public UnityEvent OnMousePressed;
        /// <summary>
        /// 鼠标释放事件
        /// </summary>
        public UnityEvent OnMouseRelease;

        private bool _isPressed;

        private void Update()
        {
            if (_isPressed)
            {
                OnMousePressed.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPressed)
            {
                _isPressed = false;
                OnMouseRelease.Invoke();
            }
        }
    }
}