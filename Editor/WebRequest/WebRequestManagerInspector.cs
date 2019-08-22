using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    public sealed class WebRequestManagerInspector : HTFEditor<WebRequestManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Web request manager, it manages all Web request! you can submit forms, upload files, download files!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsOfflineState, out Target.IsOfflineState, "Is OfflineState");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.DownloadAudioType, out Target.DownloadAudioType, "Audio Type");
            GUILayout.EndHorizontal();
        }
    }
}
