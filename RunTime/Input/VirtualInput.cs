using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 虚拟输入模块
    /// </summary>
    internal sealed class VirtualInput
    {
        private Dictionary<string, VirtualAxis> _virtualAxes = new Dictionary<string, VirtualAxis>();
        private Dictionary<string, VirtualButton> _virtualButtons = new Dictionary<string, VirtualButton>();
        private Vector3 _virtualMousePosition;
        
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
                GlobalTools.LogError(string.Format("注册虚拟轴线失败：已经存在名为 {0} 的虚拟轴线！", name));
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
                GlobalTools.LogError(string.Format("注册虚拟按钮失败：已经存在名为 {0} 的虚拟按钮！", name));
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
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            _virtualMousePosition.Set(x, y, z);
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value)
        {
            _virtualMousePosition = value;
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
            internal string Name { get; private set; }

            private int _lastPressedFrame = -5;
            private int _releasedFrame = -5;
            private bool _pressed = false;

            internal VirtualButton(string name)
            {
                Name = name;
            }

            /// <summary>
            /// 按钮按住
            /// </summary>
            internal void Pressed()
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
            internal void Released()
            {
                _pressed = false;
                _releasedFrame = Time.frameCount;
            }

            internal bool GetButton
            {
                get
                {
                    return _pressed;
                }
            }

            internal bool GetButtonDown
            {
                get
                {
                    return (_lastPressedFrame - Time.frameCount == -1);
                }
            }

            internal bool GetButtonUp
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
            internal string Name { get; private set; }

            private float _value;

            internal VirtualAxis(string name)
            {
                Name = name;
            }

            internal void Update(float value)
            {
                _value = value;
            }

            internal float GetValue
            {
                get
                {
                    return _value;
                }
            }

            internal float GetValueRaw
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