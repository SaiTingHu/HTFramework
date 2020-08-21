using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    public abstract class HTFEditorWindow : EditorWindow
    {
        private MethodInfo _linkLabel;
        private object[] _linkLabelParameter;

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
        /// <summary>
        /// 超链接文本
        /// </summary>
        /// <param name="label">文本显示内容</param>
        /// <param name="url">指向的超链接</param>
        /// <param name="options">其他可选参数</param>
        protected void LinkLabel(string label, string url, params GUILayoutOption[] options)
        {
            if (_linkLabel == null)
            {
                MethodInfo[] methods = typeof(EditorGUILayout).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var method in methods)
                {
                    if (method.Name == "LinkLabel")
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters != null && parameters.Length > 0 && parameters[0].ParameterType == typeof(string))
                        {
                            _linkLabel = method;
                            break;
                        }
                    }
                }
                _linkLabelParameter = new object[2];
            }

            _linkLabelParameter[0] = label;
            _linkLabelParameter[1] = options;

            bool isClick = (bool)_linkLabel.Invoke(null, _linkLabelParameter);
            if (isClick)
            {
                Application.OpenURL(url);
            }
        }
    }
}