using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 操作控制器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Controller)]
    public sealed class ControllerManager : InternalModuleBase
    {
        /// <summary>
        /// 默认的控制模式【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal ControlMode DefaultControlMode = ControlMode.FreeControl;
        /// <summary>
        /// Dotween动画的默认缓动类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal Ease DefaultEase = Ease.Linear;
        /// <summary>
        /// Dotween动画的默认自动启动方式【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal AutoPlay DefaultAutoPlay = AutoPlay.All;
        /// <summary>
        /// Dotween动画是否自动销毁【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsAutoKill = true;
        /// <summary>
        /// 自由控制：边界盒【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<Bounds> FreeControlBounds = new List<Bounds>();
        /// <summary>
        /// 自由控制：是否启用边界盒
        /// </summary>
        public bool IsEnableBounds = false;

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
        
        private IControllerHelper _helper;

        private ControllerManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();
            
            DOTween.defaultEaseType = DefaultEase;
            DOTween.defaultAutoPlay = DefaultAutoPlay;
            DOTween.defaultAutoKill = IsAutoKill;
            
            _helper = Helper as IControllerHelper;
            _helper.RayEvent += (target, point, point2D) =>
            {
                RayEvent?.Invoke(target, point, point2D);
            };
        }
        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            TheControlMode = DefaultControlMode;
        }

        /// <summary>
        /// 主摄像机
        /// </summary>
        public Camera MainCamera
        {
            get
            {
                return _helper.MainCamera;
            }
        }
        /// <summary>
        /// 控制模式
        /// </summary>
        public ControlMode TheControlMode
        {
            set
            {
                if (_helper.TheControlMode != value)
                {
                    _helper.TheControlMode = value;
                    switch (_helper.TheControlMode)
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
                return _helper.TheControlMode;
            }
        }
        /// <summary>
        /// 自由控制：当前摄像机注视点
        /// </summary>
        public Vector3 LookPoint
        {
            get
            {
                return _helper.LookPoint;
            }
        }
        /// <summary>
        /// 自由控制：当前摄像机注视视角
        /// </summary>
        public Vector3 LookAngle
        {
            get
            {
                return _helper.LookAngle;
            }
        }
        /// <summary>
        /// 自由控制：是否启用摄像机移动控制
        /// </summary>
        public bool EnablePositionControl
        {
            get
            {
                return _helper.EnablePositionControl;
            }
            set
            {
                _helper.EnablePositionControl = value;
            }
        }
        /// <summary>
        /// 自由控制：是否启用摄像机旋转控制
        /// </summary>
        public bool EnableRotationControl
        {
            get
            {
                return _helper.EnableRotationControl;
            }
            set
            {
                _helper.EnableRotationControl = value;
            }
        }
        /// <summary>
        /// 自由控制：在UGUI目标上是否可以控制
        /// </summary>
        public bool IsCanControlOnUGUI
        {
            get
            {
                return _helper.IsCanControlOnUGUI;
            }
            set
            {
                _helper.IsCanControlOnUGUI = value;
            }
        }
        /// <summary>
        /// 自由控制：允许在输入滚轮超越距离限制时，启用摄像机移动
        /// </summary>
        public bool AllowOverstepDistance
        {
            get
            {
                return _helper.AllowOverstepDistance;
            }
            set
            {
                _helper.AllowOverstepDistance = value;
            }
        }
        /// <summary>
        /// 自由控制：摄像机是否始终保持注视目标
        /// </summary>
        public bool IsLookAtTarget
        {
            get
            {
                return _helper.IsLookAtTarget;
            }
            set
            {
                _helper.IsLookAtTarget = value;
            }
        }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        public MouseRayTargetBase RayTarget
        {
            get
            {
                return _helper.RayTarget;
            }
        }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        public GameObject RayTargetObj
        {
            get
            {
                return _helper.RayTargetObj;
            }
        }
        /// <summary>
        /// 当前射线击中的点
        /// </summary>
        public Vector3 RayHitPoint
        {
            get
            {
                return _helper.RayHitPoint;
            }
        }
        /// <summary>
        /// 是否启用高光特效
        /// </summary>
        public bool EnableHighlightingEffect
        {
            get
            {
                return _helper.EnableHighlightingEffect;
            }
            set
            {
                _helper.EnableHighlightingEffect = value;
            }
        }
        /// <summary>
        /// 是否启用鼠标射线
        /// </summary>
        public bool EnableMouseRay
        {
            get
            {
                return _helper.EnableMouseRay;
            }
            set
            {
                _helper.EnableMouseRay = value;
            }
        }
        /// <summary>
        /// 是否启用鼠标射线击中提示框
        /// </summary>
        public bool EnableMouseRayHitPrompt
        {
            get
            {
                return _helper.EnableMouseRayHitPrompt;
            }
            set
            {
                _helper.EnableMouseRayHitPrompt = value;
            }
        }

        /// <summary>
        /// 自由控制：添加边界盒
        /// </summary>
        /// <param name="bounds">边界盒</param>
        public void AddBounds(Bounds bounds)
        {
            FreeControlBounds.Add(bounds);
        }
        /// <summary>
        /// 自由控制：添加边界盒
        /// </summary>
        /// <param name="min">边界盒最小值</param>
        /// <param name="max">边界盒最大值</param>
        public void AddBounds(Vector3 min, Vector3 max)
        {
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            FreeControlBounds.Add(bounds);
        }
        /// <summary>
        /// 自由控制：清空边界盒
        /// </summary>
        public void ClearBounds()
        {
            FreeControlBounds.Clear();
        }
        /// <summary>
        /// 自由控制：设置摄像机注视点
        /// </summary>
        /// <param name="point">目标位置</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookPoint(Vector3 point, bool damping = true)
        {
            _helper.SetLookPoint(point, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookAngle(Vector3 angle, bool damping = true)
        {
            _helper.SetLookAngle(angle.x, angle.y, angle.z, damping);
        }
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="distance">注视距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookAngle(Vector2 angle, float distance, bool damping = true)
        {
            _helper.SetLookAngle(angle.x, angle.y, distance, damping);
        }
        /// <summary>
        /// 自由控制：设置视角移动速度
        /// </summary>
        /// <param name="x">x轴移动速度</param>
        /// <param name="y">y轴移动速度</param>
        /// <param name="z">z轴移动速度</param>
        public void SetMoveSpeed(float x, float y, float z)
        {
            _helper.SetMoveSpeed(x, y, z);
        }
        /// <summary>
        /// 自由控制：设置视角移动速度
        /// </summary>
        /// <param name="speed">移动速度</param>
        public void SetMoveSpeed(Vector3 speed)
        {
            _helper.SetMoveSpeed(speed.x, speed.y, speed.z);
        }
        /// <summary>
        /// 自由控制：设置视角旋转速度
        /// </summary>
        /// <param name="x">x轴旋转速度</param>
        /// <param name="y">y轴旋转速度</param>
        /// <param name="m">滚轮缩放速度</param>
        public void SetRotateSpeed(float x, float y, float m)
        {
            _helper.SetRotateSpeed(x, y, m);
        }
        /// <summary>
        /// 自由控制：设置视角旋转速度
        /// </summary>
        /// <param name="speed">旋转速度</param>
        public void SetRotateSpeed(Vector3 speed)
        {
            _helper.SetRotateSpeed(speed.x, speed.y, speed.z);
        }
        /// <summary>
        /// 自由控制：进入保持追踪模式
        /// </summary>
        /// <param name="target">追踪目标</param>
        public void EnterKeepTrack(Transform target)
        {
            _helper.EnterKeepTrack(target);
        }
        /// <summary>
        /// 自由控制：退出保持追踪模式
        /// </summary>
        public void LeaveKeepTrack()
        {
            _helper.LeaveKeepTrack();
        }
        /// <summary>
        /// 设置射线发射器的焦点提示框
        /// </summary>
        /// <param name="background">提示框背景</param>
        /// <param name="content">提示文字框</param>
        /// <param name="uIType">提示框UI类型</param>
        public void SetMouseRayFocusImage(Image background, Text content, UIType uIType = UIType.Overlay)
        {
            _helper.SetMouseRayFocusImage(background, content, uIType);
        }

        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public void AddClickListener(GameObject target, HTFAction callback)
        {
            _helper.AddClickListener(target, callback);
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        public void RemoveClickListener(GameObject target)
        {
            _helper.RemoveClickListener(target);
        }
        /// <summary>
        /// 清空所有点击事件
        /// </summary>
        public void ClearClickListener()
        {
            _helper.ClearClickListener();
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