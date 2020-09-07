using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSMManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86073351")]
    internal sealed class FSMManagerInspector : InternalModuleInspector<FSMManager>
    {
        private IFSMHelper _FSMHelper;
        private Dictionary<string, bool> _fsmGroupsShow;

        protected override string Intro
        {
            get
            {
                return "FSM manager, it manages all state machines!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IFSMHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _FSMHelper = _helper as IFSMHelper;
            _fsmGroupsShow = new Dictionary<string, bool>();

            foreach (var fsm in _FSMHelper.FSMGroups)
            {
                _fsmGroupsShow.Add(fsm.Key, false);
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("FSMs: " + _FSMHelper.FSMs.Count);
            GUILayout.EndHorizontal();

            foreach (var fsm in _FSMHelper.FSMGroups)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                _fsmGroupsShow[fsm.Key] = EditorGUILayout.Foldout(_fsmGroupsShow[fsm.Key], fsm.Key, true);
                GUILayout.EndHorizontal();

                if (_fsmGroupsShow[fsm.Key])
                {
                    List<FSM> fsms = fsm.Value;
                    for (int i = 0; i < fsms.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label("Name: " + fsms[i].Name);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label("Entity: ");
                        EditorGUILayout.ObjectField(fsms[i], typeof(FSM), true);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
