using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的输入管理器助手
    /// </summary>
    public sealed class DefaultInputHelper : IInputHelper
    {
        private Dictionary<string, VirtualAxis> _virtualAxes = new Dictionary<string, VirtualAxis>();
        private Dictionary<string, VirtualButton> _virtualButtons = new Dictionary<string, VirtualButton>();

        /// <summary>
        /// 输入管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition { get; set; }

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

            private int _lastPressedFrame = -5;
            private int _releasedFrame = -5;
            private bool _pressed = false;

            public VirtualButton(string name)
            {
                Name = name;
            }
            
            public void Pressed()
            {
                if (_pressed)
                {
                    return;
                }
                _pressed = true;
                _lastPressedFrame = Time.frameCount;
            }
            
            public void Released()
            {
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
                    return _lastPressedFrame - Time.frameCount == -1;
                }
            }

            public bool GetButtonUp
            {
                get
                {
                    return _releasedFrame == Time.frameCount - 1;
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