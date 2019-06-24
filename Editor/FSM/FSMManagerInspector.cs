using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSMManager))]
    public sealed class FSMManagerInspector : ModuleEditor
    {
        private FSMManager _target;

        private Dictionary<string, FSM> _fsms;

        protected override void OnEnable()
        {
            _target = target as FSMManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _fsms = _target.GetType().GetField("_fsms", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<string, FSM>;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("FSM manager, it manages all state machines!", MessageType.Info);
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("FSMs: " + _fsms.Count);
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<string, FSM> fsm in _fsms)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Name: " + fsm.Key);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Entity: ");
                EditorGUILayout.ObjectField(fsm.Value, typeof(FSM), true);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
