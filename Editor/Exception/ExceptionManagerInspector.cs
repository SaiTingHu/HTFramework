using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ExceptionManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/102894933")]
    internal sealed class ExceptionManagerInspector : InternalModuleInspector<ExceptionManager, IExceptionHelper>
    {
        protected override string Intro
        {
            get
            {
                return "Exception Manager, when any exception occurs, he catches it!";
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsHandler, out Target.IsHandler, "Handler");
            GUILayout.EndHorizontal();

            if (Target.IsHandler)
            {
                GUI.enabled = false;
#if UNITY_STANDALONE_WIN
                GUI.enabled = true;
#endif
                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnableFeedback, out Target.IsEnableFeedback, "Enable Feedback");
                GUILayout.EndHorizontal();

                if (Target.IsEnableFeedback)
                {
                    GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Feedback Program Path");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.FeedbackProgramPath, out Target.FeedbackProgramPath, "");
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                GUI.enabled = true;

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnableMailReport, out Target.IsEnableMailReport, "Enable Mail Report");
                GUILayout.EndHorizontal();

                if (Target.IsEnableMailReport)
                {
                    GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                    GUILayout.BeginHorizontal();
                    TextField(Target.Host, out Target.Host, "Host");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    IntField(Target.Port, out Target.Port, "Port");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.SendMailbox, out Target.SendMailbox, "Send Mail");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    PasswordField(Target.SendMailboxPassword, out Target.SendMailboxPassword, "Password");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.ReceiveMailbox, out Target.ReceiveMailbox, "Receive Mail");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    FloatField(Target.ReportBufferTime, out Target.ReportBufferTime, "Buffer Time");
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}