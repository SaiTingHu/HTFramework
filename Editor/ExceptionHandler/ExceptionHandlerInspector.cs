using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ExceptionHandler))]
    public sealed class ExceptionHandlerInspector : HTFEditor<ExceptionHandler>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Exception handler, When an exception occurs, he catches it!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsHandler, out Target.IsHandler, "Is Handler");
            GUILayout.EndHorizontal();

            if (Target.IsHandler)
            {
                GUI.enabled = false;
#if UNITY_STANDALONE_WIN
                GUI.enabled = true;
#endif
                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnableFeedback, out Target.IsEnableFeedback, "Is Enable Feedback");
                GUILayout.EndHorizontal();

                if (Target.IsEnableFeedback)
                {
                    GUILayout.BeginVertical("Box");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Feedback Program Path");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.FeedbackProgramPath, out Target.FeedbackProgramPath);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                GUI.enabled = true;

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnableMailReport, out Target.IsEnableMailReport, "Is Enable Mail Report");
                GUILayout.EndHorizontal();

                if (Target.IsEnableMailReport)
                {
                    GUILayout.BeginVertical("Box");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Host", GUILayout.Width(80));
                    TextField(Target.Host, out Target.Host);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Port", GUILayout.Width(80));
                    IntField(Target.Port, out Target.Port);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Send Mail", GUILayout.Width(80));
                    TextField(Target.SendMailbox, out Target.SendMailbox);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Password", GUILayout.Width(80));
                    PasswordField(Target.SendMailboxPassword, out Target.SendMailboxPassword);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Receive Mail", GUILayout.Width(80));
                    TextField(Target.ReceiveMailbox, out Target.ReceiveMailbox);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Buffer Time", GUILayout.Width(80));
                    FloatField(Target.ReportBufferTime, out Target.ReportBufferTime);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }
        }
    }
}
