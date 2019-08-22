using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    public sealed class NetworkManagerInspector : HTFEditor<NetworkManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Network Manager, implementing basic network client with socket!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("IP", GUILayout.Width(50));
            TextField(Target.IP, out Target.IP);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Port", GUILayout.Width(50));
            IntField(Target.Port, out Target.Port);
            GUILayout.EndHorizontal();
        }
    }
}
