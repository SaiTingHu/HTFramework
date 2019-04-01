using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标移动控制
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MousePosition : MonoBehaviour
    {
        //注视目标
        public CameraTarget Target;
        //旋转控制器
        public MouseRotation MR;
        //移动缓冲值
        public float MoveDamping = 1;
        //x轴移动速度，y轴移动速度，滚轮移动速度
        public float XSpeed = 0.1f, YSpeed = 0.1f, MSpeed = 1;
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
        //目标缓动平移时的动画
        private Tweener _moveTweener;

        /// <summary>
        /// 是否可以控制
        /// </summary>
        public bool CanControl { get; set; } = true;

        /// <summary>
        /// 设置平移限定最小值
        /// </summary>
        public void SetMinLimit(Vector3 value)
        {
            XMinLimit = value.x;
            YMinLimit = value.y;
            ZMinLimit = value.z;
        }

        /// <summary>
        /// 设置平移限定最大值
        /// </summary>
        public void SetMaxLimit(Vector3 value)
        {
            XMaxLimit = value.x;
            YMaxLimit = value.y;
            ZMaxLimit = value.z;
        }

        /// <summary>
        /// 平移注释目标，从而平移摄像机
        /// </summary>
        public void SetPosition(Vector3 position, bool damping)
        {
            if (NeedLimit)
            {
                position.x = Mathf.Clamp(position.x, XMinLimit, XMaxLimit);
                position.y = Mathf.Clamp(position.y, YMinLimit, YMaxLimit);
                position.z = Mathf.Clamp(position.z, ZMinLimit, ZMaxLimit);
            }

            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }

            if (damping)
            {
                _moveTweener = Target.transform.DOMove(position, MoveDamping);
            }
            else
            {
                Target.transform.position = position;
            }
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (!CanControl)
                return;

            if (!IsCanOnUGUI && GlobalTools.IsPointerOverUGUI())
                return;

            if (Main.m_Input.GetButton("MouseMiddle"))
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxis("MouseX") * XSpeed * MR.Distance * -1);
                Target.transform.Translate(transform.up * Main.m_Input.GetAxis("MouseY") * YSpeed * MR.Distance * -1);
                MR.NeedDamping = false;
            }
            else if (Main.m_Input.GetAxisRaw("Horizontal") != 0 || Main.m_Input.GetAxisRaw("Vertical") != 0)
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                Target.transform.Translate(transform.right * Main.m_Input.GetAxisRaw("Horizontal") * XSpeed);
                Target.transform.Translate(transform.forward * Main.m_Input.GetAxisRaw("Vertical") * YSpeed);
                MR.NeedDamping = false;
            }
            else
            {
                MR.NeedDamping = true;
            }

            if (NeedLimit)
            {
                if (Target.transform.position.x < XMinLimit)
                {
                    _finalPosition.Set(XMinLimit, Target.transform.position.y, Target.transform.position.z);
                    Target.transform.position = _finalPosition;
                }
                else if (Target.transform.position.x > XMaxLimit)
                {
                    _finalPosition.Set(XMaxLimit, Target.transform.position.y, Target.transform.position.z);
                    Target.transform.position = _finalPosition;
                }

                if (Target.transform.position.y < YMinLimit)
                {
                    _finalPosition.Set(Target.transform.position.x, YMinLimit, Target.transform.position.z);
                    Target.transform.position = _finalPosition;
                }
                else if (Target.transform.position.y > YMaxLimit)
                {
                    _finalPosition.Set(Target.transform.position.x, YMaxLimit, Target.transform.position.z);
                    Target.transform.position = _finalPosition;
                }

                if (Target.transform.position.z < ZMinLimit)
                {
                    _finalPosition.Set(Target.transform.position.x, Target.transform.position.y, ZMinLimit);
                    Target.transform.position = _finalPosition;
                }
                else if (Target.transform.position.z > ZMaxLimit)
                {
                    _finalPosition.Set(Target.transform.position.x, Target.transform.position.y, ZMaxLimit);
                    Target.transform.position = _finalPosition;
                }
            }
        }
    }
}