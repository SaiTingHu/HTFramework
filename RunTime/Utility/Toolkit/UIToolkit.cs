using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UI工具箱
    /// </summary>
    public static class UIToolkit
    {
        private static PointerEventData EventData;
        private static List<RaycastResult> Results = new List<RaycastResult>();

        /// <summary>
        /// 当前鼠标是否停留在UI控件上
        /// </summary>
        public static bool IsStayUI
        {
            get
            {
                if (EventSystem.current)
                {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                    if (Input.touchCount > 0)
                    {
                        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    }
                    else
                    {
                        return false;
                    }
#else
                    return EventSystem.current.IsPointerOverGameObject();
#endif
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 当前鼠标是否停留在UI控件上（非WorldSpace类型的UI，必须启用鼠标射线投射：Main.m_Controller.EnableMouseRay = true）
        /// </summary>
        public static bool IsStayUINotWorld
        {
            get
            {
                if (IsStayUI)
                {
                    if (CurrentFocused)
                    {
                        Canvas canvas = CurrentFocused.GetComponentInParent<Canvas>();
                        if (canvas && canvas.renderMode != RenderMode.WorldSpace)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 当前鼠标焦点的UI控件
        /// </summary>
        internal static GameObject CurrentFocused { get; private set; }

        /// <summary>
        /// 屏幕坐标转换为UGUI坐标（只针对框架UI模块下的UI控件）
        /// </summary>
        /// <param name="position">屏幕坐标</param>
        /// <param name="reference">参照物（要赋值的UGUI控件的根物体）</param>
        /// <param name="uIType">UI类型</param>
        /// <returns>基于参照物的局部UGUI坐标</returns>
        public static Vector2 ScreenToUGUIPosition(this Vector3 position, RectTransform reference = null, UIType uIType = UIType.Overlay)
        {
            Vector2 anchoredPos = Vector2.zero;
            if (position.z < 0)
            {
                anchoredPos.Set(-100000, -100000);
            }
            else
            {
                position.z = 0;
                switch (uIType)
                {
                    case UIType.Overlay:
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.OverlayUIRoot, position, null, out anchoredPos);
                        break;
                    case UIType.Camera:
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.CameraUIRoot, position, Main.m_UI.UICamera, out anchoredPos);
                        break;
                }
            }
            return anchoredPos;
        }
        /// <summary>
        /// 获取输入框的int类型值，若不是该类型值，则返回-1
        /// </summary>
        /// <param name="input">输入框</param>
        /// <returns>值</returns>
        public static int IntText(this InputField input)
        {
            int value = -1;
            int.TryParse(input.text, out value);
            return value;
        }
        /// <summary>
        /// 获取输入框的float类型值，若不是该类型值，则返回float.NaN
        /// </summary>
        /// <param name="input">输入框</param>
        /// <returns>值</returns>
        public static float FloatText(this InputField input)
        {
            float value = -1f;
            if (float.TryParse(input.text, out value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }
        /// <summary>
        /// 设置下拉框值，若该下拉框不存在该值，则无效
        /// </summary>
        /// <param name="dropdown">下拉框</param>
        /// <param name="value">目标值</param>
        public static void SetValue(this Dropdown dropdown, string value)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == value)
                {
                    dropdown.value = i;
                    return;
                }
            }
        }
        /// <summary>
        /// 计算当前鼠标焦点的UI控件
        /// </summary>
        internal static void CalculateCurrentFocused(Vector3 mousePosition)
        {
            if (IsStayUI)
            {
                if (EventData == null) EventData = new PointerEventData(EventSystem.current);

                EventData.position = mousePosition;
                EventSystem.current.RaycastAll(EventData, Results);
                CurrentFocused = Results.Count > 0 ? Results[0].gameObject : null;
            }
            else
            {
                CurrentFocused = null;
            }
        }
    }
}