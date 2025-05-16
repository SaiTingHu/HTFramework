using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 滚动数据列表 - 元素
    /// </summary>
    public class ScrollListElement : HTBehaviour
    {
        private RectTransform _rectTransform;
        private ScrollListData _data;

        /// <summary>
        /// UGUI位置组件
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }
        /// <summary>
        /// 数据
        /// </summary>
        public virtual ScrollListData Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                UpdateData();
            }
        }

        /// <summary>
        /// 更新显示数据
        /// </summary>
        public virtual void UpdateData()
        {

        }
    }
}