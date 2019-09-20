using UnityEngine;
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
    public sealed class UIPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnPressed;
        public UnityEvent OnRelease;

        private bool _isPressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPressed)
            {
                _isPressed = false;
                OnRelease.Invoke();
            }
        }

        private void Update()
        {
            if (_isPressed)
            {
                OnPressed.Invoke();
            }
        }
    }
}
