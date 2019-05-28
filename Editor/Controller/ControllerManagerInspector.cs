using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    public sealed class ControllerManagerInspector : ModuleEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Controller Manager, It includes free control, first person control, third person control, etc!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
