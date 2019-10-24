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
            GUI.backgroundColor = Color.cyan;
            Button(GenerateGUID, "Generate GUID");
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.GUID, out Target.GUID, "GUID");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.State, out Target.State, "State");
            GUILayout.EndHorizontal();
        }

        private void GenerateGUID()
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
    }
}