using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ExceptionHandler))]
    public sealed class ExceptionHandlerInspector : Editor
    {
        private ExceptionHandler _target;

        private void OnEnable()
        {
            _target = target as ExceptionHandler;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Exception handler, When an exception occurs, he catches it!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.Toggle(_target.IsHandler, out _target.IsHandler, "Is Handler");
            GUILayout.EndHorizontal();

            if (_target.IsHandler)
            {
                GUILayout.BeginHorizontal();
                this.Toggle(_target.IsQuitWhenException, out _target.IsQuitWhenException, "Is Quit When Exception");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                this.Toggle(_target.IsReportMailWhenException, out _target.IsReportMailWhenException, "Is Report Mail When Exception");
                GUILayout.EndHorizontal();

                if (_target.IsReportMailWhenException)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Send Mail", GUILayout.Width(80));
                    this.TextField(_target.SendMailbox, out _target.SendMailbox);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Password", GUILayout.Width(80));
                    this.PasswordField(_target.SendMailboxPassword, out _target.SendMailboxPassword);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Receive Mail", GUILayout.Width(80));
                    this.TextField(_target.ReceiveMailbox, out _target.ReceiveMailbox);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
