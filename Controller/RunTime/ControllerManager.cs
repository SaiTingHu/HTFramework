using System;
using UnityEngine;

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

        private ControlMode _mode;

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

            Mode = ControlMode.FreeControl;
        }

        public override void Refresh()
        {
            _mouseRay.Refresh();
            switch (Mode)
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
        public ControlMode Mode
        {
            set
            {
                _mode = value;
                switch (_mode)
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
            get
            {
                return _mode;
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
        /// 自由控制：设置摄像机平移点
        /// </summary>
        public void SetPosition(Vector3 position, bool damping)
        {
            _mousePosition.SetPosition(position, damping);
        }

        /// <summary>
        /// 自由控制：设置摄像机旋转角度
        /// </summary>
        public void SetAngle(Vector3 angle, bool damping)
        {
            _mouseRotation.SetAngle(angle, damping);
        }

        /// <summary>
        /// 自由控制：设置摄像机旋转角度
        /// </summary>
        public void SetAngle(Vector2 angle, float distance, bool damping)
        {
            _mouseRotation.SetAngle(angle, distance, damping);
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