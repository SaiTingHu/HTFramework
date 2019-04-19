using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace HT.Framework
{
    /// <summary>
    /// UGUI靠近按钮
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIEnterButton")]
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public sealed class UIEnterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnEnter;
        public UnityEvent OnExit;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExit.Invoke();
        }
    }
}
