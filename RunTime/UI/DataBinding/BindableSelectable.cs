using System.Collections.Generic;
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
                _onValueChanged?.Invoke(_value);
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

        private List<string> _values = new List<string>();

        public BindableSelectable()
        {
            Value = 0;
        }

        public BindableSelectable(string[] values, int value = 0)
        {
            for (int i = 0; i < values.Length; i++)
            {
                _values.Add(values[i]);
            }

            Value = value;
        }

        public BindableSelectable(List<string> values, int value = 0)
        {
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

            if (control is Dropdown)
            {
                Dropdown dropdown = control as Dropdown;
                if (dropdown.options == null)
                {
                    dropdown.options = new List<Dropdown.OptionData>();
                }
                for (int i = 0; i < _values.Count; i++)
                {
                    dropdown.options.Add(new Dropdown.OptionData(_values[i]));
                }
                dropdown.value = Value;
                dropdown.onValueChanged.AddListener((value) => { Value = value; });
                _onValueChanged += (value) => { if (dropdown) dropdown.value = value; };
            }
            else
            {
                Log.Warning(string.Format("数据驱动器：数据绑定失败，当前不支持控件 {0} 与 BindableSelectable 类型的数据绑定！", control.GetType().FullName));
            }
        }
    }
}