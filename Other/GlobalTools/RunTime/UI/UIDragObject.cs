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
    public class UIDragObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 被拖动目标
        /// </summary>
        public GameObject DragTarget;

        private bool _isDrag = false;
        private Vector3 _lastPos;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDrag = true;
            _lastPos = Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDrag = false;
        }

        private void Update()
        {
            if (_isDrag)
            {
                Vector3 direction = Input.mousePosition - _lastPos;
                DragTarget.transform.position += direction;
                _lastPos = Input.mousePosition;
            }
        }
    }
}
