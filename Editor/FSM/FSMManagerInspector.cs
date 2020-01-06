using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSMManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86073351")]
    internal sealed class FSMManagerInspector : HTFEditor<FSMManager>
    {
        private Dictionary<string, FSM> _fsms;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _fsms = Target.GetType().GetField("_fsms", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, FSM>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("FSM manager, it manages all state machines!", MessageType.Info);
            GUILayout.EndHorizontal();
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
