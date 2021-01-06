using UnityEngine.EventSystems;

namespace HT.Framework
{
    /// <summary>
    /// 可绑定的数据类型
    /// </summary>
    public abstract class BindableType<T>
    {
        protected T _value;
        protected HTFAction<T> _onValueChanged;

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
                _onValueChanged?.Invoke(_value);
            }
        }
        
        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="control">绑定的目标控件</param>
        protected virtual void Binding(UIBehaviour control)
        {

        }
    }
}