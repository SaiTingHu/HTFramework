﻿using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(TaskTarget))]
    internal sealed class TaskTargetInspector : HTFEditor<TaskTarget>
    {
        private HTFAction _generateID;

        protected override bool IsEnableRuntimeData => false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _generateID = GenerateGUID;
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            Button(_generateID, "Generate GUID");
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            PropertyField(nameof(TaskTarget.GUID), "GUID");
            PropertyField(nameof(TaskTarget.State), "State");
            PropertyField(nameof(TaskTarget.CompletingTime), "Completing Time");
            PropertyField(nameof(TaskTarget.OnTaskPointComplete), "On TaskPoint Complete");
            PropertyField(nameof(TaskTarget.OnTaskPointAutoComplete), "On TaskPoint AutoComplete");
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