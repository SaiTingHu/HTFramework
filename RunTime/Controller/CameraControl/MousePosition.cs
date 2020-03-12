using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标移动控制
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class MousePosition : MonoBehaviour
    {
        /// <summary>
        /// 阻尼缓冲时长
        /// </summary>
        public float DampingTime = 1;
        /// <summary>
        /// x轴移动速度，y轴移动速度，z轴移动速度
        /// </summary>
        public float XSpeed = 0.1f, YSpeed = 0.1f, ZSpeed = 0.1f;
        /// <summary>
        /// 是否限定平移位置
        /// </summary>
        public bool NeedLimit = true;
        /// <summary>
        /// x轴平移最低值，x轴平移最高值
        /// </summary>
        public float XMinLimit = -5, XMaxLimit = 5;
        /// <summary>
        /// y轴平移最低值，y轴平移最高值
        /// </summary>
        public float YMinLimit = 0.1f, YMaxLimit = 5;
        /// <summary>
        /// z轴平移最低值，z轴平移最高值
        /// </summary>
        public float ZMinLimit = -5, ZMaxLimit = 5;
        /// <summary>
        /// 在UGUI目标上是否可以控制
        /// </summary>
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
        public bool CanControl { get; set; } = true;

        /// <summary>
        /// 注视目标
        /// </summary>
        public CameraTarget Target { get; set; }

        /// <summary>
        /// 旋转控制器
        /// </summary>
        public MouseRotation MR { get; set; }

        /// <summary>
        /// 设置平移限定最小值
        /// </summary>
        /// <param name="value">视野平移时，视角在x,y,z三个轴的最小值</param>
        public void SetMinLimit(Vector3 value)
        {
            XMinLimit = value.x;
            YMinLimit = value.y;
            ZMinLimit = value.z;
        }

        /// <summary>
        /// 设置平移限定最大值
        /// </summary>
        /// <param name="value">视野平移时，视角在x,y,z三个轴的最大值</param>
        public void SetMaxLimit(Vector3 value)
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
        public void SetPosition(Vector3 position, bool damping = true)
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
        public void EnterKeepTrack(Transform target)
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
        public void LeaveKeepTrack()
        {
            _isKeepTrack = false;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void OnRefresh()
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
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis(InputAxisType.MouseX) * XSpeed * -1);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis(InputAxisType.MouseY) * YSpeed * -1);
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