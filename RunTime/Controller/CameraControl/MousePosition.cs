using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标移动控制
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MousePosition : MonoBehaviour
    {
        //注视目标
        public CameraTarget Target;
        //旋转控制器
        public MouseRotation MR;
        //阻尼缓冲时长
        public float DampingTime = 1;
        //x轴移动速度，y轴移动速度，z轴移动速度
        public float XSpeed = 0.1f, YSpeed = 0.1f, ZSpeed = 0.1f;
        //是否限定平移位置
        public bool NeedLimit = true;
        //x轴平移最低值，x轴平移最高值
        public float XMinLimit = -5, XMaxLimit = 5;
        //y轴平移最低值，y轴平移最高值
        public float YMinLimit = 0.1f, YMaxLimit = 5;
        //z轴平移最低值，z轴平移最高值
        public float ZMinLimit = -5, ZMaxLimit = 5;
        //在UGUI目标上是否可以控制
        public bool IsCanOnUGUI = false;
        
        //最终的位置
        private Vector3 _finalPosition;
        //阻尼缓动模式时的动画
        private Tweener _moveTweener;
        //保持追踪模式
        private bool _isKeepTrack = false;
        //追踪目标
        private Transform _trackTarget;

        /// <summary>
        /// 是否可以控制
        /// </summary>
        internal bool CanControl { get; set; } = true;

        /// <summary>
        /// 设置平移限定最小值
        /// </summary>
        /// <param name="value">视野平移时，视角在x,y,z三个轴的最小值</param>
        internal void SetMinLimit(Vector3 value)
        {
            XMinLimit = value.x;
            YMinLimit = value.y;
            ZMinLimit = value.z;
        }

        /// <summary>
        /// 设置平移限定最大值
        /// </summary>
        /// <param name="value">视野平移时，视角在x,y,z三个轴的最大值</param>
        internal void SetMaxLimit(Vector3 value)
        {
            XMaxLimit = value.x;
            YMaxLimit = value.y;
            ZMaxLimit = value.z;
        }

        /// <summary>
        /// 平移注视视野
        /// </summary>
        /// <param name="position">目标位置</param>
        /// <param name="damping">阻尼缓动模式</param>
        internal void SetPosition(Vector3 position, bool damping = true)
        {
            if (_isKeepTrack)
            {
                return;
            }

            if (NeedLimit)
            {
                position = GlobalTools.Clamp(position, XMinLimit, YMinLimit, ZMinLimit, XMaxLimit, YMaxLimit, ZMaxLimit);
            }

            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }

            if (damping)
            {
                _moveTweener = Target.transform.DOMove(position, DampingTime);
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
        internal void EnterKeepTrack(Transform target)
        {
            if (!target)
            {
                throw new HTFrameworkException(HTFrameworkModule.Controller, "保持追踪的目标不能为空！");
            }

            _isKeepTrack = true;
            _trackTarget = target;
        }

        /// <summary>
        /// 退出保持追踪模式
        /// </summary>
        internal void LeaveKeepTrack()
        {
            _isKeepTrack = false;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        internal void OnRefresh()
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

            if (!IsCanOnUGUI && GlobalTools.IsPointerOverUGUI())
                return;

            if (Main.m_Input.GetButton(InputButtonType.MouseMiddle))
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis(InputAxisType.MouseX) * XSpeed * MR.Distance * -1);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis(InputAxisType.MouseY) * YSpeed * MR.Distance * -1);
                MR.NeedDamping = false;
            }
            else if (Main.m_Input.GetAxisRaw(InputAxisType.Horizontal) != 0 || Main.m_Input.GetAxisRaw(InputAxisType.Vertical) != 0 || Main.m_Input.GetAxisRaw(InputAxisType.UpperLower) != 0)
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis(InputAxisType.Horizontal) * XSpeed);
                Target.transform.Translate(transform.forward * Main.m_Input.GetAxis(InputAxisType.Vertical) * ZSpeed);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis(InputAxisType.UpperLower) * YSpeed);
                MR.NeedDamping = false;
            }
            else
            {
                MR.NeedDamping = true;
            }
        }

        private void ApplyPosition()
        {
            if (NeedLimit)
            {
                Target.transform.position = GlobalTools.Clamp(Target.transform.position, XMinLimit, YMinLimit, ZMinLimit, XMaxLimit, YMaxLimit, ZMaxLimit);
            }
        }
    }
}