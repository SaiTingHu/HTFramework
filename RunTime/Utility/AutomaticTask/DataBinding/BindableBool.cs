using UnityEngine.Events;
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

        private UnityAction<bool> _callback;

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
                OnValueChanged?.Invoke(_value);
            }
        }

        public BindableBool()
        {
            _callback = (v) => { Value = v; };
            Value = false;
        }
        public BindableBool(bool value)
        {
            _callback = (v) => { Value = v; };
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

            if (control is Toggle)
            {
                Toggle toggle = control as Toggle;
                toggle.isOn = Value;
                toggle.onValueChanged.AddListener(_callback);
                OnValueChanged += (value) => { if (toggle) toggle.isOn = value; };
                _bindedControls.Add(control);
            }
            else if (control is Button)
            {
                Button button = control as Button;
                button.interactable = Value;
                OnValueChanged += (value) => { if (button) button.interactable = value; };
                _bindedControls.Add(control);
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value.ToString();
                OnValueChanged += (value) => { if (text) text.text = value.ToString(); };
                _bindedControls.Add(control);
            }
            else
            {
                Log.Warning($"自动化任务：数据绑定失败，当前不支持控件 {control.GetType().FullName} 与 BindableBool 类型的数据绑定！");
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

                if (control is Toggle)
                {
                    Toggle toggle = control as Toggle;
                    toggle.onValueChanged.RemoveListener(_callback);
                }
            }
            OnValueChanged = null;
            _bindedControls.Clear();
        }
    }
}