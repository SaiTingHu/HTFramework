using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    public sealed class NetworkManagerInspector : Editor
    {
        private NetworkManager _target;

        private void OnEnable()
        {
            _target = target as NetworkManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Network Manager, implementing basic network client with socket!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("IP", GUILayout.Width(50));
            _target.IP = EditorGUILayout.TextField(_target.IP);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Port", GUILayout.Width(50));
            _target.Port = EditorGUILayout.IntField(_target.Port);
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
