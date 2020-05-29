using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 输入管理器的助手接口
    /// </summary>
    public interface IInputHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 鼠标位置
        /// </summary>
        Vector3 MousePosition { get; set; }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualAxis(string name);
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualButton(string name);
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        void RegisterVirtualAxis(string name);
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        void RegisterVirtualButton(string name);
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        void UnRegisterVirtualAxis(string name);
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        void UnRegisterVirtualButton(string name);

        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        bool GetButton(string name);
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonDown(string name);
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        bool GetButtonUp(string name);
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="raw">是否获取整数值</param>
        /// <returns>轴线值</returns>
        float GetAxis(string name, bool raw);

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonDown(string name);
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonUp(string name);
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisPositive(string name);
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisNegative(string name);
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisZero(string name);
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">轴线值</param>
        void SetAxis(string name, float value);

        /// <summary>
        /// 清除所有输入状态
        /// </summary>
        void ResetAll();
    }
}