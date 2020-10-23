using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的Float类型
    /// </summary>
    public sealed class BindableFloat : BindableType<float>
    {
        public static implicit operator float(BindableFloat bFloat)
        {
            return bFloat.Value;
        }

        public static implicit operator string(BindableFloat bFloat)
        {
            return bFloat.Value.ToString("F4");
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public override float Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (_value.Approximately(value))
                    return;

                _value = value;
                _onValueChanged?.Invoke(_value);
            }
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
                    float newValue;
                    if (float.TryParse(value, out newValue))
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
            else if (control is Slider)
            {
                Slider slider = control as Slider;
                slider.value = Value;
                slider.onValueChanged.AddListener((value) => { Value = value; });
                _onValueChanged += (value) => { if (slider) slider.value = value; };
            }
            else if (control is Scrollbar)
            {
                Scrollbar scrollbar = control as Scrollbar;
                scrollbar.value = Value;
                scrollbar.onValueChanged.AddListener((value) => { Value = value; });
                _onValueChanged += (value) => { if (scrollbar) scrollbar.value = value; };
            }
            else
            {
                Log.Warning(string.Format("数据驱动器：数据绑定失败，当前不支持控件 {0} 与 BindableFloat 类型的数据绑定！", control.GetType().FullName));
            }
        }
    }
}