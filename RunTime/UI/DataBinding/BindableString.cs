using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的String类型
    /// </summary>
    public sealed class BindableString : BindableType<string>
    {
        public static implicit operator string(BindableString bString)
        {
            return bString.Value;
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public override string Value
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

        public BindableString()
        {
            Value = "";
        }

        public BindableString(string value)
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

            if (control is InputField)
            {
                InputField inputField = control as InputField;
                inputField.text = Value;
                inputField.onValueChanged.AddListener((value) => { Value = value; });
                _onValueChanged += (value) => { if (inputField) inputField.text = value; };
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value;
                _onValueChanged += (value) => { if (text) text.text = value; };
            }
            else
            {
                Log.Warning(string.Format("数据驱动器：数据绑定失败，当前不支持控件 {0} 与 BindableString 类型的数据绑定！", control.GetType().FullName));
            }
        }
    }
}