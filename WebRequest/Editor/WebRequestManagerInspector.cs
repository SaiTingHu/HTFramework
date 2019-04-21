using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    public sealed class WebRequestManagerInspector : Editor
    {
        private WebRequestManager _target;

        private void OnEnable()
        {
            _target = target as WebRequestManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Web request manager, it manages all Web request! you can submit forms, upload files, download files!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.Toggle(_target.IsOfflineState, out _target.IsOfflineState, "Is OfflineState");
            GUILayout.EndHorizontal();
        }
    }
}
