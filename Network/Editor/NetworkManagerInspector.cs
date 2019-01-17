using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    public sealed class NetworkManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Network Manager, implementing basic network client with socket!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
