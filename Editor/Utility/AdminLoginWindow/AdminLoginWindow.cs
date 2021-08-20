using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 管理员身份登录窗口
    /// </summary>
    internal sealed class AdminLoginWindow : HTFEditorWindow
    {
        public static void OpenWindow(IAdminLoginWindow parent, HTFAction<string> checkAction)
        {
            AdminLoginWindow window = GetWindow<AdminLoginWindow>();
            window._parent = parent;
            window._checkAction = checkAction;
            window.titleContent.text = "Admin Login";
            window.position = new Rect(parent.Cast<HTFEditorWindow>().position.center - new Vector2(125, 0), new Vector2(250, 50));
            window.minSize = new Vector2(250, 50);
            window.maxSize = new Vector2(250, 50);
            window.Show();
        }

        private IAdminLoginWindow _parent;
        private HTFAction<string> _checkAction;
        private string _password = "";

        protected override bool IsEnableTitleGUI => false;

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            EventHandle();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Password:");
            _password = EditorGUILayout.PasswordField(_password, GUILayout.Width(100));
            GUI.enabled = _password != "";
            if (GUILayout.Button("Login", EditorStyles.miniButton))
            {
                _checkAction?.Invoke(_password);
                Close();
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        private void EventHandle()
        {
            if (Event.current == null)
            {
                return;
            }

            switch (Event.current.rawType)
            {
                case EventType.KeyDown:
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                            if (_password != "")
                            {
                                _checkAction?.Invoke(_password);
                                Close();
                            }
                            break;
                    }
                    break;
            }
        }
        private void Update()
        {
            if (_parent == null)
            {
                Close();
            }
        }
    }
}