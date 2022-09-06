﻿using UnityEngine.Events;
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

        private UnityAction<string> _callback;

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
                OnValueChanged?.Invoke(_value);
            }
        }

        public BindableString()
        {
            _callback = (v) => { Value = v; };
            Value = null;
        }
        public BindableString(string value)
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

            if (control is InputField)
            {
                InputField inputField = control as InputField;
                inputField.text = Value;
                inputField.onValueChanged.AddListener(_callback);
                OnValueChanged += (value) => { if (inputField) inputField.text = value; };
                _bindedControls.Add(control);
            }
            else if (control is Text)
            {
                Text text = control as Text;
                text.text = Value;
                OnValueChanged += (value) => { if (text) text.text = value; };
                _bindedControls.Add(control);
            }
            else
            {
                Log.Warning(string.Format("自动化任务：数据绑定失败，当前不支持控件 {0} 与 BindableString 类型的数据绑定！", control.GetType().FullName));
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
                    inputField.onValueChanged.RemoveListener(_callback);
                }
            }
            OnValueChanged = null;
            _bindedControls.Clear();
        }
    }
}