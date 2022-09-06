﻿using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Input)]
    public sealed class InputManager : InternalModuleBase<IInputHelper>
    {
        /// <summary>
        /// 输入设备类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string InputDeviceType = "";

        /// <summary>
        /// 模块的优先级（越小越优先）
        /// </summary>
        public override int Priority
        {
            get
            {
                return -2;
            }
        }
        /// <summary>
        /// 是否启用输入设备
        /// </summary>
        public bool IsEnableInputDevice
        {
            get
            {
                return _helper.IsEnableInputDevice;
            }
            set
            {
                _helper.IsEnableInputDevice = value;
            }
        }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition
        {
            get
            {
                return _helper.MousePosition;
            }
        }
        /// <summary>
        /// 任意键按住
        /// </summary>
        public bool AnyKey
        {
            get
            {
                return _helper.AnyKey;
            }
        }
        /// <summary>
        /// 任意键按下
        /// </summary>
        public bool AnyKeyDown
        {
            get
            {
                return _helper.AnyKeyDown;
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _helper.LoadDevice(InputDeviceType);
        }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        internal bool IsExistVirtualAxis(string name)
        {
            return _helper.IsExistVirtualAxis(name);
        }
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        internal bool IsExistVirtualButton(string name)
        {
            return _helper.IsExistVirtualButton(name);
        }
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void RegisterVirtualAxis(string name)
        {
            _helper.RegisterVirtualAxis(name);
        }
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void RegisterVirtualButton(string name)
        {
            _helper.RegisterVirtualButton(name);
        }
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void UnRegisterVirtualAxis(string name)
        {
            _helper.UnRegisterVirtualAxis(name);
        }
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void UnRegisterVirtualButton(string name)
        {
            _helper.UnRegisterVirtualButton(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        internal void SetVirtualMousePosition(float x, float y, float z)
        {
            _helper.SetVirtualMousePosition(new Vector3(x, y, z));
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        internal void SetVirtualMousePosition(Vector3 value)
        {
            _helper.SetVirtualMousePosition(value);
        }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void SetButtonDown(string name)
        {
            _helper.SetButtonDown(name);
        }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void SetButtonUp(string name)
        {
            _helper.SetButtonUp(name);
        }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisPositive(string name)
        {
            _helper.SetAxisPositive(name);
        }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisNegative(string name)
        {
            _helper.SetAxisNegative(name);
        }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisZero(string name)
        {
            _helper.SetAxisZero(name);
        }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        internal void SetAxis(string name, float value)
        {
            _helper.SetAxis(name, value);
        }

        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string name)
        {
            return _helper.GetButton(name);
        }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return _helper.GetButtonDown(name);
        }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            return _helper.GetButtonUp(name);
        }
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxis(string name)
        {
            return _helper.GetAxis(name, false);
        }
        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxisRaw(string name)
        {
            return _helper.GetAxis(name, true);
        }

        /// <summary>
        /// 键盘按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode)
        {
            return _helper.GetKey(keyCode);
        }
        /// <summary>
        /// 键盘按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode)
        {
            return _helper.GetKeyDown(keyCode);
        }
        /// <summary>
        /// 键盘按键抬起
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode)
        {
            return _helper.GetKeyUp(keyCode);
        }
        /// <summary>
        /// 键盘组合按键按住（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKey(keyCode1, keyCode2);
        }
        /// <summary>
        /// 键盘组合按键按下（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKeyDown(keyCode1, keyCode2);
        }
        /// <summary>
        /// 键盘组合按键抬起（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKeyUp(keyCode1, keyCode2);
        }
        /// <summary>
        /// 键盘组合按键按住（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKey(keyCode1, keyCode2, keyCode3);
        }
        /// <summary>
        /// 键盘组合按键按下（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKeyDown(keyCode1, keyCode2, keyCode3);
        }
        /// <summary>
        /// 键盘组合按键抬起（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKeyUp(keyCode1, keyCode2, keyCode3);
        }
    }
}