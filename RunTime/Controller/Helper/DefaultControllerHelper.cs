using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 默认的操作控制器助手
    /// </summary>
    public sealed class DefaultControllerHelper : IControllerHelper
    {
        private ControllerManager _module;
        private CameraTarget _cameraTarget;
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private MouseRay _mouseRay;
        private HighlightingEffect _highlightingEffect;

        /// <summary>
        /// 操作控制器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 控制模式
        /// </summary>
        public ControlMode Mode { get; set; }
        /// <summary>
        /// 主摄像机
        /// </summary>
        public Camera MainCamera { get; private set; }
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
        /// 自由控制：在UGUI目标上是否可以控制
        /// </summary>
        public bool IsCanControlOnUGUI
        {
            get
            {
                return _mousePosition.IsCanOnUGUI || _mouseRotation.IsCanOnUGUI;
            }
            set
            {
                _mousePosition.IsCanOnUGUI = _mouseRotation.IsCanOnUGUI = value;
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
                if (!value)
                {
                    HighlightableToolkit.CloseAllFlashHighLight(HighlightAutoDie);
                    HighlightableToolkit.CloseAllHighLight(HighlightAutoDie);
                    HighlightableToolkit.CloseAllOccluder(HighlightAutoDie);
                    MeshOutlineToolkit.CloseAllMeshOutline(HighlightAutoDie);
                }
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
                if (!_mouseRay.IsOpenRay)
                {
                    _mouseRay.CloseRay();
                }
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
        /// 高亮组件是否自动销毁
        /// </summary>
        public bool HighlightAutoDie
        {
            get
            {
                return _mouseRay.IsAutoDie;
            }
            set
            {
                _mouseRay.IsAutoDie = value;
            }
        }
        /// <summary>
        /// 射线投射事件(MouseRayTargetBase：当前射中的目标，Vector3：当前射中的点，Vector2：当前鼠标位置转换后的UGUI坐标)
        /// </summary>
        public event HTFAction<MouseRayTargetBase, Vector3, Vector2> RayEvent;

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            _module = Module as ControllerManager;
            MainCamera = _module.GetComponentByChild<Camera>("MainCamera");
            _cameraTarget = _module.GetComponentByChild<CameraTarget>("CameraTarget");
            _mousePosition = MainCamera.GetComponent<MousePosition>();
            _mouseRotation = MainCamera.GetComponent<MouseRotation>();
            _mouseRay = MainCamera.GetComponent<MouseRay>();
            _highlightingEffect = MainCamera.GetComponent<HighlightingEffect>();

            _mousePosition.Target = _cameraTarget;
            _mousePosition.MR = _mouseRotation;
            _mousePosition.Manager = Module as ControllerManager;
            _mouseRotation.Target = _cameraTarget;
            _mouseRotation.Manager = Module as ControllerManager;
            _mouseRay.RayCamera = MainCamera;
            _mouseRay.RayEvent += (target, point, point2D) =>
            {
                RayEvent?.Invoke(target, point, point2D);
            };
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {
            _mouseRay.OnRefresh();
            if (Mode == ControlMode.FreeControl)
            {
                _mousePosition.OnRefresh();
                _mouseRotation.OnRefresh();
            }

            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
            {
                if (RayTarget != null)
                {
                    RayTarget.OnMouseClick.Invoke();
                }
            }
            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick))
            {
                if (RayTarget != null)
                {
                    MouseRayTarget target = RayTarget as MouseRayTarget;
                    if (target && target.IsDoubleClickFocus)
                    {
                        SetLookPoint(target.transform.position);
                    }
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            
        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {
            
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
        /// <param name="x">视角x值</param>
        /// <param name="y">视角y值</param>
        /// <param name="distance">视角距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetLookAngle(float x, float y, float distance, bool damping = true)
        {
            _mouseRotation.SetAngle(x, y, distance, damping);
        }
        /// <summary>
        /// 自由控制：设置视角移动速度
        /// </summary>
        /// <param name="x">x轴移动速度</param>
        /// <param name="y">y轴移动速度</param>
        /// <param name="z">z轴移动速度</param>
        public void SetMoveSpeed(float x, float y, float z)
        {
            _mousePosition.XSpeed = x;
            _mousePosition.YSpeed = y;
            _mousePosition.ZSpeed = z;
        }
        /// <summary>
        /// 自由控制：设置视角旋转速度
        /// </summary>
        /// <param name="x">x轴旋转速度</param>
        /// <param name="y">y轴旋转速度</param>
        /// <param name="m">滚轮缩放速度</param>
        public void SetRotateSpeed(float x, float y, float m)
        {
            _mouseRotation.XSpeed = x;
            _mouseRotation.YSpeed = y;
            _mouseRotation.MSpeed = m;
        }
        /// <summary>
        /// 自由控制：设置摄像机旋转时视角Y轴的限制
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public void SetAngleLimit(float min, float max)
        {
            _mouseRotation.YMinAngleLimit = min;
            _mouseRotation.YMaxAngleLimit = max;
        }
        /// <summary>
        /// 自由控制：设置摄像机注视距离的最小值和最大值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public void SetMinMaxDistance(float min, float max)
        {
            _mouseRotation.MinDistance = min;
            _mouseRotation.MaxDistance = max;
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

        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public void AddClickListener(GameObject target, UnityAction callback)
        {
            if (target == null || callback == null)
                return;

            MouseRayTargetBase mouseRayTargetBase = target.GetComponent<MouseRayTargetBase>();
            if (mouseRayTargetBase)
            {
                mouseRayTargetBase.OnMouseClick.AddListener(callback);
            }
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public void RemoveClickListener(GameObject target, UnityAction callback)
        {
            if (target == null || callback == null)
                return;

            MouseRayTargetBase mouseRayTargetBase = target.GetComponent<MouseRayTargetBase>();
            if (mouseRayTargetBase)
            {
                mouseRayTargetBase.OnMouseClick.RemoveListener(callback);
            }
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除所有的鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        public void RemoveAllClickListener(GameObject target)
        {
            if (target == null)
                return;

            MouseRayTargetBase mouseRayTargetBase = target.GetComponent<MouseRayTargetBase>();
            if (mouseRayTargetBase)
            {
                mouseRayTargetBase.OnMouseClick.RemoveAllListeners();
            }
        }
    }
}