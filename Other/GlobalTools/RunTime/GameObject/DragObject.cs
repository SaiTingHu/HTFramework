using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 可拖动对象
    /// </summary>
    [AddComponentMenu("HTFramework/GameObject/DragObject")]
    [DisallowMultipleComponent]
    public sealed class DragObject : MonoBehaviour
    {
        private Vector3 _offset;

        private void OnMouseDown()
        {
            if (GlobalTools.IsPointerOverUGUI())
                return;

            Vector3 mousePoint = Main.m_Input.MousePosition;
            Vector3 screenPoint = Main.m_Controller.MainCamera.WorldToScreenPoint(transform.position);
            mousePoint.z = screenPoint.z = 0;
            _offset = screenPoint - mousePoint;
        }

        private void OnMouseDrag()
        {
            if (GlobalTools.IsPointerOverUGUI())
                return;

            Vector3 mousePoint = Main.m_Input.MousePosition;
            Vector3 screenPoint = Main.m_Controller.MainCamera.WorldToScreenPoint(transform.position);
            mousePoint.z = screenPoint.z;
            transform.position = Main.m_Controller.MainCamera.ScreenToWorldPoint(mousePoint + _offset);
        }
    }
}
