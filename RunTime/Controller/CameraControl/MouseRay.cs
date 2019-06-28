using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 鼠标位置发射射线捕获目标
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MouseRay : MonoBehaviour
    {
        public bool IsOpenRay = true;
        public bool IsOpenPrompt = true;
        public LayerMask ActivatedLayer;
        public HighlightingType TriggerHighlighting = HighlightingType.Flash;
        public Color NormalColor = Color.cyan;
        public Color FlashColor1 = Color.red;
        public Color FlashColor2 = Color.white;
        public Image RayHitImage;
        public Text RayHitText;

        private Ray _ray;
        private RaycastHit _hit;
        private GameObject _rayTarget;
        private TargetType _rayTargetType;

        private PointerEventData _eventData;
        private List<RaycastResult> _results = new List<RaycastResult>();

        /// <summary>
        /// 射线发射摄像机
        /// </summary>
        public Camera RayCamera { get; set; }

        /// <summary>
        /// 当前被射线捕获的目标
        /// </summary>
        public MouseRayTargetBase Target { get; private set; }

        /// <summary>
        /// 当前被射线击中的点
        /// </summary>
        public Vector3 HitPoint { get; private set; }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (IsOpenRay)
            {
                if (GlobalTools.IsPointerOverUGUI())
                {
                    RaycastHiting(GetCurrentUGUI());
                }
                else
                {
                    _ray = RayCamera.ScreenPointToRay(Main.m_Input.MousePosition);
                    if (Physics.Raycast(_ray, out _hit, 100, ActivatedLayer))
                    {
                        HitPoint = _hit.point;
                        RaycastHiting(_hit.transform.gameObject);
                    }
                    else
                    {
                        RaycastHiting(null);
                    }
                }

                RaycastHitImageFlow();
            }
        }

        /// <summary>
        /// 射线击中目标
        /// </summary>
        private void RaycastHiting(GameObject target)
        {
            if (_rayTarget == target)
            {
                return;
            }

            if (_rayTarget)
            {
                if (_rayTargetType == TargetType.GameObject)
                {
                    switch (TriggerHighlighting)
                    {
                        case HighlightingType.Normal:
                            _rayTarget.CloseHighLight(true);
                            break;
                        case HighlightingType.Flash:
                            _rayTarget.CloseFlashHighLight(true);
                            break;
                    }
                }
                Target = null;
                _rayTarget = null;
            }

            if (target && target.GetComponent<MouseRayTargetBase>())
            {
                Target = target.GetComponent<MouseRayTargetBase>();
                _rayTarget = target;
                _rayTargetType = _rayTarget.GetComponent<RectTransform>() ? TargetType.UI : TargetType.GameObject;

                if (_rayTargetType == TargetType.GameObject)
                {
                    switch (TriggerHighlighting)
                    {
                        case HighlightingType.Normal:
                            Target.gameObject.OpenHighLight(NormalColor);
                            break;
                        case HighlightingType.Flash:
                            Target.gameObject.OpenFlashHighLight(FlashColor1, FlashColor2);
                            break;
                    }
                }

                if (IsOpenPrompt)
                {
                    if (RayHitImage && RayHitText)
                    {
                        RayHitImage.gameObject.SetActive(true);
                        RayHitText.text = Target.Name;
                    }
                }
            }
            else
            {
                if (IsOpenPrompt)
                {
                    if (RayHitImage && RayHitText)
                    {
                        RayHitImage.gameObject.SetActive(false);
                        RayHitText.text = "";
                    }
                }
            }
        }

        /// <summary>
        /// 射线击中目标的名字显示框跟随
        /// </summary>
        private void RaycastHitImageFlow()
        {
            if (IsOpenPrompt && Target && RayHitImage && RayHitImage.gameObject.activeSelf)
            {
                RayHitImage.transform.position = Main.m_Input.MousePosition + new Vector3(0, 40, 0);
                RayHitImage.rectTransform.sizeDelta = new Vector2(RayHitText.rectTransform.sizeDelta.x + 40, RayHitImage.rectTransform.sizeDelta.y);
            }
        }

        /// <summary>
        /// 获取当前鼠标位置的UGUI控件
        /// </summary>
        private GameObject GetCurrentUGUI()
        {
            if (_eventData == null) _eventData = new PointerEventData(EventSystem.current);

            _eventData.position = Main.m_Input.MousePosition;
            EventSystem.current.RaycastAll(_eventData, _results);

            if (_results.Count > 0)
            {
                return _results[0].gameObject;
            }
            return null;
        }
        
        /// <summary>
        /// 高光类型
        /// </summary>
        public enum HighlightingType
        {
            /// <summary>
            /// 默认
            /// </summary>
            Normal,
            /// <summary>
            /// 闪光
            /// </summary>
            Flash,
        }

        /// <summary>
        /// 射线捕获的目标类型
        /// </summary>
        public enum TargetType
        {
            /// <summary>
            /// 物体
            /// </summary>
            GameObject,
            /// <summary>
            /// UI对象
            /// </summary>
            UI,
        }
    }
}