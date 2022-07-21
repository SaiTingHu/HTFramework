using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 鼠标位置射线发射器
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class MouseRay : HTBehaviour
    {
        /// <summary>
        /// 是否开启射线投射
        /// </summary>
        public bool IsOpenRay = true;
        /// <summary>
        /// 是否开启射中提示
        /// </summary>
        public bool IsOpenPrompt = true;
        /// <summary>
        /// 是否开启射中高亮
        /// </summary>
        public bool IsOpenHighlight = true;
        /// <summary>
        /// 射线投射的有效层
        /// </summary>
        public LayerMask ActivatedLayer;
        /// <summary>
        /// 射中目标的高亮方式
        /// </summary>
        public HighlightingType TriggerHighlighting = HighlightingType.Flash;
        /// <summary>
        /// 默认高亮颜色
        /// </summary>
        public Color NormalColor = Color.cyan;
        /// <summary>
        /// 闪光高亮颜色1
        /// </summary>
        public Color FlashColor1 = Color.red;
        /// <summary>
        /// 闪光高亮颜色2
        /// </summary>
        public Color FlashColor2 = Color.white;
        /// <summary>
        /// 轮廓发光强度
        /// </summary>
        public float OutlineIntensity = 1;
        /// <summary>
        /// 高亮组件是否自动销毁
        /// </summary>
        public bool IsAutoDie = false;
        /// <summary>
        /// 提示框背景（基于屏幕中心对齐，且父级中最好不要有偏移对齐的节点）
        /// </summary>
        public Image RayHitBG;
        /// <summary>
        /// 提示框文本（基于屏幕左侧对齐，父级为 RayHitBG，并使用 Content Size Fitter 进行水平自动布局）
        /// </summary>
        public Text RayHitText;
        /// <summary>
        /// 提示框UI类型
        /// </summary>
        public UIType RayHitImageType = UIType.Overlay;
        /// <summary>
        /// 提示框背景位置偏移（RayHitBG 的位置基于射线击中坐标的偏移值）
        /// </summary>
        public Vector2 BGPosOffset = Vector2.zero;
        /// <summary>
        /// 提示框背景宽度偏移（RayHitBG 的宽度基于 RayHitText 的宽度的偏移值）
        /// </summary>
        public float BGWidthOffset = 40;
        /// <summary>
        /// 射线投射事件(MouseRayTargetBase：当前射中的目标，Vector3：当前射中的点，Vector2：当前鼠标位置转换后的UGUI位置)
        /// </summary>
        public event HTFAction<MouseRayTargetBase, Vector3, Vector2> RayEvent;

        private Ray _ray;
        private RaycastHit _hit;
        private GameObject _rayTarget;
        private TargetType _rayTargetType;
        private Vector2 _rayHitBGPos;
        private Vector2 _rayHitBGSize;
        private ContentSizeFitter _rayHitTextFitter;
        
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
        /// 提示框文本自适应器
        /// </summary>
        private ContentSizeFitter RayHitTextFitter
        {
            get
            {
                if (_rayHitTextFitter == null)
                {
                    _rayHitTextFitter = RayHitText.GetComponent<ContentSizeFitter>();
                }
                return _rayHitTextFitter;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            if (IsOpenRay)
            {
                if (UIToolkit.IsStayUI)
                {
                    UIToolkit.CalculateCurrentFocused(Main.m_Input.MousePosition);
                    RaycastHiting(UIToolkit.CurrentFocused);
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

                Vector2 pos = Main.m_Input.MousePosition.ScreenToUGUIPosition(null, RayHitImageType);
                RaycastHitBGFlow(pos);
                RayEvent?.Invoke(Target, HitPoint, pos);
            }
        }
        /// <summary>
        /// 关闭射线投射
        /// </summary>
        public void CloseRay()
        {
            IsOpenRay = false;
            RaycastHiting(null);
            RayEvent?.Invoke(null, Vector3.zero, Vector2.zero);
            if (RayHitBG && RayHitBG.gameObject.activeSelf)
            {
                RayHitBG.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 射线击中目标
        /// </summary>
        private void RaycastHiting(GameObject target)
        {
            if (_rayTarget == target)
                return;

            if (_rayTarget)
            {
                if (_rayTargetType == TargetType.GameObject)
                {
                    switch (TriggerHighlighting)
                    {
                        case HighlightingType.Normal:
                            _rayTarget.CloseHighLight(IsAutoDie);
                            break;
                        case HighlightingType.Flash:
                            _rayTarget.CloseFlashHighLight(IsAutoDie);
                            break;
                        case HighlightingType.Outline:
                            _rayTarget.CloseMeshOutline(IsAutoDie);
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
                _rayTargetType = _rayTarget.rectTransform() ? TargetType.UI : TargetType.GameObject;

                if (IsOpenPrompt && Target.IsOpenPrompt)
                {
                    if (RayHitBG)
                    {
                        RayHitBG.gameObject.SetActive(true);
                        RayHitText.text = Target.Name;
                        if (RayHitTextFitter != null)
                        {
                            RayHitTextFitter.SetLayoutHorizontal();
                        }
                    }
                }
                else
                {
                    if (RayHitBG && RayHitBG.gameObject.activeSelf)
                    {
                        RayHitBG.gameObject.SetActive(false);
                    }
                }

                if (IsOpenHighlight && Target.IsOpenHighlight)
                {
                    if (_rayTargetType == TargetType.GameObject)
                    {
                        switch (TriggerHighlighting)
                        {
                            case HighlightingType.Normal:
                                _rayTarget.OpenHighLight(NormalColor);
                                break;
                            case HighlightingType.Flash:
                                _rayTarget.OpenFlashHighLight(FlashColor1, FlashColor2);
                                break;
                            case HighlightingType.Outline:
                                _rayTarget.OpenMeshOutline(NormalColor, OutlineIntensity);
                                break;
                        }
                    }
                }
            }
            else
            {
                if (RayHitBG && RayHitBG.gameObject.activeSelf)
                {
                    RayHitBG.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 射线击中目标的名字显示框跟随
        /// </summary>
        private void RaycastHitBGFlow(Vector2 pos)
        {
            if (IsOpenPrompt && Target && RayHitBG && RayHitBG.gameObject.activeSelf)
            {
                _rayHitBGPos.Set(pos.x + BGPosOffset.x, pos.y + BGPosOffset.y);
                _rayHitBGSize.Set(RayHitText.rectTransform.sizeDelta.x + BGWidthOffset, RayHitBG.rectTransform.sizeDelta.y);
                
                RayHitBG.rectTransform.anchoredPosition = _rayHitBGPos;
                RayHitBG.rectTransform.sizeDelta = _rayHitBGSize;
            }
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
            /// <summary>
            /// 轮廓发光
            /// </summary>
            Outline
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