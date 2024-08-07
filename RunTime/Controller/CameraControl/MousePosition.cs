using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标移动控制
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class MousePosition : HTBehaviour
    {
        /// <summary>
        /// 是否可以控制
        /// </summary>
        public bool CanControl = true;
        /// <summary>
        /// 在UGUI目标上是否可以控制
        /// </summary>
        public bool IsCanOnUGUI = false;
        /// <summary>
        /// 是否可以通过按键控制
        /// </summary>
        public bool IsCanByKey = true;
        /// <summary>
        /// x轴移动速度，y轴移动速度，z轴移动速度
        /// </summary>
        public float XSpeed = 0.1f, YSpeed = 0.1f, ZSpeed = 0.1f;
        
        /// <summary>
        /// 阻尼缓动模式时的动画缓存
        /// </summary>
        private Tweener _moveTweener;
        /// <summary>
        /// 保持追踪模式
        /// </summary>
        private bool _isKeepTrack = false;
        /// <summary>
        /// 追踪的目标
        /// </summary>
        private Transform _trackTarget;
        
        /// <summary>
        /// 注视目标
        /// </summary>
        public CameraTarget Target { get; set; }
        /// <summary>
        /// 操作控制器
        /// </summary>
        public ControllerManager Controller { get; set; }

        /// <summary>
        /// 平移注视视野
        /// </summary>
        /// <param name="position">目标位置</param>
        /// <param name="damping">阻尼缓动模式</param>
        /// <param name="dampingTime">阻尼缓动时长</param>
        public void SetPosition(Vector3 position, bool damping, float dampingTime)
        {
            if (_isKeepTrack)
                return;

            if (Controller.IsEnableBounds)
            {
                //应用边界盒
                position = ApplyBounds(position);
            }

            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }

            if (damping)
            {
                _moveTweener = Target.transform.DOMove(position, dampingTime);
            }
            else
            {
                Target.transform.position = position;
            }
        }
        /// <summary>
        /// 进入保持追踪模式
        /// </summary>
        /// <param name="target">追踪目标</param>
        public void EnterKeepTrack(Transform target)
        {
            if (target == null)
                return;

            _isKeepTrack = true;
            _trackTarget = target;
        }
        /// <summary>
        /// 退出保持追踪模式
        /// </summary>
        public void LeaveKeepTrack()
        {
            _isKeepTrack = false;
        }
        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
        {
            //控制
            Control();
            //应用
            ApplyPosition();
        }
        
        private void Control()
        {
            if (_isKeepTrack)
            {
                if (_trackTarget)
                {
                    Target.transform.position = _trackTarget.position;
                }
                else
                {
                    _isKeepTrack = false;
                }
                return;
            }

            if (!CanControl)
                return;

            if (!IsCanOnUGUI && UIToolkit.IsStayUINotWorld)
                return;

            if (Main.m_Input.GetButton(InputButtonType.MouseMiddle))
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis(InputAxisType.MouseX) * XSpeed * -1);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis(InputAxisType.MouseY) * YSpeed * -1);
            }
            else if (IsCanByKey && (Main.m_Input.GetAxisRaw(InputAxisType.Horizontal) != 0 || Main.m_Input.GetAxisRaw(InputAxisType.Vertical) != 0 || Main.m_Input.GetAxisRaw(InputAxisType.UpperLower) != 0))
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis(InputAxisType.Horizontal) * XSpeed);
                Target.transform.Translate(transform.forward * Main.m_Input.GetAxis(InputAxisType.Vertical) * ZSpeed);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis(InputAxisType.UpperLower) * YSpeed);
            }
        }
        private void ApplyPosition()
        {
            if (Controller.IsEnableBounds)
            {
                Target.transform.position = ApplyBounds(Target.transform.position);
            }
        }
        private Vector3 ApplyBounds(Vector3 position)
        {
            if (InTheBounds(position))
            {
                return position;
            }
            else
            {
                return ClosestPoint(position);
            }
        }
        private bool InTheBounds(Vector3 position)
        {
            if (Controller.FreeControlBounds.Count == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < Controller.FreeControlBounds.Count; i++)
                {
                    if (Controller.FreeControlBounds[i].Contains(position))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private Vector3 ClosestPoint(Vector3 position)
        {
            if (Controller.FreeControlBounds.Count == 1)
            {
                return Controller.FreeControlBounds[0].ClosestPoint(position);
            }
            else
            {
                Bounds bounds = Controller.FreeControlBounds[0];
                float dis = Vector3.Distance(bounds.center, position);
                for (int i = 1; i < Controller.FreeControlBounds.Count; i++)
                {
                    float newdis = Vector3.Distance(Controller.FreeControlBounds[i].center, position);
                    if (newdis < dis)
                    {
                        bounds = Controller.FreeControlBounds[i];
                        dis = newdis;
                    }
                }
                return bounds.ClosestPoint(position);
            }
        }
    }
}