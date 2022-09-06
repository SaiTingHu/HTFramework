﻿using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 摄像机注视目标旋转控制
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class MouseRotation : HTBehaviour
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
        /// 允许在输入滚轮超越距离限制时，启用摄像机移动
        /// </summary>
        public bool AllowOverstepDistance = true;
        /// <summary>
        /// x轴旋转速度，y轴旋转速度，滚轮缩放速度
        /// </summary>
        public float XSpeed = 150, YSpeed = 150, MSpeed = 30;
        //y轴视角最低值，y轴视角最高值（目标是角色的话，推荐最小10，最大85，摄像机最低不会到角色脚底，最高不会到角色头顶正上方）
        //例如视角最低值为10，那么摄像机在y轴旋转最小只能到10
        //例如视角最高值为85，那么摄像机在y轴旋转最大只能到85
        public float YMinAngleLimit = -85, YMaxAngleLimit = 85;
        /// <summary>
        /// 摄像机与注释目标距离
        /// </summary>
        public float Distance = 2.0f;
        /// <summary>
        /// 摄像机与注释目标最小距离
        /// </summary>
        public float MinDistance = 0;
        /// <summary>
        /// 摄像机与注释目标最大距离
        /// </summary>
        public float MaxDistance = 4;
        /// <summary>
        /// 摄像机围绕目标旋转时是否使用阻尼缓动模式
        /// </summary>
        public bool NeedDamping = true;
        /// <summary>
        /// 阻尼缓冲时长
        /// </summary>
        public float DampingTime = 3;
        /// <summary>
        /// 初始的摄像机x轴旋转值
        /// </summary>
        public float X = 90.0f;
        /// <summary>
        /// 初始的摄像机y轴旋转值
        /// </summary>
        public float Y = 30.0f;
        
        /// <summary>
        /// 目标位置
        /// </summary>
        private Quaternion _rotation;
        private Vector3 _position;
        private Vector3 _disVector;
        
        /// <summary>
        /// 注视目标
        /// </summary>
        public CameraTarget Target { get; set; }
        /// <summary>
        /// 操作控制器
        /// </summary>
        public ControllerManager Manager { get; set; }
        
        /// <summary>
        /// 旋转注视视野
        /// </summary>
        /// <param name="x">视角x值</param>
        /// <param name="y">视角y值</param>
        /// <param name="distance">视角距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        public void SetAngle(float x, float y, float distance, bool damping = true)
        {
            X = x;
            Y = y;
            Distance = distance;

            if (!damping)
            {
                CalculateAngle();
                SwitchAngle(damping);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        public void OnUpdate()
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

            if (!IsCanOnUGUI && UIToolkit.IsStayUINotWorld)
                return;

            if (Main.m_Input.GetButton(InputButtonType.MouseRight))
            {
                X += Main.m_Input.GetAxis(InputAxisType.MouseX) * XSpeed * Time.deltaTime;
                Y -= Main.m_Input.GetAxis(InputAxisType.MouseY) * YSpeed * Time.deltaTime;
            }
            if (Main.m_Input.GetAxisRaw(InputAxisType.MouseScrollWheel) != 0)
            {
                Distance -= Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel) * MSpeed * Time.deltaTime;

                if (AllowOverstepDistance)
                {
                    if (Distance <= MinDistance || Distance >= MaxDistance)
                    {
                        Target.transform.Translate(transform.forward * Main.m_Input.GetAxis(InputAxisType.MouseScrollWheel) * MSpeed * Time.deltaTime);
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
        }
        private void CalculateAngle()
        {
            //将横向旋转值映射到区间[0,360]
            if (X > 360) X = X % 360;
            else if (X < 0) X = 360 + X;
            //将纵向旋转限制在视角最低值和最高值之间
            Y = ClampYAngle(Y, YMinAngleLimit, YMaxAngleLimit);
            //将视角距离限制在最小值和最大值之间
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            
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
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime * DampingTime);
                transform.position = Vector3.Lerp(transform.position, _position, Time.deltaTime * DampingTime);
            }
            //摄像机直接变换到新的位置
            else
            {
                transform.rotation = _rotation;
                transform.position = _position;
            }

            //摄像机位置限制
            if (Manager.IsEnableBounds)
            {
                //应用边界盒
                transform.position = ApplyBounds(transform.position);
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
            if (Manager.FreeControlBounds.Count == 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < Manager.FreeControlBounds.Count; i++)
                {
                    if (Manager.FreeControlBounds[i].Contains(position))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        private Vector3 ClosestPoint(Vector3 position)
        {
            if (Manager.FreeControlBounds.Count == 1)
            {
                return Manager.FreeControlBounds[0].ClosestPoint(position);
            }
            else
            {
                Bounds bounds = Manager.FreeControlBounds[0];
                float dis = Vector3.Distance(bounds.center, position);
                for (int i = 1; i < Manager.FreeControlBounds.Count; i++)
                {
                    float newdis = Vector3.Distance(Manager.FreeControlBounds[i].center, position);
                    if (newdis < dis)
                    {
                        bounds = Manager.FreeControlBounds[i];
                        dis = newdis;
                    }
                }
                return bounds.ClosestPoint(position);
            }
        }
    }
}