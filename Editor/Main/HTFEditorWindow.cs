using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public abstract class HTFEditorWindow : EditorWindow
    {
        /// <summary>
        /// 是否启用标题UI
        /// </summary>
        protected virtual bool IsEnableTitleGUI
        {
            get
            {
                return true;
            }
        }
        
        private void OnGUI()
        {
            if (IsEnableTitleGUI)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                OnTitleGUI();
                GUILayout.EndHorizontal();
            }

            OnBodyGUI();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialization()
        {
        }
        /// <summary>
        /// 标题UI
        /// </summary>
        protected virtual void OnTitleGUI()
        {
        }
        /// <summary>
        /// 窗体UI
        /// </summary>
        protected virtual void OnBodyGUI()
        {
        }
    }
}