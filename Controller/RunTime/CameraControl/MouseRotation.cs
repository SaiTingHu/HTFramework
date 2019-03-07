using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标旋转控制
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public sealed class MouseRotation : MonoBehaviour
    {
        //注视目标
        public CameraTarget Target;
        //注视点x、y轴偏移，若都为0，则注视点等于注视目标的transform.position，类似目标是角色时，注视点可能会上下偏移
        public float OffsetY = 0f;
        public float OffsetX = 0f;
        //x轴旋转速度，y轴旋转速度，滚轮缩放速度
        public float XSpeed = 150, YSpeed = 150, MSpeed = 30;
        //y轴视角最低值，y轴视角最高值（目标是角色的话，推荐最小10，最大85，摄像机最低不会到角色脚底，最高不会到角色头顶正上方）
        //例如视角最低值为10，那么摄像机在y轴旋转最小只能到10，不会小于0乃至超过0，如果目标是角色的话，也就是摄像机只能接近地面然后就不能往下继续移动了
        //例如视角最高值为85，那么摄像机在y轴旋转最大只能到85，不会大于85乃至更多，如果目标是角色的话，也就是摄像机只能接近角色头顶，不能到正90度完全垂直向下看向角色
        public float YMinAngleLimit = -85, YMaxAngleLimit = 85;
        //摄像机与注释目标距离
        public float Distance = 2.0f;
        //摄像机与注释目标最小距离
        public float MinDistance = 0;
        //摄像机与注释目标最大距离
        public float MaxDistance = 4;
        //摄像机的围绕目标旋转动画是否是插值动画
        public bool NeedDamping = true;
        //初始的摄像机x旋转，y旋转，摄像机在围绕目标旋转是这两个值会不停改变，设置初始值用于使摄像机初始看向角色的指定方向，例如初始看向角色背后
        public float X = 90.0f;
        public float Y = 30.0f;
        //摄像机是否限定位置
        public bool NeedLimit = false;
        //x轴位置最低值，x轴位置最高值
        public float XMinLimit = -5, XMaxLimit = 5;
        //y轴位置最低值，y轴位置最高值
        public float YMinLimit = 0.1f, YMaxLimit = 5;
        //z轴位置最低值，z轴位置最高值
        public float ZMinLimit = -5, ZMaxLimit = 5;
        //在UGUI目标上是否可以控制
        public bool IsCanOnUGUI = false;
        
        //注视点（注视目标的准确位置，经过偏移后的位置）
        private Vector3 _targetPoint;
        //插值量
        private float _damping = 5.0f;
        //系数
        private float _factor = 0.02f;

        //目标位置
        private Quaternion _rotation;
        private Vector3 _position;
        private Vector3 _disVector;
        //最终的位置
        private Vector3 _finalPosition;

        /// <summary>
        /// 是否可以控制
        /// </summary>
        public bool CanControl { get; set; } = true;

        /// <summary>
        /// 设置旋转限定最小值
        /// </summary>
        public void SetMinLimit(Vector3 value)
        {
            XMinLimit = value.x;
            YMinLimit = value.y;
            ZMinLimit = value.z;
        }

        /// <summary>
        /// 设置旋转限定最大值
        /// </summary>
        public void SetMaxLimit(Vector3 value)
        {
            XMaxLimit = value.x;
            YMaxLimit = value.y;
            ZMaxLimit = value.z;
        }

        /// <summary>
        /// 设置注视视角
        /// </summary>
        public void SetAngle(Vector3 angle, bool damping)
        {
            X = angle.x;
            Y = angle.y;
            Distance = angle.z;

            if (!damping)
            {
                CalculateAngle();
                SwitchAngle(damping);
            }
        }

        /// <summary>
        /// 设置注视视角
        /// </summary>
        public void SetAngle(Vector2 angle, float distance, bool damping)
        {
            X = angle.x;
            Y = angle.y;
            Distance = distance;

            if (!damping)
            {
                CalculateAngle();
                SwitchAngle(damping);
            }
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            //控制
            Control();
            //应用
            ApplyRotation();
        }

        private void Control()
        {
            if (!CanControl)
                return;

            if (!IsCanOnUGUI && GlobalTools.IsPointerOverUGUI())
                return;

            if (Input.GetMouseButton(1))
            {
                //记录鼠标横向移动量
                X += Input.GetAxis("Mouse X") * XSpeed * _factor;
                //记录鼠标纵向移动量
                Y -= Input.GetAxis("Mouse Y") * YSpeed * _factor;
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                //鼠标滚轮缩放视角距离
                Distance -= Input.GetAxis("Mouse ScrollWheel") * MSpeed * Time.deltaTime;

                if (Distance <= MinDistance)
                {
                    Target.transform.Translate(transform.forward * Input.GetAxis("Mouse ScrollWheel"));
                }
            }
        }

        private void ApplyRotation()
        {
            //重新计算视角
            CalculateAngle();

            //切换视角
            SwitchAngle(NeedDamping);

            //摄像机一直保持注视目标点
            transform.LookAt(_targetPoint);
        }

        private void CalculateAngle()
        {
            //将纵向移动量限制在视角最低值和最高值之间
            Y = ClampYAngle(Y, YMinAngleLimit, YMaxAngleLimit);
            //将视角距离限制在最小值和最大值之间
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

            //重新获取摄像机注视点
            _targetPoint.Set(Target.transform.position.x + OffsetX, Target.transform.position.y + OffsetY, Target.transform.position.z);

            //摄像机的最新旋转角度
            _rotation = Quaternion.Euler(Y, X, 0.0f);
            //摄像机与注视点的距离向量
            _disVector.Set(0.0f, 0.0f, -Distance);
            //摄像机的最新旋转角度*摄像机与注视点的距离，得出摄像机与注视点的相对位置，再由注视点的位置加上相对位置便等于摄像机的位置
            _position = Target.transform.position + _rotation * _disVector;
        }

        private void SwitchAngle(bool damping)
        {
            //摄像机插值变换到新的位置
            if (damping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime * _damping);
                transform.position = Vector3.Lerp(transform.position, _position, Time.deltaTime * _damping);
            }
            //摄像机直接变换到新的位置
            else
            {
                transform.rotation = _rotation;
                transform.position = _position;
            }

            //摄像机位置限制
            if (NeedLimit)
            {
                if (transform.position.x < XMinLimit)
                {
                    _finalPosition.Set(XMinLimit, transform.position.y, transform.position.z);
                    transform.position = _finalPosition;
                }
                else if (transform.position.x > XMaxLimit)
                {
                    _finalPosition.Set(XMaxLimit, transform.position.y, transform.position.z);
                    transform.position = _finalPosition;
                }

                if (transform.position.y < YMinLimit)
                {
                    _finalPosition.Set(transform.position.x, YMinLimit, transform.position.z);
                    transform.position = _finalPosition;
                }
                else if (transform.position.y > YMaxLimit)
                {
                    _finalPosition.Set(transform.position.x, YMaxLimit, transform.position.z);
                    transform.position = _finalPosition;
                }

                if (transform.position.z < ZMinLimit)
                {
                    _finalPosition.Set(transform.position.x, transform.position.y, ZMinLimit);
                    transform.position = _finalPosition;
                }
                else if (transform.position.z > ZMaxLimit)
                {
                    _finalPosition.Set(transform.position.x, transform.position.y, ZMaxLimit);
                    transform.position = _finalPosition;
                }
            }
        }

        private float ClampYAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }
    }
}