using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSMManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86073351")]
    internal sealed class FSMManagerInspector : InternalModuleInspector<FSMManager>
    {
        private Dictionary<string, FSM> _fsms;

        protected override string Intro
        {
            get
            {
                return "FSM manager, it manages all state machines!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _fsms = Target.GetType().GetField("_fsms", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, FSM>;
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("FSMs: " + _fsms.Count);
            GUILayout.EndHorizontal();

            foreach (var fsm in _fsms)
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
        }
    }
}
