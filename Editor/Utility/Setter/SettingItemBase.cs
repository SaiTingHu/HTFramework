using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 设置项基类
    /// </summary>
    public abstract class SettingItemBase
    {
        /// <summary>
        /// 设置面板的显示名称
        /// </summary>
        public virtual string Name
        {
            get
            {
                return GetType().FullName;
            }
        }
        /// <summary>
        /// 开始设置
        /// </summary>
        public virtual void OnBeginSetting()
        { }
        /// <summary>
        /// 设置面板UI
        /// </summary>
        public virtual void OnSettingGUI()
        { }
        /// <summary>
        /// 结束设置
        /// </summary>
        public virtual void OnEndSetting()
        { }
        /// <summary>
        /// 重置所有设置
        /// </summary>
        public virtual void OnReset()
        { }
        /// <summary>
        /// 标记目标已改变
        /// </summary>
        protected void HasChanged(Object target)
        {
            if (!EditorApplication.isPlaying && target != null)
            {
                EditorUtility.SetDirty(target);
                Component component = target as Component;
                if (component != null && component.gameObject.scene != null)
                {
                    EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                }
            }
        }
    }
}