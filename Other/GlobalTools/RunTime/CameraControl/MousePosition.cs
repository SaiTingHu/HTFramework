using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标移动控制
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class MousePosition : MonoBehaviour
    {
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

        //注视目标
        private CameraTarget _target;
        //是否可以控制
        private bool _isCanControl = true;
        //最终的位置
        private Vector3 _finalPosition;
        //目标缓动平移时的动画
        private Tweener _moveTweener;

        private static MousePosition _instance;
        public static MousePosition Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _target = CameraTarget.Instance;
        }

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
            if (!_target)
                _target = CameraTarget.Instance;

            position.x = Mathf.Clamp(position.x, XMinLimit, XMaxLimit);
            position.y = Mathf.Clamp(position.y, YMinLimit, YMaxLimit);
            position.z = Mathf.Clamp(position.z, ZMinLimit, ZMaxLimit);

            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }

            if (damping)
            {
                _moveTweener = _target.transform.DOMove(position, MoveDamping);
            }
            else
            {
                _target.transform.position = position;
            }
        }

        /// <summary>
        /// 是否可以控制
        /// </summary>
        public bool CanControl
        {
            get
            {
                return _isCanControl;
            }
            set
            {
                _isCanControl = value;
            }
        }

        private void Update()
        {
            if (!_target)
                return;

            if (!_isCanControl)
                return;

            if (!IsCanOnUGUI && GlobalTools.IsPointerOverUGUI())
                return;

            if (Input.GetMouseButton(2))
            {
                if (_moveTweener != null)
                {
                    _moveTweener.Kill();
                    _moveTweener = null;
                }
                _target.transform.Translate(transform.right * Input.GetAxis("Mouse X") * XSpeed * MouseRotation.Instance.Distance * -1);
                _target.transform.Translate(transform.up * Input.GetAxis("Mouse Y") * YSpeed * MouseRotation.Instance.Distance * -1);
                MouseRotation.Instance.NeedDamping = false;
            }
            else
            {
                MouseRotation.Instance.NeedDamping = true;
            }

            if (NeedLimit)
            {
                if (_target.transform.position.x < XMinLimit)
                {
                    _finalPosition.Set(XMinLimit, _target.transform.position.y, _target.transform.position.z);
                    _target.transform.position = _finalPosition;
                }
                else if (_target.transform.position.x > XMaxLimit)
                {
                    _finalPosition.Set(XMaxLimit, _target.transform.position.y, _target.transform.position.z);
                    _target.transform.position = _finalPosition;
                }

                if (_target.transform.position.y < YMinLimit)
                {
                    _finalPosition.Set(_target.transform.position.x, YMinLimit, _target.transform.position.z);
                    _target.transform.position = _finalPosition;
                }
                else if (_target.transform.position.y > YMaxLimit)
                {
                    _finalPosition.Set(_target.transform.position.x, YMaxLimit, _target.transform.position.z);
                    _target.transform.position = _finalPosition;
                }

                if (_target.transform.position.z < ZMinLimit)
                {
                    _finalPosition.Set(_target.transform.position.x, _target.transform.position.y, ZMinLimit);
                    _target.transform.position = _finalPosition;
                }
                else if (_target.transform.position.z > ZMaxLimit)
                {
                    _finalPosition.Set(_target.transform.position.x, _target.transform.position.y, ZMaxLimit);
                    _target.transform.position = _finalPosition;
                }
            }
        }
    }
}