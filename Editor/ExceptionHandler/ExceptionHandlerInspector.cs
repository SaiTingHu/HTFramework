using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ExceptionHandler))]
    public sealed class ExceptionHandlerInspector : ModuleEditor
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
            Toggle(_target.IsHandler, out _target.IsHandler, "Is Handler");
            GUILayout.EndHorizontal();

            if (_target.IsHandler)
            {
                GUI.enabled = false;
#if UNITY_STANDALONE_WIN
                GUI.enabled = true;
#endif
                GUILayout.BeginHorizontal();
                Toggle(_target.IsEnableFeedback, out _target.IsEnableFeedback, "Is Enable Feedback");
                GUILayout.EndHorizontal();

                if (_target.IsEnableFeedback)
                {
                    GUILayout.BeginVertical("Box");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Feedback Program Path");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(_target.FeedbackProgramPath, out _target.FeedbackProgramPath);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                GUI.enabled = true;

                GUILayout.BeginHorizontal();
                Toggle(_target.IsEnableMailReport, out _target.IsEnableMailReport, "Is Enable Mail Report");
                GUILayout.EndHorizontal();

                if (_target.IsEnableMailReport)
                {
                    GUILayout.BeginVertical("Box");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Host", GUILayout.Width(80));
                    TextField(_target.Host, out _target.Host);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Port", GUILayout.Width(80));
                    IntField(_target.Port, out _target.Port);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Send Mail", GUILayout.Width(80));
                    TextField(_target.SendMailbox, out _target.SendMailbox);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Password", GUILayout.Width(80));
                    PasswordField(_target.SendMailboxPassword, out _target.SendMailboxPassword);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Receive Mail", GUILayout.Width(80));
                    TextField(_target.ReceiveMailbox, out _target.ReceiveMailbox);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Buffer Time", GUILayout.Width(80));
                    FloatField(_target.ReportBufferTime, out _target.ReportBufferTime);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }
        }
    }
}
