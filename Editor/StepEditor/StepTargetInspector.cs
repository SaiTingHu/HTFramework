﻿using UnityEditor;
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
            GUILayout.Label("GUID: ", GUILayout.Width(50));
            GUILayout.Label(_target.GUID);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("State: ", GUILayout.Width(50));
            _target.State = (StepTargetState)EditorGUILayout.EnumPopup(_target.State);
            GUILayout.EndHorizontal();
        }
    }
}