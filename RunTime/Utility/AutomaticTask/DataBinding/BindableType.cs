using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的数据类型
    /// </summary>
    public abstract class BindableType<T>
    {
        /// <summary>
        /// 数据值改变
        /// </summary>
        public HTFAction<T> OnValueChanged;

        protected T _value;
        protected HashSet<UIBehaviour> _bindedControls = new HashSet<UIBehaviour>();

        /// <summary>
        /// 数据值
        /// </summary>
        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
        
        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="control">绑定的目标控件</param>
        protected virtual void Binding(UIBehaviour control)
        {

        }
        /// <summary>
        /// 解除所有控件的绑定
        /// </summary>
        protected virtual void Unbind()
        {

        }
    }
}