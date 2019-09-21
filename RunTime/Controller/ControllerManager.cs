using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 操作控制者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ControllerManager : ModuleManagerBase
    {
        /// <summary>
        /// 切换至自由控制事件
        /// </summary>
        public event HTFAction SwitchToFreeControlEvent;
        /// <summary>
        /// 切换至第一人称事件
        /// </summary>
        public event HTFAction SwitchToFirstPersonEvent;
        /// <summary>
        /// 切换至第三人称事件
        /// </summary>
        public event HTFAction SwitchToThirdPersonEvent;
        /// <summary>
        /// 射线投射事件(MouseRayTargetBase：当前射中的目标，Vector3：当前射中的点，Vector2：当前鼠标位置转换后的UGUI坐标)
        /// </summary>
        public event HTFAction<MouseRayTargetBase, Vector3, Vector2> RayEvent;

        /// <summary>
        /// 默认的控制模式【请勿在代码中修改】
        /// </summary>
        public ControlMode DefaultControlMode = ControlMode.FreeControl;
        /// <summary>
        /// Dotween动画的默认缓动类型【请勿在代码中修改】
        /// </summary>
        public Ease DefaultEase = Ease.Linear;
        /// <summary>
        /// Dotween动画的默认自动启动方式【请勿在代码中修改】
        /// </summary>
        public AutoPlay DefaultAutoPlay = AutoPlay.All;
        /// <summary>
        /// Dotween动画是否自动销毁【请勿在代码中修改】
        /// </summary>
        public bool IsAutoKill = true;
        
        private CameraTarget _cameraTarget;
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private MouseRay _mouseRay;
        private HighlightingEffect _highlightingEffect;
        private Dictionary<MouseRayTargetBase, HTFAction> _mouseClickTargets = new Dictionary<MouseRayTargetBase, HTFAction>();
        private ControlMode _controlMode;

        public override void OnInitialization()
        {
            base.OnInitialization();

            DOTween.defaultEaseType = DefaultEase;
            DOTween.defaultAutoPlay = DefaultAutoPlay;
            DOTween.defaultAutoKill = IsAutoKill;

            MainCamera = transform.GetComponentByChild<Camera>("MainCamera");
            _cameraTarget = transform.GetComponentByChild<CameraTarget>("CameraTarget");
            _mousePosition = MainCamera.GetComponent<MousePosition>();
            _mouseRotation = MainCamera.GetComponent<MouseRotation>();
            _mouseRay = MainCamera.GetComponent<MouseRay>();
            _highlightingEffect = MainCamera.GetComponent<HighlightingEffect>();

            _mousePosition.Target = _cameraTarget;
            _mousePosition.MR = _mouseRotation;
            _mouseRotation.Target = _cameraTarget;
            _mouseRay.RayCamera = MainCamera;
            _mouseRay.RayEvent += OnRay;
        }

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            TheControlMode = DefaultControlMode;
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            _mouseRay.OnRefresh();
            switch (TheControlMode)
            {
                case ControlMode.FreeControl:
                    _mousePosition.OnRefresh();
                    _mouseRotation.OnRefresh();
                    break;
            }

            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
            {
                if (RayTarget != null)
                {
                    if (_mouseClickTargets.ContainsKey(RayTarget))
                    {
                        _mouseClickTargets[RayTarget]?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 主摄像机
        /// </summary>
        public Camera MainCamera { get; private set; }
        /// <summary>
        /// 控制模式
        /// </summary>
        public ControlMode TheControlMode
        {
            set
            {
                if (_controlMode != value)
                {
                    _controlMode = value;
                    switch (_controlMode)
                    {
                        case ControlMode.FreeControl:
                            SwitchToFreeControlEvent?.Invoke();
                            break;
                        case ControlMode.FirstPerson:
                            SwitchToFirstPersonEvent?.Invoke();
                            break;
                        case ControlMode.ThirdPerson:
                            SwitchToThirdPersonEvent?.Invoke();
                            break;
                    }
                }
            }
            get
            {
                return _controlMode;
            }
        }
        /// <summary>
        /// 自由控制：是否限制控制外围
        /// </summary>
        public bool NeedLimit
        {
            set
            {
                _mousePosition.NeedLimit = value;
                _mouseRotation.NeedLimit = value;
            }
            get
            {
                return _mousePosition.NeedLimit;
            }
        }
        /// <summary>
        /// 自由控制：当前摄像机注视点
        /// </summary>
        public Vector3 LookPoint
        {
            get
            {
                return _cameraTarget.transform.position;
            }
        }
        /// <summary>
        /// 自由控制：当前摄像机注视视角
        /// </summary>
        public Vector3 LookAngle
        {
            get
            {
                return new Vector3(_mouseRotation.X, _mouseRotation.Y, _mouseRotation.Distance);
            }
        }
        /// <summary>
        /// 自由控制：是否启用摄像机移动控制
        /// </summary>
        public bool EnablePositionControl
        {
            get
            {
                return _mousePosition.CanControl;
            }
            set
            {
                _mousePosition.CanControl = value;
            }
        }
        /// <summary>
        /// 自由控制：是否启用摄像机旋转控制
        /// </summary>
        public bool EnableRotationControl
        {
            get
            {
                return _mouseRotation.CanControl;
            }
            set
            {
                _mouseRotation.CanControl = value;
            }
        }
        /// <summary>
        /// 自由控制：允许在输入滚轮超越距离限制时，启用摄像机移动
        /// </summary>
        public bool AllowOverstepDistance
        {
            get
            {
                return _mouseRotation.AllowOverstepDistance;
            }
            set
            {
                _mouseRotation.AllowOverstepDistance = value;
            }
        }
        /// <summary>
        /// 自由控制：摄像机是否始终保持注视目标
        /// </summary>
        public bool IsLookAtTarget
        {
            get
            {
                return _mouseRotation.IsLookAtTarget;
            }
            set
            {
                _mouseRotation.IsLookAtTarget = value;
            }
        }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        public MouseRayTargetBase RayTarget
        {
            get
            {
                return _mouseRay.Target;
            }
        }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        public GameObject RayTargetObj
        {
            get
            {
                if (_mouseRay.Target)
                    return _mouseRay.Target.gameObject;
                else
                    return null;
            }
        }
        /// <summary>
        /// 当前射线击中的点
        /// </summary>
        public Vector3 RayHitPoint
        {
            get
            {
                return _mouseRay.HitPoint;
            }
        }
        /// <summary>
        /// 是否启用高光特效
        /// </summary>
        public bool EnableHighlightingEffect
        {
            get
            {
                return _highlightingEffect.enabled;
            }
            set
            {
                _highlightingEffect.enabled = value;
            }
        }
        /// <summary>
        /// 是否启用鼠标射线
        /// </summary>
        public bool EnableMouseRay
        {
            get
            {
                return _mouseRay.IsOpenRay;
            }
            set
            {
                _mouseRay.IsOpenRay = value;
            }
        }
        /// <summary>
        /// 是否启用鼠标射线击中提示框
        /// </summary>
        public bool EnableMouseRayHitPrompt
        {
            get
            {
                return _mouseRay.IsOpenPrompt;
            }
            set
            {
                _mouseRay.IsOpenPrompt = value;
            }
        }

        /// <summary>
        /// 自由控制：设置控制外围限定最小值
        /// </summary>
        /// <param name="value">视野平移、旋转时，视角在x,y,z三个轴的最小值</param>
        public void SetMinLimit(Vector3 value)
        {
            _mousePosition.SetMinLimit(value);
            _mouseRotation.SetMinLimit(value);
        }
        /// <summary>
        /// 自由控制：设置控制外围限定最大值
        /// </summary>
        /// <param name="value">视野平移、旋转时，视角在x,y,z三个轴的最大值</param>
        public void SetMaxLimit(Vector3 value)
        {
            _mousePosition.SetMaxLimit(value);
            _mouseRotation.SetMaxLimit(value);
        }

        /// <summary>
        /// 自由控制：设置摄像机注视点
        /// </summary>
        /// <param name="point">目标位置</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookPoint(Vector3 point, bool damping = true)
        {
            _mousePosition.SetPosition(point, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookAngle(Vector3 angle, bool damping = true)
        {
            _mouseRotation.SetAngle(angle, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="distance">注视距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookAngle(Vector2 angle, float distance, bool damping = true)
        {
            _mouseRotation.SetAngle(angle, distance, damping);
        }

        /// <summary>
        /// 自由控制：进入保持追踪模式
        /// </summary>
        /// <param name="target">追踪目标</param>
        public void EnterKeepTrack(Transform target)
        {
            _mousePosition.EnterKeepTrack(target);
        }
        /// <summary>
        /// 自由控制：退出保持追踪模式
        /// </summary>
        public void LeaveKeepTrack()
        {
            _mousePosition.LeaveKeepTrack();
        }

        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public void AddClickListener(GameObject target, HTFAction callback)
        {
            MouseRayTargetBase mouseRayTargetBase = target.GetComponent<MouseRayTargetBase>();
            if (mouseRayTargetBase)
            {
                if (!_mouseClickTargets.ContainsKey(mouseRayTargetBase))
                {
                    _mouseClickTargets.Add(mouseRayTargetBase, callback);
                }
            }
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        public void RemoveClickListener(GameObject target)
        {
            MouseRayTargetBase mouseRayTargetBase = target.GetComponent<MouseRayTargetBase>();
            if (mouseRayTargetBase)
            {
                if (_mouseClickTargets.ContainsKey(mouseRayTargetBase))
                {
                    _mouseClickTargets.Remove(mouseRayTargetBase);
                }
            }
        }
        /// <summary>
        /// 清空所有点击事件
        /// </summary>
        public void ClearClickListener()
        {
            _mouseClickTargets.Clear();
        }

        /// <summary>
        /// 设置射线发射器的焦点提示框
        /// </summary>
        /// <param name="background">提示框背景</param>
        /// <param name="content">提示文字框</param>
        /// <param name="uIType">提示框UI类型</param>
        public void SetMouseRayFocusImage(Image background, Text content, UIType uIType = UIType.Overlay)
        {
            if (background == null || content == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Controller, "焦点提示框的背景和文字框均不能为空！");
            }

            content.transform.SetParent(background.transform);
            content.raycastTarget = false;
            background.raycastTarget = false;

            ContentSizeFitter contentSizeFitter = content.gameObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null) contentSizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            _mouseRay.RayHitBG = background;
            _mouseRay.RayHitText = content;
            _mouseRay.RayHitImageType = uIType;
        }
        
        private void OnRay(MouseRayTargetBase mouseRayTargetBase, Vector3 point, Vector2 pos)
        {
            RayEvent?.Invoke(mouseRayTargetBase, point, pos);
        }
    }

    /// <summary>
    /// 控制模式
    /// </summary>
    public enum ControlMode
    {
        /// <summary>
        /// 自由控制
        /// </summary>
        FreeControl,
        /// <summary>
        /// 第一人称控制
        /// </summary>
        FirstPerson,
        /// <summary>
        /// 第三人称控制
        /// </summary>
        ThirdPerson
    }
}