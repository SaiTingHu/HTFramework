using System.Collections.Generic;
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

        private ILocalizeWindow _localizeWindow;
        private string _languagePrefsKey;
        private Language _language = Language.English;
        private Dictionary<string, Word> _localizeWords;

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
        /// <summary>
        /// 是否启用韩语本地化
        /// </summary>
        protected virtual bool IsEnableKorean { get; } = false;
        /// <summary>
        /// 是否启用日语本地化
        /// </summary>
        protected virtual bool IsEnableJapanese { get; } = false;

        protected virtual void OnEnable()
        {
            _adminLoginWindow = this as IAdminLoginWindow;
            _localizeWindow = this as ILocalizeWindow;

            if (_localizeWindow != null)
            {
                _languagePrefsKey = "HT.Framework.HTFEditorWindow.Language." + GetType().FullName;
                _language = (Language)EditorPrefs.GetInt(_languagePrefsKey, 1);
                GenerateWords();
            }
        }
        protected void OnGUI()
        {
            if (IsEnableTitleGUI)
            {
                GUI.backgroundColor = IsAdminMode ? AdminModeColor : Color.white;

                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                if (IsAdminMode)
                {
                    GUILayout.Label("Admin Mode");
                }
                
                OnTitleGUI();

                if (_localizeWindow != null)
                {
                    if (GUILayout.Button("Localize", EditorStyles.toolbarPopup))
                    {
                        GenericMenu gm = new GenericMenu();
                        gm.AddItem(new GUIContent("简体中文"), _language == Language.Chinese, () =>
                        {
                            _language = Language.Chinese;
                            EditorPrefs.SetInt(_languagePrefsKey, (int)_language);
                        });
                        gm.AddItem(new GUIContent("English"), _language == Language.English, () =>
                        {
                            _language = Language.English;
                            EditorPrefs.SetInt(_languagePrefsKey, (int)_language);
                        });
                        if (IsEnableKorean)
                        {
                            gm.AddItem(new GUIContent("한국어"), _language == Language.Korean, () =>
                            {
                                _language = Language.Korean;
                                EditorPrefs.SetInt(_languagePrefsKey, (int)_language);
                            });
                        }
                        else
                        {
                            gm.AddDisabledItem(new GUIContent("한국어"));
                        }
                        if (IsEnableJapanese)
                        {
                            gm.AddItem(new GUIContent("日本語"), _language == Language.Japanese, () =>
                        {
                            _language = Language.Japanese;
                            EditorPrefs.SetInt(_languagePrefsKey, (int)_language);
                        });
                        }
                        else
                        {
                            gm.AddDisabledItem(new GUIContent("日本語"));
                        }
                        gm.ShowAsContext();
                    }
                }
                
                if (_adminLoginWindow != null)
                {
                    if (IsAdminMode)
                    {
                        if (GUILayout.Button("Logout", EditorStyles.toolbarPopup))
                        {
                            IsAdminMode = false;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Admin Login", EditorStyles.toolbarPopup))
                        {
                            AdminLoginWindow.OpenWindow(_adminLoginWindow, OnAdminCheck);
                        }
                    }
                }
                
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;
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
        /// 生成本地化词汇列表
        /// </summary>
        protected virtual void GenerateWords()
        {
            _localizeWords = new Dictionary<string, Word>();
        }
        /// <summary>
        /// 根据key获取对应的本地化词汇
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>本地化词汇</returns>
        protected string GetWord(string key)
        {
            if (_localizeWindow != null)
            {
                if (_localizeWords.ContainsKey(key))
                {
                    Word word = _localizeWords[key];
                    switch (_language)
                    {
                        case Language.Chinese:
                            return word.Chinese;
                        case Language.English:
                            return word.English;
                        case Language.Korean:
                            return word.Korean;
                        case Language.Japanese:
                            return word.Japanese;
                        default:
                            return key;
                    }
                }
                return key;
            }
            else
            {
                return key;
            }
        }
        /// <summary>
        /// 添加本地化词汇，英语默认为key
        /// </summary>
        /// <param name="chinese">简体中文</param>
        /// <param name="english">英语</param>
        /// <param name="korean">韩语</param>
        /// <param name="japanese">日语</param>
        protected void AddWord(string chinese, string english, string korean, string japanese)
        {
            if (!_localizeWords.ContainsKey(english))
            {
                _localizeWords.Add(english, new Word(chinese, english, korean, japanese));
            }
            else
            {
                Log.Error(string.Format("{0} 窗口发现相同Key的本地化词汇：{1}！", GetType().FullName, english));
            }
        }
        /// <summary>
        /// 添加本地化词汇，英语默认为key
        /// </summary>
        /// <param name="chinese">简体中文</param>
        /// <param name="english">英语</param>
        protected void AddWord(string chinese, string english)
        {
            if (!_localizeWords.ContainsKey(english))
            {
                _localizeWords.Add(english, new Word(chinese, english));
            }
            else
            {
                Log.Error(string.Format("{0} 窗口发现相同Key的本地化词汇：{1}！", GetType().FullName, english));
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