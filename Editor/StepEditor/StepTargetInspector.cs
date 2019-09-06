using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepTarget))]
    public sealed class StepTargetInspector : HTFEditor<StepTarget>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUI.color = Color.cyan;
            if (GUILayout.Button("Generate GUID"))
            {
                if (Target.GUID == "<None>")
                {
                    Target.GUID = Guid.NewGuid().ToString();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to regenerate the GUID？", "Yes", "No"))
                    {
                        Target.GUID = Guid.NewGuid().ToString();
                    }
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GUID: ", GUILayout.Width(50));
            GUI.color = Target.GUID == "<None>" ? Color.red : Color.white;
            EditorGUILayout.TextField(Target.GUID);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("State: ", GUILayout.Width(50));
            Target.State = (StepTargetState)EditorGUILayout.EnumPopup(Target.State);
            GUILayout.EndHorizontal();
        }
    }
}