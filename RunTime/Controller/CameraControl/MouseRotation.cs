using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标旋转控制
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MouseRotation : MonoBehaviour
    {
        //注视目标
        public CameraTarget Target;
        //注视点x、y轴偏移，若都为0，则注视点等于注视目标的transform.position
        public float OffsetX = 0f;
        public float OffsetY = 0f;
        //x轴旋转速度，y轴旋转速度，滚轮缩放速度
        public float XSpeed = 150, YSpeed = 150, MSpeed = 30;
        //y轴视角最低值，y轴视角最高值（目标是角色的话，推荐最小10，最大85，摄像机最低不会到角色脚底，最高不会到角色头顶正上方）
        //例如视角最低值为10，那么摄像机在y轴旋转最小只能到10
        //例如视角最高值为85，那么摄像机在y轴旋转最大只能到85
        public float YMinAngleLimit = -85, YMaxAngleLimit = 85;
        //摄像机与注释目标距离
        public float Distance = 2.0f;
        //摄像机与注释目标最小距离
        public float MinDistance = 0;
        //摄像机与注释目标最大距离
        public float MaxDistance = 4;
        //摄像机围绕目标旋转时是否使用阻尼缓动模式
        public bool NeedDamping = true;
        //初始的摄像机x轴旋转值，y轴旋转值
        public float X = 90.0f;
        public float Y = 30.0f;
        //是否限定旋转位置
        public bool NeedLimit = false;
        //x轴旋转最低值，x轴旋转最高值
        public float XMinLimit = -5, XMaxLimit = 5;
        //y轴旋转最低值，y轴旋转最高值
        public float YMinLimit = 0.1f, YMaxLimit = 5;
        //z轴旋转最低值，z轴旋转最高值
        public float ZMinLimit = -5, ZMaxLimit = 5;
        //在UGUI目标上是否可以控制
        public bool IsCanOnUGUI = false;
        //允许在输入滚轮超越距离限制时，启用摄像机移动
        public bool AllowOverstepDistance = true;

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
        /// <param name="value">视野旋转时，视角在x,y,z三个轴的最小值</param>
        public void SetMinLimit(Vector3 value)
        {
            XMinLimit = value.x;
            YMinLimit = value.y;
            ZMinLimit = value.z;
        }

        /// <summary>
        /// 设置旋转限定最大值
        /// </summary>
        /// <param name="value">视野旋转时，视角在x,y,z三个轴的最大值</param>
        public void SetMaxLimit(Vector3 value)
        {
            XMaxLimit = value.x;
            YMaxLimit = value.y;
            ZMaxLimit = value.z;
        }

        /// <summary>
        /// 旋转注视视野
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetAngle(Vector3 angle, bool damping = true)
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
        /// 旋转注视视野
        /// </summary>
        /// <param name="angle">目标角度</param>
        /// <param name="distance">距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetAngle(Vector2 angle, float distance, bool damping = true)
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
        public void OnRefresh()
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

            if (Main.m_Input.GetButton(InputButtonType.MouseRight))
            {
                X += Main.m_Input.GetAxis(InputAxisType.MouseX) * XSpeed * _factor;
                Y -= Main.m_Input.GetAxis(InputAxisType.MouseY) * YSpeed * _factor;
            }
            if (Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel) != 0)
            {
                Distance -= Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel) * MSpeed * Time.deltaTime;

                if (AllowOverstepDistance)
                {
                    if (Distance <= MinDistance)
                    {
                        Target.transform.Translate(transform.forward * Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel));
                    }
                    else if (Distance >= MaxDistance)
                    {
                        Target.transform.Translate(transform.forward * Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel) * -1);
                    }
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
            //将纵向旋转限制在视角最低值和最高值之间
            Y = ClampYAngle(Y, YMinAngleLimit, YMaxAngleLimit);
            //将视角距离限制在最小值和最大值之间
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

            //重新获取摄像机注视点
            _targetPoint.Set(Target.transform.position.x + OffsetX, Target.transform.position.y + OffsetY, Target.transform.position.z);

            //摄像机的最新旋转角度
            _rotation = Quaternion.Euler(Y, X, 0f);
            //摄像机与注视点的距离向量
            _disVector.Set(0f, 0f, -Distance);
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
                transform.position = GlobalTools.Clamp(transform.position, XMinLimit, YMinLimit, ZMinLimit, XMaxLimit, YMaxLimit, ZMaxLimit);
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