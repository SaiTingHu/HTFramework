using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 操作控制器的助手接口
    /// </summary>
    public interface IControllerHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 控制模式
        /// </summary>
        ControlMode TheControlMode { get; set; }
        /// <summary>
        /// 主摄像机
        /// </summary>
        Camera MainCamera { get; }
        /// <summary>
        /// 自由控制：当前摄像机注视点
        /// </summary>
        Vector3 LookPoint { get; }
        /// <summary>
        /// 自由控制：当前摄像机注视视角
        /// </summary>
        Vector3 LookAngle { get; }
        /// <summary>
        /// 自由控制：是否启用摄像机移动控制
        /// </summary>
        bool EnablePositionControl { get; set; }
        /// <summary>
        /// 自由控制：是否启用摄像机旋转控制
        /// </summary>
        bool EnableRotationControl { get; set; }
        /// <summary>
        /// 自由控制：在UGUI目标上是否可以控制
        /// </summary>
        bool IsCanControlOnUGUI { get; set; }
        /// <summary>
        /// 自由控制：允许在输入滚轮超越距离限制时，启用摄像机移动
        /// </summary>
        bool AllowOverstepDistance { get; set; }
        /// <summary>
        /// 自由控制：摄像机是否始终保持注视目标
        /// </summary>
        bool IsLookAtTarget { get; set; }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        MouseRayTargetBase RayTarget { get; }
        /// <summary>
        /// 当前射线击中的目标
        /// </summary>
        GameObject RayTargetObj { get; }
        /// <summary>
        /// 当前射线击中的点
        /// </summary>
        Vector3 RayHitPoint { get; }
        /// <summary>
        /// 是否启用高光特效
        /// </summary>
        bool EnableHighlightingEffect { get; set; }
        /// <summary>
        /// 是否启用鼠标射线
        /// </summary>
        bool EnableMouseRay { get; set; }
        /// <summary>
        /// 是否启用鼠标射线击中提示框
        /// </summary>
        bool EnableMouseRayHitPrompt { get; set; }
        /// <summary>
        /// 高亮组件是否自动销毁
        /// </summary>
        bool HighlightAutoDie { get; set; }
        /// <summary>
        /// 射线投射事件(MouseRayTargetBase：当前射中的目标，Vector3：当前射中的点，Vector2：当前鼠标位置转换后的UGUI坐标)
        /// </summary>
        event HTFAction<MouseRayTargetBase, Vector3, Vector2> RayEvent;
        
        /// <summary>
        /// 自由控制：设置摄像机注视点
        /// </summary>
        /// <param name="point">目标位置</param>
        /// <param name="damping">阻尼缓动模式</param>
        void SetLookPoint(Vector3 point, bool damping = true);
        /// <summary>
        /// 自由控制：设置摄像机注视角度
        /// </summary>
        /// <param name="x">视角x值</param>
        /// <param name="y">视角y值</param>
        /// <param name="distance">视角距离</param>
        /// <param name="damping">阻尼缓动模式</param>
        void SetLookAngle(float x, float y, float distance, bool damping = true);
        /// <summary>
        /// 自由控制：设置视角移动速度
        /// </summary>
        /// <param name="x">x轴移动速度</param>
        /// <param name="y">y轴移动速度</param>
        /// <param name="z">z轴移动速度</param>
        void SetMoveSpeed(float x, float y, float z);
        /// <summary>
        /// 自由控制：设置视角旋转速度
        /// </summary>
        /// <param name="x">x轴旋转速度</param>
        /// <param name="y">y轴旋转速度</param>
        /// <param name="m">滚轮缩放速度</param>
        void SetRotateSpeed(float x, float y, float m);
        /// <summary>
        /// 自由控制：设置摄像机旋转时视角Y轴的限制
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        void SetAngleLimit(float min, float max);
        /// <summary>
        /// 自由控制：设置摄像机注视距离的最小值和最大值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        void SetMinMaxDistance(float min, float max);
        /// <summary>
        /// 自由控制：进入保持追踪模式
        /// </summary>
        /// <param name="target">追踪目标</param>
        void EnterKeepTrack(Transform target);
        /// <summary>
        /// 自由控制：退出保持追踪模式
        /// </summary>
        void LeaveKeepTrack();
        /// <summary>
        /// 设置射线发射器的焦点提示框
        /// </summary>
        /// <param name="background">提示框背景</param>
        /// <param name="content">提示文字框</param>
        /// <param name="uIType">提示框UI类型</param>
        void SetMouseRayFocusImage(Image background, Text content, UIType uIType = UIType.Overlay);

        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        void AddClickListener(GameObject target, HTFAction callback);
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        void RemoveClickListener(GameObject target);
        /// <summary>
        /// 清空所有点击事件
        /// </summary>
        void ClearClickListener();
    }
}