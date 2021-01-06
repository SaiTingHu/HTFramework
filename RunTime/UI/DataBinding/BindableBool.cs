using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的Bool类型
    /// </summary>
    public sealed class BindableBool : BindableType<bool>
    {
        public static implicit operator bool(BindableBool bBool)
        {
            return bBool.Value;
        }
        public static implicit operator string(BindableBool bBool)
        {
            return bBool.Value.ToString();
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public override bool Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (_value == value)
                    return;

                _value = value;
                _onValueChanged?.Invoke(_value);
            }
        }

        public BindableBool()
        {
            Value = false;
        }
        public BindableBool(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="control">绑定的目标控件</param>
        protected override void Binding(UIBehaviour control)
        {
            base.Binding(control);

            if (control is Toggle)
            {
                Toggle toggle = control as Toggle;
                toggle.isOn = Value;
                toggle.onValueChanged.AddListener((value) => { Value = value; });
                _onValueChanged += (value) => { if (toggle) toggle.isOn = value; };
            }
            else if (control is Button)
            {
                Button button = control as Button;
                button.interactable = Value;
                _onValueChanged += (value) => { if (button) button.interactable = value; };
            }
            else
            {
                Log.Warning(string.Format("数据驱动器：数据绑定失败，当前不支持控件 {0} 与 BindableBool 类型的数据绑定！", control.GetType().FullName));
            }
        }
    }
}