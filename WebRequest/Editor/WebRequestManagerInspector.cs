using UnityEngine;
using UnityEditor;

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
            GUI.color = _target.IsOfflineState ? Color.white : Color.gray;
            _target.IsOfflineState = GUILayout.Toggle(_target.IsOfflineState, "Is OfflineState");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
