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
    public sealed class UIClickButton : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent OnLeftClick;
        public UnityEvent OnMiddleClick;
        public UnityEvent OnRightClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                OnLeftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                OnMiddleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                OnRightClick.Invoke();
        }
    }
}
