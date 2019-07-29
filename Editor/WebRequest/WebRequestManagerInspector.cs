using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    public sealed class WebRequestManagerInspector : ModuleEditor
    {
        private WebRequestManager _target;

        protected override void OnEnable()
        {
            _target = target as WebRequestManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Web request manager, it manages all Web request! you can submit forms, upload files, download files!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsOfflineState, out _target.IsOfflineState, "Is OfflineState");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(_target.DownloadAudioType, out _target.DownloadAudioType, "Audio Type");
            GUILayout.EndHorizontal();
        }
    }
}
