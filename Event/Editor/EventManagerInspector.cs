using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(EventManager))]
    public sealed class EventManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Event Manager, you can subscribe any events, also custom events!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
