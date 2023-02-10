using UnityEngine.Events;
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

        private UnityAction<float> _callbackFloat;
        private UnityAction<string> _callbackString;

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
                OnValueChanged?.Invoke(_value);
            }
        }

        public BindableFloat()
        {
            _callbackFloat = (v) => { Value = v; };
            _callbackString = (v) =>
            {
                float newValue;
                if (float.TryParse(v, out newValue)) Value = newValue;
            };
            Value = 0f;
        }
        public BindableFloat(float value)
        {
            _callbackFloat = (v) => { Value = v; };
            _callbackString = (v) =>
            {
                float newValue;
                if (float.TryParse(v, out newValue)) Value = newValue;
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
                OnValueChanged += (value) => { if (inputField) inputField.text = value.ToString(); };
                _bindedControls.Add(control);
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value.ToString();
                OnValueChanged += (value) => { if (text) text.text = value.ToString(); };
                _bindedControls.Add(control);
            }
            else if (control is Slider)
            {
                Slider slider = control as Slider;
                slider.value = Value;
                slider.onValueChanged.AddListener(_callbackFloat);
                OnValueChanged += (value) => { if (slider) slider.value = value; };
                _bindedControls.Add(control);
            }
            else if (control is Scrollbar)
            {
                Scrollbar scrollbar = control as Scrollbar;
                scrollbar.value = Value;
                scrollbar.onValueChanged.AddListener(_callbackFloat);
                OnValueChanged += (value) => { if (scrollbar) scrollbar.value = value; };
                _bindedControls.Add(control);
            }
            else
            {
                Log.Warning($"自动化任务：数据绑定失败，当前不支持控件 {control.GetType().FullName} 与 BindableFloat 类型的数据绑定！");
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
                else if (control is Scrollbar)
                {
                    Scrollbar scrollbar = control as Scrollbar;
                    scrollbar.onValueChanged.RemoveListener(_callbackFloat);
                }
            }
            OnValueChanged = null;
            _bindedControls.Clear();
        }
    }
}