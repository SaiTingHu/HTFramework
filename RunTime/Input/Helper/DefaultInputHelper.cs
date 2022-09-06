﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的输入管理器助手
    /// </summary>
    public sealed class DefaultInputHelper : IInputHelper
    {
        private bool _isEnableInputDevice = true;
        private Dictionary<string, VirtualAxis> _virtualAxes = new Dictionary<string, VirtualAxis>();
        private Dictionary<string, VirtualButton> _virtualButtons = new Dictionary<string, VirtualButton>();

        /// <summary>
        /// 输入管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 输入设备
        /// </summary>
        public InputDeviceBase Device { get; private set; }
        /// <summary>
        /// 是否启用输入设备
        /// </summary>
        public bool IsEnableInputDevice
        {
            get
            {
                return _isEnableInputDevice;
            }
            set
            {
                _isEnableInputDevice = value;
                if (!_isEnableInputDevice)
                {
                    ResetAll();
                }
            }
        }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition { get; private set; }
        /// <summary>
        /// 任意键按住
        /// </summary>
        public bool AnyKey
        {
            get
            {
                return IsEnableInputDevice ? Input.anyKey : false;
            }
        }
        /// <summary>
        /// 任意键按下
        /// </summary>
        public bool AnyKeyDown
        {
            get
            {
                return IsEnableInputDevice ? Input.anyKeyDown : false;
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {
            Device.OnStartUp();

            Main.Current.ApplicationFocusEvent += (isFocus) =>
            {
                ResetAll();
            };
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (IsEnableInputDevice)
            {
                Device.OnRun();
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            Device.OnShutdown();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            ResetAll();
        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name)
        {
            return _virtualAxes.ContainsKey(name);
        }
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name)
        {
            return _virtualButtons.ContainsKey(name);
        }
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void RegisterVirtualAxis(string name)
        {
            if (_virtualAxes.ContainsKey(name))
            {
                Log.Error(string.Format("注册虚拟轴线失败：已经存在名为 {0} 的虚拟轴线！", name));
            }
            else
            {
                _virtualAxes.Add(name, new VirtualAxis(name));
            }
        }
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void RegisterVirtualButton(string name)
        {
            if (_virtualButtons.ContainsKey(name))
            {
                Log.Error(string.Format("注册虚拟按钮失败：已经存在名为 {0} 的虚拟按钮！", name));
            }
            else
            {
                _virtualButtons.Add(name, new VirtualButton(name));
            }
        }
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void UnRegisterVirtualAxis(string name)
        {
            if (_virtualAxes.ContainsKey(name))
            {
                _virtualAxes.Remove(name);
            }
        }
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void UnRegisterVirtualButton(string name)
        {
            if (_virtualButtons.ContainsKey(name))
            {
                _virtualButtons.Remove(name);
            }
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value)
        {
            MousePosition = value;
        }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            _virtualButtons[name].Pressed();
        }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            _virtualButtons[name].Released();
        }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(1);
        }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(-1);
        }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(0);
        }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">轴线值</param>
        public void SetAxis(string name, float value)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(value);
        }

        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            return _virtualButtons[name].GetButton;
        }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            return _virtualButtons[name].GetButtonDown;
        }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            return _virtualButtons[name].GetButtonUp;
        }
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="raw">是否获取整数值</param>
        /// <returns>轴线值</returns>
        public float GetAxis(string name, bool raw)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            return raw ? _virtualAxes[name].GetValueRaw : _virtualAxes[name].GetValue;
        }

        /// <summary>
        /// 键盘按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode)
        {
            return IsEnableInputDevice ? Input.GetKey(keyCode) : false;
        }
        /// <summary>
        /// 键盘按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode)
        {
            return IsEnableInputDevice ? Input.GetKeyDown(keyCode) : false;
        }
        /// <summary>
        /// 键盘按键抬起
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode)
        {
            return IsEnableInputDevice ? Input.GetKeyUp(keyCode) : false;
        }
        /// <summary>
        /// 键盘组合按键按住（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2)
        {
            return IsEnableInputDevice ? (Input.GetKey(keyCode1) && Input.GetKey(keyCode2)) : false;
        }
        /// <summary>
        /// 键盘组合按键按下（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2)
        {
            if (IsEnableInputDevice)
            {
                if (Input.GetKeyDown(keyCode2) && Input.GetKey(keyCode1))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 键盘组合按键抬起（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2)
        {
            if (IsEnableInputDevice)
            {
                if (Input.GetKeyUp(keyCode2) && Input.GetKey(keyCode1))
                {
                    return true;
                }
            }
            return false;
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
            return IsEnableInputDevice ? (Input.GetKey(keyCode1) && Input.GetKey(keyCode2) && Input.GetKey(keyCode3)) : false;
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
            if (IsEnableInputDevice)
            {
                if (Input.GetKeyDown(keyCode3) && Input.GetKey(keyCode1) && Input.GetKey(keyCode2))
                {
                    return true;
                }
            }
            return false;
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
            if (IsEnableInputDevice)
            {
                if (Input.GetKeyUp(keyCode3) && Input.GetKey(keyCode1) && Input.GetKey(keyCode2))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 加载输入设备
        /// </summary>
        /// <param name="deviceType">输入设备类型</param>
        public void LoadDevice(string deviceType)
        {
            Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(deviceType);
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(InputDeviceBase)))
                {
                    Device = Activator.CreateInstance(type) as InputDeviceBase;
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Input, "加载输入设备失败：输入设备类 " + deviceType + " 必须继承至输入设备基类：InputDeviceBase！");
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Input, "加载输入设备失败：丢失输入设备类 " + deviceType + "！");
            }
        }
        /// <summary>
        /// 清除所有输入状态
        /// </summary>
        public void ResetAll()
        {
            foreach (var item in _virtualAxes)
            {
                item.Value.Update(0);
            }
            foreach (var item in _virtualButtons)
            {
                item.Value.Released();
            }
        }

        /// <summary>
        /// 虚拟按钮
        /// </summary>
        private sealed class VirtualButton
        {
            public string Name { get; private set; }

            private int _pressedFrame = -5;
            private int _releasedFrame = -5;
            private bool _pressed = false;

            public VirtualButton(string name)
            {
                Name = name;
            }
            
            public void Pressed()
            {
                if (_pressed)
                    return;

                _pressed = true;
                _pressedFrame = Time.frameCount;
            }
            
            public void Released()
            {
                if (!_pressed)
                    return;

                _pressed = false;
                _releasedFrame = Time.frameCount;
            }

            public bool GetButton
            {
                get
                {
                    return _pressed;
                }
            }

            public bool GetButtonDown
            {
                get
                {
                    return _pressedFrame == Time.frameCount;
                }
            }

            public bool GetButtonUp
            {
                get
                {
                    return _releasedFrame == Time.frameCount;
                }
            }
        }
        /// <summary>
        /// 虚拟轴线
        /// </summary>
        private sealed class VirtualAxis
        {
            public string Name { get; private set; }

            private float _value;

            public VirtualAxis(string name)
            {
                Name = name;
            }

            public void Update(float value)
            {
                _value = value;
            }

            public float GetValue
            {
                get
                {
                    return _value;
                }
            }

            public float GetValueRaw
            {
                get
                {
                    if (_value < 0) return -1;
                    else if (_value > 0) return 1;
                    else return 0;
                }
            }
        }
    }
}