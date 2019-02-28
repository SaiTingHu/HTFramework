using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    public class MainInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("HTFramework Main Module!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
