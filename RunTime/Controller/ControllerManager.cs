using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 操作控制者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ControllerManager : ModuleManager
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
        /// 自由控制刷新事件
        /// </summary>
        public event HTFAction FreeControlUpdateEvent;
        /// <summary>
        /// 第一人称刷新事件
        /// </summary>
        public event HTFAction FirstPersonUpdateEvent;
        /// <summary>
        /// 第三人称刷新事件
        /// </summary>
        public event HTFAction ThirdPersonUpdateEvent;
        /// <summary>
        /// 射线投射事件(MouseRayTargetBase：当前射中的目标，Vector3：当前射中的点，Vector2：当前鼠标位置转换后的UGUI坐标)
        /// </summary>
        public event HTFAction<MouseRayTargetBase, Vector3, Vector2> RayEvent;

        private CameraTarget _cameraTarget;
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private MouseRay _mouseRay;
        private HighlightingEffect _highlightingEffect;

        private ControlMode _controlMode;

        public override void OnInitialization()
        {
            base.OnInitialization();

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

            TheControlMode = ControlMode.FreeControl;
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            _mouseRay.OnRefresh();
            switch (TheControlMode)
            {
                case ControlMode.FreeControl:
                    FreeControlUpdateEvent?.Invoke();

                    _mousePosition.OnRefresh();
                    _mouseRotation.OnRefresh();
                    break;
                case ControlMode.FirstPerson:
                    FirstPersonUpdateEvent?.Invoke();
                    break;
                case ControlMode.ThirdPerson:
                    ThirdPersonUpdateEvent?.Invoke();
                    break;
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
        /// 退出保持追踪模式
        /// </summary>
        public void LeaveKeepTrack()
        {
            _mousePosition.LeaveKeepTrack();
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
                GlobalTools.LogError("焦点提示框的背景和文字框均不能为空！");
                return;
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