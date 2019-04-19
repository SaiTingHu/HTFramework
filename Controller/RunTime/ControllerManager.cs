using System;
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
        public event Action SwitchToFreeControlEvent;
        /// <summary>
        /// 切换至第一人称事件
        /// </summary>
        public event Action SwitchToFirstPersonEvent;
        /// <summary>
        /// 切换至第三人称事件
        /// </summary>
        public event Action SwitchToThirdPersonEvent;
        /// <summary>
        /// 自由控制刷新事件
        /// </summary>
        public event Action FreeControlUpdateEvent;
        /// <summary>
        /// 第一人称刷新事件
        /// </summary>
        public event Action FirstPersonUpdateEvent;
        /// <summary>
        /// 第三人称刷新事件
        /// </summary>
        public event Action ThirdPersonUpdateEvent;
        
        private CameraTarget _cameraTarget;
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private MouseRay _mouseRay;
        private HighlightingEffect _highlightingEffect;

        private ControlMode _controlMode;

        public override void Initialization()
        {
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
        }

        public override void Preparatory()
        {
            TheControlMode = ControlMode.FreeControl;
        }

        public override void Refresh()
        {
            _mouseRay.Refresh();
            switch (TheControlMode)
            {
                case ControlMode.FreeControl:
                    if (FreeControlUpdateEvent != null)
                        FreeControlUpdateEvent();

                    _mousePosition.Refresh();
                    _mouseRotation.Refresh();
                    break;
                case ControlMode.FirstPerson:
                    if (FirstPersonUpdateEvent != null)
                        FirstPersonUpdateEvent();
                    break;
                case ControlMode.ThirdPerson:
                    if (ThirdPersonUpdateEvent != null)
                        ThirdPersonUpdateEvent();
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
                            if (SwitchToFreeControlEvent != null)
                                SwitchToFreeControlEvent();
                            break;
                        case ControlMode.FirstPerson:
                            if (SwitchToFirstPersonEvent != null)
                                SwitchToFirstPersonEvent();
                            break;
                        case ControlMode.ThirdPerson:
                            if (SwitchToThirdPersonEvent != null)
                                SwitchToThirdPersonEvent();
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
                return _mousePosition.Target.transform.position;
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
        public MouseRayTarget RayTarget
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
        public void SetMinLimit(Vector3 value)
        {
            _mousePosition.SetMinLimit(value);
            _mouseRotation.SetMinLimit(value);
        }
        /// <summary>
        /// 自由控制：设置控制外围限定最大值
        /// </summary>
        public void SetMaxLimit(Vector3 value)
        {
            _mousePosition.SetMaxLimit(value);
            _mouseRotation.SetMaxLimit(value);
        }

        /// <summary>
        /// 自由控制：设置摄像机注视点
        /// </summary>
        public void SetLookPoint(Vector3 point, bool damping)
        {
            _mousePosition.SetPosition(point, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        public void SetLookAngle(Vector3 angle, bool damping)
        {
            _mouseRotation.SetAngle(angle, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        public void SetLookAngle(Vector2 angle, float distance, bool damping)
        {
            _mouseRotation.SetAngle(angle, distance, damping);
        }

        /// <summary>
        /// 设置射线发射器的焦点提示框
        /// </summary>
        public void SetMouseRayFocusImage(Image background, Text content)
        {
            _mouseRay.RayHitImage = background;
            _mouseRay.RayHitText = content;
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