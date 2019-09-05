using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 标准输入设备
    /// </summary>
    public sealed class StandardInputDevice : InputDeviceBase
    {
        /// <summary>
        /// 鼠标左键双击时间间隔
        /// </summary>
        private readonly float _mouseLeftDoubleClickInterval = 0.3f;
        /// <summary>
        /// 鼠标左键单击计时器
        /// </summary>
        private float _mouseLeftClickTimer = 0;
        /// <summary>
        /// UpperLower轴线值
        /// </summary>
        private float _upperLowerValue = 0;

        public override void OnStartUp()
        {
            Main.m_Input.RegisterVirtualButton(InputButtonType.MouseLeft);
            Main.m_Input.RegisterVirtualButton(InputButtonType.MouseRight);
            Main.m_Input.RegisterVirtualButton(InputButtonType.MouseMiddle);
            Main.m_Input.RegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);

            Main.m_Input.RegisterVirtualAxis(InputAxisType.MouseX);
            Main.m_Input.RegisterVirtualAxis(InputAxisType.MouseY);
            Main.m_Input.RegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Main.m_Input.RegisterVirtualAxis(InputAxisType.Horizontal);
            Main.m_Input.RegisterVirtualAxis(InputAxisType.Vertical);
            Main.m_Input.RegisterVirtualAxis(InputAxisType.UpperLower);
        }

        public override void OnRun()
        {
            //标准PC平台：鼠标和键盘做为输入设备
            if (Input.GetMouseButtonDown(0)) Main.m_Input.SetButtonDown(InputButtonType.MouseLeft);
            else if (Input.GetMouseButtonUp(0)) Main.m_Input.SetButtonUp(InputButtonType.MouseLeft);

            if (Input.GetMouseButtonDown(1)) Main.m_Input.SetButtonDown(InputButtonType.MouseRight);
            else if (Input.GetMouseButtonUp(1)) Main.m_Input.SetButtonUp(InputButtonType.MouseRight);

            if (Input.GetMouseButtonDown(2)) Main.m_Input.SetButtonDown(InputButtonType.MouseMiddle);
            else if (Input.GetMouseButtonUp(2)) Main.m_Input.SetButtonUp(InputButtonType.MouseMiddle);
            
            if (Input.GetMouseButtonDown(0))
            {
                if (_mouseLeftClickTimer <= 0)
                {
                    _mouseLeftClickTimer = _mouseLeftDoubleClickInterval;
                }
                else
                {
                    Main.m_Input.SetButtonDown(InputButtonType.MouseLeftDoubleClick);
                    Main.m_Input.SetButtonUp(InputButtonType.MouseLeftDoubleClick);
                }
            }
            if (_mouseLeftClickTimer > 0)
            {
                _mouseLeftClickTimer -= Time.deltaTime;
            }

            Main.m_Input.SetAxis(InputAxisType.MouseX, Input.GetAxis("Mouse X"));
            Main.m_Input.SetAxis(InputAxisType.MouseY, Input.GetAxis("Mouse Y"));
            Main.m_Input.SetAxis(InputAxisType.MouseScrollWheel, Input.GetAxis("Mouse ScrollWheel"));
            Main.m_Input.SetAxis(InputAxisType.Horizontal, Input.GetAxis("Horizontal"));
            Main.m_Input.SetAxis(InputAxisType.Vertical, Input.GetAxis("Vertical"));

            if (Input.GetKey(KeyCode.Q)) _upperLowerValue -= Time.deltaTime;
            else if (Input.GetKey(KeyCode.E)) _upperLowerValue += Time.deltaTime;
            else _upperLowerValue = 0;
            Main.m_Input.SetAxis(InputAxisType.UpperLower, Mathf.Clamp(_upperLowerValue, -1, 1));

            Main.m_Input.SetVirtualMousePosition(Input.mousePosition);
        }

        public override void OnShutdown()
        {
            Main.m_Input.UnRegisterVirtualButton(InputButtonType.MouseLeft);
            Main.m_Input.UnRegisterVirtualButton(InputButtonType.MouseRight);
            Main.m_Input.UnRegisterVirtualButton(InputButtonType.MouseMiddle);
            Main.m_Input.UnRegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);

            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.MouseX);
            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.MouseY);
            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.Horizontal);
            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.Vertical);
            Main.m_Input.UnRegisterVirtualAxis(InputAxisType.UpperLower);
        }
    }

    /// <summary>
    /// 输入按键类型
    /// </summary>
    public static class InputButtonType
    {
        /// <summary>
        /// 鼠标左键
        /// </summary>
        public static string MouseLeft
        {
            get
            {
                return "MouseLeft";
            }
        }
        /// <summary>
        /// 鼠标右键
        /// </summary>
        public static string MouseRight
        {
            get
            {
                return "MouseRight";
            }
        }
        /// <summary>
        /// 鼠标中键
        /// </summary>
        public static string MouseMiddle
        {
            get
            {
                return "MouseMiddle";
            }
        }
        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        public static string MouseLeftDoubleClick
        {
            get
            {
                return "MouseLeftDoubleClick";
            }
        }
    }

    /// <summary>
    /// 输入轴线类型
    /// </summary>
    public static class InputAxisType
    {
        /// <summary>
        /// 鼠标X轴移动
        /// </summary>
        public static string MouseX
        {
            get
            {
                return "MouseX";
            }
        }
        /// <summary>
        /// 鼠标Y轴移动
        /// </summary>
        public static string MouseY
        {
            get
            {
                return "MouseY";
            }
        }
        /// <summary>
        /// 鼠标滚轮滚动
        /// </summary>
        public static string MouseScrollWheel
        {
            get
            {
                return "MouseScrollWheel";
            }
        }
        /// <summary>
        /// 键盘水平输入
        /// </summary>
        public static string Horizontal
        {
            get
            {
                return "Horizontal";
            }
        }
        /// <summary>
        /// 键盘垂直输入
        /// </summary>
        public static string Vertical
        {
            get
            {
                return "Vertical";
            }
        }
        /// <summary>
        /// 键盘上下输入
        /// </summary>
        public static string UpperLower
        {
            get
            {
                return "UpperLower";
            }
        }
    }
}
