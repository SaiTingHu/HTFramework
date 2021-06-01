using UnityEngine.Events;
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

        private UnityAction<float> _callbackFloat;
        private UnityAction<string> _callbackString;

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
            _callbackFloat = (v) => { Value = (int)v; };
            _callbackString = (v) =>
            {
                int newValue;
                if (int.TryParse(v, out newValue)) Value = newValue;
            };
            Value = 0;
        }
        public BindableInt(int value)
        {
            _callbackFloat = (v) => { Value = (int)v; };
            _callbackString = (v) =>
            {
                int newValue;
                if (int.TryParse(v, out newValue)) Value = newValue;
            };
            Value = value;
        }

        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="control">绑定的目标控件</param>
        protected override void Binding(UIBehaviour control)
        {
            base.Binding(control);

            if (_bindedControls.Contains(control))
                return;

            if (control is InputField)
            {
                InputField inputField = control as InputField;
                inputField.text = Value.ToString();
                inputField.onValueChanged.AddListener(_callbackString);
                _onValueChanged += (value) => { if (inputField) inputField.text = value.ToString(); };
                _bindedControls.Add(control);
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value.ToString();
                _onValueChanged += (value) => { if (text) text.text = value.ToString(); };
                _bindedControls.Add(control);
            }
            else if (control is Slider)
            {
                Slider slider = control as Slider;
                slider.value = Value;
                slider.onValueChanged.AddListener(_callbackFloat);
                _onValueChanged += (value) => { if (slider) slider.value = value; };
                _bindedControls.Add(control);
            }
            else
            {
                Log.Warning(string.Format("自动化任务：数据绑定失败，当前不支持控件 {0} 与 BindableInt 类型的数据绑定！", control.GetType().FullName));
            }
        }
        /// <summary>
        /// 解除所有控件的绑定
        /// </summary>
        protected override void Unbind()
        {
            base.Unbind();

            foreach (var control in _bindedControls)
            {
                if (control == null)
                    continue;

                if (control is InputField)
                {
                    InputField inputField = control as InputField;
                    inputField.onValueChanged.RemoveListener(_callbackString);
                }
                else if (control is Slider)
                {
                    Slider slider = control as Slider;
                    slider.onValueChanged.RemoveListener(_callbackFloat);
                }
            }
            _onValueChanged = null;
            _bindedControls.Clear();
        }
    }
}