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
            Main.m_Input.RegisterVirtualButton("MouseLeft");
            Main.m_Input.RegisterVirtualButton("MouseRight");
            Main.m_Input.RegisterVirtualButton("MouseMiddle");
            Main.m_Input.RegisterVirtualButton("MouseLeftDoubleClick");

            Main.m_Input.RegisterVirtualAxis("MouseX");
            Main.m_Input.RegisterVirtualAxis("MouseY");
            Main.m_Input.RegisterVirtualAxis("MouseScrollWheel");
            Main.m_Input.RegisterVirtualAxis("Horizontal");
            Main.m_Input.RegisterVirtualAxis("Vertical");
            Main.m_Input.RegisterVirtualAxis("UpperLower");
        }

        public override void OnRun()
        {
            //标准PC平台：鼠标和键盘做为输入设备
            if (Input.GetMouseButtonDown(0)) Main.m_Input.SetButtonDown("MouseLeft");
            else if (Input.GetMouseButtonUp(0)) Main.m_Input.SetButtonUp("MouseLeft");

            if (Input.GetMouseButtonDown(1)) Main.m_Input.SetButtonDown("MouseRight");
            else if (Input.GetMouseButtonUp(1)) Main.m_Input.SetButtonUp("MouseRight");

            if (Input.GetMouseButtonDown(2)) Main.m_Input.SetButtonDown("MouseMiddle");
            else if (Input.GetMouseButtonUp(2)) Main.m_Input.SetButtonUp("MouseMiddle");
            
            if (Input.GetMouseButtonDown(0))
            {
                if (_mouseLeftClickTimer <= 0)
                {
                    _mouseLeftClickTimer = _mouseLeftDoubleClickInterval;
                }
                else
                {
                    Main.m_Input.SetButtonDown("MouseLeftDoubleClick");
                    Main.m_Input.SetButtonUp("MouseLeftDoubleClick");
                }
            }
            if (_mouseLeftClickTimer > 0)
            {
                _mouseLeftClickTimer -= Time.deltaTime;
            }

            Main.m_Input.SetAxis("MouseX", Input.GetAxis("Mouse X"));
            Main.m_Input.SetAxis("MouseY", Input.GetAxis("Mouse Y"));
            Main.m_Input.SetAxis("MouseScrollWheel", Input.GetAxis("Mouse ScrollWheel"));
            Main.m_Input.SetAxis("Horizontal", Input.GetAxis("Horizontal"));
            Main.m_Input.SetAxis("Vertical", Input.GetAxis("Vertical"));

            if (Input.GetKey(KeyCode.Q)) _upperLowerValue -= Time.deltaTime;
            else if (Input.GetKey(KeyCode.E)) _upperLowerValue += Time.deltaTime;
            else _upperLowerValue = 0;
            Main.m_Input.SetAxis("UpperLower", Mathf.Clamp(_upperLowerValue, -1, 1));

            Main.m_Input.SetVirtualMousePosition(Input.mousePosition);
        }

        public override void OnShutdown()
        {
            Main.m_Input.UnRegisterVirtualButton("MouseLeft");
            Main.m_Input.UnRegisterVirtualButton("MouseRight");
            Main.m_Input.UnRegisterVirtualButton("MouseMiddle");
            Main.m_Input.UnRegisterVirtualButton("MouseLeftDoubleClick");

            Main.m_Input.UnRegisterVirtualAxis("MouseX");
            Main.m_Input.UnRegisterVirtualAxis("MouseY");
            Main.m_Input.UnRegisterVirtualAxis("MouseScrollWheel");
            Main.m_Input.UnRegisterVirtualAxis("Horizontal");
            Main.m_Input.UnRegisterVirtualAxis("Vertical");
            Main.m_Input.UnRegisterVirtualAxis("UpperLower");
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
