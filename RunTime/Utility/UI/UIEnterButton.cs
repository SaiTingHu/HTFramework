using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UGUI靠近按钮
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIEnterButton")]
    [RequireComponent(typeof(Graphic))]
    [DisallowMultipleComponent]
    public sealed class UIEnterButton : HTBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        public UnityEvent OnMouseEnter;
        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        public UnityEvent OnMouseExit;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseExit.Invoke();
        }
    }
}
