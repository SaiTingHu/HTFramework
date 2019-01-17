using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    public sealed class WebRequestManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Web request manager, it manages all Web request! you can submit forms, upload files, download files!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
