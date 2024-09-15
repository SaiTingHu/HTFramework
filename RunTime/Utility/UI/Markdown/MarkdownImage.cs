using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 显示Markdown的Image
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class MarkdownImage : Image
    {
        private bool _isDirty = false;
        private bool _isShow = true;

        /// <summary>
        /// 是否显示图像
        /// </summary>
        public bool IsShow
        {
            get
            {
                return _isShow;
            }
            set
            {
                _isShow = value;
                _isDirty = true;
            }
        }

        private void Update()
        {
            if (_isDirty)
            {
                _isDirty = false;

                Color c = color;
                c.a = IsShow ? 1 : 0;
                color = c;

                raycastTarget = IsShow;
            }
        }
    }
}