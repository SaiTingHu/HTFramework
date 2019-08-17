using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepTarget))]
    public sealed class StepTargetInspector : ModuleEditor
    {
        private StepTarget _target;

        protected override void OnEnable()
        {
            _target = target as StepTarget;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate GUID"))
            {
                if (_target.GUID == "<None>")
                {
                    _target.GUID = Guid.NewGuid().ToString();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to regenerate the GUID？", "Yes", "No"))
                    {
                        _target.GUID = Guid.NewGuid().ToString();
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GUID: ", GUILayout.Width(50));
            GUI.color = _target.GUID == "<None>" ? Color.red : Color.white;
            EditorGUILayout.TextField(_target.GUID);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("State: ", GUILayout.Width(50));
            _target.State = (StepTargetState)EditorGUILayout.EnumPopup(_target.State);
            GUILayout.EndHorizontal();
        }
    }
}
