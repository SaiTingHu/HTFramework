using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 输入设备基类
    /// </summary>
    public abstract class InputDeviceBase
    {
        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        protected bool IsExistVirtualAxis(string name)
        {
            return Main.m_Input.IsExistVirtualAxis(name);
        }
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        protected bool IsExistVirtualButton(string name)
        {
            return Main.m_Input.IsExistVirtualButton(name);
        }
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void RegisterVirtualAxis(string name)
        {
            Main.m_Input.RegisterVirtualAxis(name);
        }
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void RegisterVirtualButton(string name)
        {
            Main.m_Input.RegisterVirtualButton(name);
        }
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void UnRegisterVirtualAxis(string name)
        {
            Main.m_Input.UnRegisterVirtualAxis(name);
        }
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void UnRegisterVirtualButton(string name)
        {
            Main.m_Input.UnRegisterVirtualButton(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        protected void SetVirtualMousePosition(float x, float y, float z)
        {
            Main.m_Input.SetVirtualMousePosition(new Vector3(x, y, z));
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        protected void SetVirtualMousePosition(Vector3 value)
        {
            Main.m_Input.SetVirtualMousePosition(value);
        }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void SetButtonDown(string name)
        {
            Main.m_Input.SetButtonDown(name);
        }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void SetButtonUp(string name)
        {
            Main.m_Input.SetButtonUp(name);
        }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisPositive(string name)
        {
            Main.m_Input.SetAxisPositive(name);
        }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisNegative(string name)
        {
            Main.m_Input.SetAxisNegative(name);
        }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisZero(string name)
        {
            Main.m_Input.SetAxisZero(name);
        }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        protected void SetAxis(string name, float value)
        {
            Main.m_Input.SetAxis(name, value);
        }

        /// <summary>
        /// 设备启动
        /// </summary>
        public abstract void OnStartUp();
        /// <summary>
        /// 设备运作
        /// </summary>
        public abstract void OnRun();
        /// <summary>
        /// 设备关闭
        /// </summary>
        public abstract void OnShutdown();
    }
}