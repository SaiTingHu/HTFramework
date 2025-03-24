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
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(ExceptionManager.IsHandler), "Handler");

            if (Target.IsHandler)
            {
                GUI.enabled = false;
#if UNITY_STANDALONE_WIN
                GUI.enabled = true;
#endif
                PropertyField(nameof(ExceptionManager.IsEnableFeedback), "Enable Feedback");
                
                if (Target.IsEnableFeedback)
                {
                    GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Feedback Program Path");
                    GUILayout.EndHorizontal();

                    PropertyField(nameof(ExceptionManager.FeedbackProgramPath), "");
                    
                    GUILayout.EndVertical();
                }

                GUI.enabled = true;

                PropertyField(nameof(ExceptionManager.IsEnableMailReport), "Enable Mail Report");
                
                if (Target.IsEnableMailReport)
                {
                    GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                    PropertyField(nameof(ExceptionManager.Host), "Host");
                    PropertyField(nameof(ExceptionManager.Port), "Port");
                    PropertyField(nameof(ExceptionManager.SendMailbox), "Send Mail");
                    
                    GUILayout.BeginHorizontal();
                    PasswordField(ref Target.SendMailboxPassword, "Password");
                    GUILayout.EndHorizontal();

                    PropertyField(nameof(ExceptionManager.ReceiveMailbox), "Receive Mail");
                    PropertyField(nameof(ExceptionManager.ReportBufferTime), "Buffer Time");
                    
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