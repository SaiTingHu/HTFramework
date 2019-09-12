using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 输入管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputManager : ModuleManagerBase
    {
        /// <summary>
        /// 输入设备类型【请勿在代码中修改】
        /// </summary>
        public string InputDeviceType = "";

        private VirtualInput _inputModule;
        private InputDeviceBase _inputDevice;

        /// <summary>
        /// 是否启用输入设备
        /// </summary>
        public bool IsEnableInputDevice { get; set; } = true;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _inputModule = new VirtualInput();

            //加载输入设备
            Type type = GlobalTools.GetTypeInRunTimeAssemblies(InputDeviceType);
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(InputDeviceBase)))
                {
                    _inputDevice = Activator.CreateInstance(type) as InputDeviceBase;
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Input, "加载输入设备失败：输入设备类 " + InputDeviceType + " 必须继承至输入设备基类：InputDeviceBase！");
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Input, "加载输入设备失败：丢失输入设备类 " + InputDeviceType + "！");
            }
        }

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            _inputDevice.OnStartUp();
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableInputDevice)
            {
                _inputDevice.OnRun();
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            _inputDevice.OnShutdown();
        }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name)
        {
            return _inputModule.IsExistVirtualAxis(name);
        }
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name)
        {
            return _inputModule.IsExistVirtualButton(name);
        }
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void RegisterVirtualAxis(string name)
        {
            _inputModule.RegisterVirtualAxis(name);
        }
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void RegisterVirtualButton(string name)
        {
            _inputModule.RegisterVirtualButton(name);
        }
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void UnRegisterVirtualAxis(string name)
        {
            _inputModule.UnRegisterVirtualAxis(name);
        }
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void UnRegisterVirtualButton(string name)
        {
            _inputModule.UnRegisterVirtualButton(name);
        }
        
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition
        {
            get
            {
                return _inputModule.MousePosition;
            }
        }
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxis(string name)
        {
            return _inputModule.GetAxis(name, false);
        }
        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxisRaw(string name)
        {
            return _inputModule.GetAxis(name, true);
        }
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string name)
        {
            return _inputModule.GetButton(name);
        }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return _inputModule.GetButtonDown(name);
        }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            return _inputModule.GetButtonUp(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            _inputModule.SetVirtualMousePosition(x, y, z);
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value)
        {
            _inputModule.SetVirtualMousePosition(value);
        }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name)
        {
            _inputModule.SetButtonDown(name);
        }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name)
        {
            _inputModule.SetButtonUp(name);
        }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name)
        {
            _inputModule.SetAxisPositive(name);
        }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name)
        {
            _inputModule.SetAxisNegative(name);
        }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name)
        {
            _inputModule.SetAxisZero(name);
        }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        public void SetAxis(string name, float value)
        {
            _inputModule.SetAxis(name, value);
        }
    }
}