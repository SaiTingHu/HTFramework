using UnityEngine;
using UnityEngine.Events;

namespace HT.Framework
{
    /// <summary>
    /// 可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/GameObject/DragObject")]
    [DisallowMultipleComponent]
    public sealed class DragObject : HTBehaviour
    {
        /// <summary>
        /// 是否启用全局拖拽
        /// </summary>
        public static bool IsAllowDragEnvironment = true;

        /// <summary>
        /// 是否启用拖拽
        /// </summary>
        public bool IsAllowDrag = true;
        /// <summary>
        /// 开始拖拽事件
        /// </summary>
        public UnityEvent BeginDragEvent;
        /// <summary>
        /// 拖拽中事件
        /// </summary>
        public UnityEvent DragingEvent;
        /// <summary>
        /// 结束拖拽事件
        /// </summary>
        public UnityEvent EndDragEvent;

        private Vector3 _offset;
        private bool _isDraging = false;

        /// <summary>
        /// 是否可以拖拽
        /// </summary>
        public bool IsCanDrag
        {
            get
            {
                return IsAllowDragEnvironment && IsAllowDrag;
            }
        }
        
        private void OnMouseDown()
        {
            if (IsCanDrag)
            {
                if (UIToolkit.IsStayUI)
                    return;
                
                Vector3 mousePoint = Main.m_Input.MousePosition;
                Vector3 screenPoint = Main.m_Controller.MainCamera.WorldToScreenPoint(transform.position);
                mousePoint.z = screenPoint.z = 0;
                _offset = screenPoint - mousePoint;
            }
        }
        private void OnMouseDrag()
        {
            if (IsCanDrag)
            {
                if (UIToolkit.IsStayUI)
                    return;

                Vector3 mousePoint = Main.m_Input.MousePosition;
                Vector3 screenPoint = Main.m_Controller.MainCamera.WorldToScreenPoint(transform.position);
                mousePoint.z = screenPoint.z;
                transform.position = Main.m_Controller.MainCamera.ScreenToWorldPoint(mousePoint + _offset);

                if (!_isDraging)
                {
                    _isDraging = true;
                    BeginDragEvent.Invoke();
                }
                else
                {
                    DragingEvent.Invoke();
                }
            }
        }
        private void OnMouseUp()
        {
            if (IsCanDrag)
            {
                _offset = Vector3.zero;

                if (_isDraging)
                {
                    _isDraging = false;
                    EndDragEvent.Invoke();
                }
            }
        }
    }
}