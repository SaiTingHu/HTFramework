using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 虚拟输入模块
    /// </summary>
    public sealed class VirtualInput
    {
        private Dictionary<string, VirtualAxis> _virtualAxes = new Dictionary<string, VirtualAxis>();
        private Dictionary<string, VirtualButton> _virtualButtons = new Dictionary<string, VirtualButton>();
        private Vector3 _virtualMousePosition;
        
        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        public bool IsExistVirtualAxis(string name)
        {
            return _virtualAxes.ContainsKey(name);
        }
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        public bool IsExistVirtualButton(string name)
        {
            return _virtualButtons.ContainsKey(name);
        }
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        public void RegisterVirtualAxis(string name)
        {
            if (_virtualAxes.ContainsKey(name))
            {
                GlobalTools.LogError("注册虚拟轴线失败：已经存在名为 " + name + " 的虚拟轴线！");
            }
            else
            {
                _virtualAxes.Add(name, new VirtualAxis(name));
            }
        }
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        public void RegisterVirtualButton(string name)
        {
            if (_virtualButtons.ContainsKey(name))
            {
                GlobalTools.LogError("注册虚拟按钮失败：已经存在名为 " + name + " 的虚拟按钮！");
            }
            else
            {
                _virtualButtons.Add(name, new VirtualButton(name));
            }
        }
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
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
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            _virtualMousePosition.Set(x, y, z);
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        public void SetVirtualMousePosition(Vector3 value)
        {
            _virtualMousePosition = value;
        }
        
        /// <summary>
        /// 按钮按住
        /// </summary>
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
        public bool GetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            return _virtualButtons[name].GetButtonUp;
        }
        /// <summary>
        /// 设置按钮按下
        /// </summary>
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
        public void SetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
            {
                RegisterVirtualButton(name);
            }
            _virtualButtons[name].Released();
        }

        /// <summary>
        /// 获取轴线值
        /// </summary>
        public float GetAxis(string name, bool raw)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            return raw ? _virtualAxes[name].GetValueRaw : _virtualAxes[name].GetValue;
        }
        /// <summary>
        /// 设置轴线值为正数最大值
        /// </summary>
        public void SetAxisPositive(string name)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(1);
        }
        /// <summary>
        /// 设置轴线值为负数最小值
        /// </summary>
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
        public void SetAxis(string name, float value)
        {
            if (!IsExistVirtualAxis(name))
            {
                RegisterVirtualAxis(name);
            }
            _virtualAxes[name].Update(value);
        }
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition
        {
            get
            {
                return _virtualMousePosition;
            }
        }

        /// <summary>
        /// 虚拟按钮
        /// </summary>
        public sealed class VirtualButton
        {
            public string Name { get; private set; }

            private int _lastPressedFrame = -5;
            private int _releasedFrame = -5;
            private bool _pressed = false;

            public VirtualButton(string name)
            {
                Name = name;
            }
            
            /// <summary>
            /// 按钮按住
            /// </summary>
            public void Pressed()
            {
                if (_pressed)
                {
                    return;
                }
                _pressed = true;
                _lastPressedFrame = Time.frameCount;
            }

            /// <summary>
            /// 按钮释放
            /// </summary>
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
                    return (_lastPressedFrame - Time.frameCount == -1);
                }
            }

            public bool GetButtonUp
            {
                get
                {
                    return (_releasedFrame == Time.frameCount - 1);
                }
            }
        }
        /// <summary>
        /// 虚拟轴线
        /// </summary>
        public sealed class VirtualAxis
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

                if (_value < -1) _value = -1;
                else if (_value > 1) _value = 1;
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
