using UnityEngine;
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

        /// <summary>
        /// 射线发射摄像机
        /// </summary>
        public Camera RayCamera { get; set; }

        /// <summary>
        /// 当前被射线捕获的目标
        /// </summary>
        public MouseRayTarget Target { get; private set; }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (IsOpenRay)
            {
                if (GlobalTools.IsPointerOverUGUI())
                {
                    RaycastHiting(null);
                    return;
                }

                _ray = RayCamera.ScreenPointToRay(Input.mousePosition);
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
            if (Target == target)
            {
                return;
            }

            if (Target)
            {
                switch (TriggerHighlighting)
                {
                    case HighlightingType.Normal:
                        Target.gameObject.CloseHighLight(true);
                        break;
                    case HighlightingType.Flash:
                        Target.gameObject.CloseFlashHighLight(true);
                        break;
                }
                Target = null;
            }

            Target = target;
            if (Target)
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
                RayHitImage.transform.position = Input.mousePosition + new Vector3(0, 20, 0);
                RayHitImage.rectTransform.sizeDelta = new Vector2(RayHitText.rectTransform.sizeDelta.x + 40, RayHitImage.rectTransform.sizeDelta.y);
            }
        }
        
        public enum HighlightingType
        {
            Normal,
            Flash,
        }
    }
}