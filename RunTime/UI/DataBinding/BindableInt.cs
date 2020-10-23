using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的Int类型
    /// </summary>
    public sealed class BindableInt : BindableType<int>
    {
        public static implicit operator int(BindableInt bInt)
        {
            return bInt.Value;
        }

        public static implicit operator string(BindableInt bInt)
        {
            return bInt.Value.ToString();
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public override int Value
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

        public BindableInt()
        {
            Value = 0;
        }

        public BindableInt(int value)
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
                inputField.text = Value.ToString();
                inputField.onValueChanged.AddListener((value) =>
                {
                    int newValue;
                    if (int.TryParse(value, out newValue))
                    {
                        Value = newValue;
                    }
                });
                _onValueChanged += (value) => { if (inputField) inputField.text = value.ToString(); };
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value.ToString();
                _onValueChanged += (value) => { if (text) text.text = value.ToString(); };
            }
            else
            {
                Log.Warning(string.Format("数据驱动器：数据绑定失败，当前不支持控件 {0} 与 BindableInt 类型的数据绑定！", control.GetType().FullName));
            }
        }
    }
}