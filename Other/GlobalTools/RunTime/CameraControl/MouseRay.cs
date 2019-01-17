using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 鼠标位置发射射线捕获目标
    /// </summary>
    [RequireComponent(typeof(Camera))]
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

        private Camera _camera;
        private MouseRayTarget _rayTarget;
        private Ray _ray;
        private RaycastHit _hit;

        private static MouseRay _instance;
        public static MouseRay Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _instance = this;
        }

        private void Update()
        {
            if (IsOpenRay)
            {
                if (GlobalTools.IsPointerOverUGUI())
                {
                    RaycastHiting(null);
                    return;
                }

                _ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit, 100, ActivatedLayer))
                {
                    RaycastHiting(_hit.transform.GetComponent<MouseRayTarget>());
                }
                else
                {
                    RaycastHiting(null);
                }

                RaycastHitImageFlow();
            }
        }

        /// <summary>
        /// 射线击中目标
        /// </summary>
        private void RaycastHiting(MouseRayTarget target)
        {
            if (_rayTarget == target)
            {
                return;
            }

            if (_rayTarget)
            {
                switch (TriggerHighlighting)
                {
                    case HighlightingType.Normal:
                        _rayTarget.gameObject.CloseHighLight(true);
                        break;
                    case HighlightingType.Flash:
                        _rayTarget.gameObject.CloseFlashHighLight(true);
                        break;
                }
                _rayTarget = null;
            }

            _rayTarget = target;
            if (_rayTarget)
            {
                switch (TriggerHighlighting)
                {
                    case HighlightingType.Normal:
                        _rayTarget.gameObject.OpenHighLight(NormalColor);
                        break;
                    case HighlightingType.Flash:
                        _rayTarget.gameObject.OpenFlashHighLight(FlashColor1, FlashColor2);
                        break;
                }

                if (IsOpenPrompt)
                {
                    if (RayHitImage && RayHitText)
                    {
                        RayHitImage.gameObject.SetActive(true);
                        RayHitText.text = _rayTarget.Name;
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
            if (IsOpenPrompt && _rayTarget && RayHitImage && RayHitImage.gameObject.activeSelf)
            {
                RayHitImage.transform.position = _camera.WorldToScreenPoint(_rayTarget.transform.position) + new Vector3(0, 20, 0);
                RayHitImage.rectTransform.sizeDelta = new Vector2(RayHitText.rectTransform.sizeDelta.x + 40, RayHitImage.rectTransform.sizeDelta.y);
            }
        }

        /// <summary>
        /// 当前被射线捕获的目标
        /// </summary>
        public MouseRayTarget Target
        {
            get
            {
                return _rayTarget;
            }
        }

        /// <summary>
        /// 当前被射线捕获的目标
        /// </summary>
        public GameObject TargetObj
        {
            get
            {
                if (_rayTarget)
                    return _rayTarget.gameObject;
                else
                    return null;
            }
        }

        public enum HighlightingType
        {
            Normal,
            Flash,
        }
    }
}