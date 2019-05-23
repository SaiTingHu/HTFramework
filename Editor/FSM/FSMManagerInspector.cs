using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSMManager))]
    public sealed class FSMManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("FSM manager, it manages all state machines!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
