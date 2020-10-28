using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    public abstract class HTFEditorWindow : EditorWindow
    {
        private IAdminLoginWindow _adminLoginWindow;
        private bool _isAdminMode = false;
        
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
        /// <summary>
        /// 是否是管理员模式
        /// </summary>
        protected bool IsAdminMode
        {
            get
            {
                return _adminLoginWindow != null && _isAdminMode;
            }
            private set
            {
                _isAdminMode = value;
            }
        }
        /// <summary>
        /// 管理员密码
        /// </summary>
        protected virtual string Password { get; } = "1+A/HydBW5UMiL9xsRLN2A==";
        /// <summary>
        /// 管理员模式颜色
        /// </summary>
        protected Color AdminModeColor { get; set; } = new Color(1, 0.43f, 0, 1);

        protected virtual void OnEnable()
        {
            _adminLoginWindow = this as IAdminLoginWindow;
        }
        protected void OnGUI()
        {
            if (IsEnableTitleGUI)
            {
                if (_adminLoginWindow != null)
                {
                    if (IsAdminMode)
                    {
                        GUI.backgroundColor = AdminModeColor;
                        GUILayout.BeginHorizontal(EditorStyles.toolbar);
                        GUILayout.Label("Admin Mode");
                        GUILayout.FlexibleSpace();
                        OnTitleGUI();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Logout", EditorStyles.toolbarPopup))
                        {
                            IsAdminMode = false;
                        }
                        GUILayout.EndHorizontal();
                        GUI.backgroundColor = Color.white;
                    }
                    else
                    {
                        GUILayout.BeginHorizontal(EditorStyles.toolbar);
                        OnTitleGUI();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Admin Login", EditorStyles.toolbarPopup))
                        {
                            AdminLoginWindow.OpenWindow(_adminLoginWindow, OnAdminCheck);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    OnTitleGUI();
                    GUILayout.EndHorizontal();
                }
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
        /// 当执行管理员登录验证
        /// </summary>
        /// <param name="password">输入的密码</param>
        protected virtual void OnAdminCheck(string password)
        {
            IsAdminMode = MathfToolkit.MD5Encrypt(password) == Password;
            GUI.changed = true;

            if (!IsAdminMode)
            {
                ShowNotification(new GUIContent("Password is wrong!"));
            }
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