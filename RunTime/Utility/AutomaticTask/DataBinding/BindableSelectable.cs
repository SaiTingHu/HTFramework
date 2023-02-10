using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的Selectable类型
    /// </summary>
    public sealed class BindableSelectable : BindableType<int>
    {
        public static implicit operator int(BindableSelectable bSelectable)
        {
            return bSelectable.Value;
        }
        public static implicit operator string(BindableSelectable bSelectable)
        {
            return bSelectable.ValueString;
        }

        private UnityAction<int> _callback;
        private List<string> _values = new List<string>();

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
                int newValue = value;
                if (newValue < 0) newValue = 0;
                else if (newValue >= _values.Count) newValue = _values.Count - 1;

                if (_value == newValue)
                    return;

                _value = newValue;
                OnValueChanged?.Invoke(_value);
            }
        }
        /// <summary>
        /// 数据值（字符串）
        /// </summary>
        public string ValueString
        {
            get
            {
                return (Value >= 0 && Value < _values.Count) ? _values[Value] : null;
            }
        }
        
        public BindableSelectable()
        {
            _callback = (v) => { Value = v; };
            Value = 0;
        }
        public BindableSelectable(string[] values, int value = 0)
        {
            _callback = (v) => { Value = v; };
            for (int i = 0; i < values.Length; i++)
            {
                _values.Add(values[i]);
            }
            Value = value;
        }
        public BindableSelectable(List<string> values, int value = 0)
        {
            _callback = (v) => { Value = v; };
            for (int i = 0; i < values.Count; i++)
            {
                _values.Add(values[i]);
            }
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

            if (control is Dropdown)
            {
                Dropdown dropdown = control as Dropdown;
                if (dropdown.options == null)
                {
                    dropdown.options = new List<Dropdown.OptionData>();
                }
                while (dropdown.options.Count != _values.Count)
                {
                    if (dropdown.options.Count < _values.Count)
                        dropdown.options.Add(new Dropdown.OptionData());
                    else if (dropdown.options.Count > _values.Count)
                        dropdown.options.RemoveAt(0);
                }
                for (int i = 0; i < _values.Count; i++)
                {
                    dropdown.options[i].text = _values[i];
                }
                dropdown.value = Value;
                dropdown.onValueChanged.AddListener(_callback);
                OnValueChanged += (value) => { if (dropdown) dropdown.value = value; };
                _bindedControls.Add(control);
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = ValueString;
                OnValueChanged += (value) => { if (text) text.text = ValueString; };
                _bindedControls.Add(control);
            }
            else
            {
                Log.Warning($"自动化任务：数据绑定失败，当前不支持控件 {control.GetType().FullName} 与 BindableSelectable 类型的数据绑定！");
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

                if (control is Dropdown)
                {
                    Dropdown dropdown = control as Dropdown;
                    dropdown.onValueChanged.RemoveListener(_callback);
                }
            }
            OnValueChanged = null;
            _bindedControls.Clear();
        }
    }
}