using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EventManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/85689865")]
    internal sealed class EventManagerInspector : InternalModuleInspector<EventManager>
    {
        private IEventHelper _eventHelper;
        private bool _isShowEvent1 = false;
        private bool _isShowEvent2 = false;
        private bool _isShowEvent3 = false;

        protected override string Intro
        {
            get
            {
                return "Event Manager, you can subscribe any events, also custom events!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IEventHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _eventHelper = _helper as IEventHelper;
        }
        
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _isShowEvent1 = EditorGUILayout.Foldout(_isShowEvent1, "Event List: <object, EventHandlerBase> " + _eventHelper.EventHandlerList1.Count, true);
            GUILayout.EndHorizontal();

            if (_isShowEvent1)
            {
                foreach (var item in _eventHelper.EventHandlerList1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(item.Key.Name);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = item.Value != null;
                    if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        Target.Throw(this, Main.m_ReferencePool.Spawn(item.Key) as EventHandlerBase);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _isShowEvent2 = EditorGUILayout.Foldout(_isShowEvent2, "Event List: <> " + _eventHelper.EventHandlerList2.Count, true);
            GUILayout.EndHorizontal();

            if (_isShowEvent2)
            {
                foreach (var item in _eventHelper.EventHandlerList2)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(item.Key.Name);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = item.Value != null;
                    if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        Target.Throw(item.Key);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _isShowEvent3 = EditorGUILayout.Foldout(_isShowEvent3, "Event List: <EventHandlerBase> " + _eventHelper.EventHandlerList3.Count, true);
            GUILayout.EndHorizontal();

            if (_isShowEvent3)
            {
                foreach (var item in _eventHelper.EventHandlerList3)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(item.Key.Name);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = item.Value != null;
                    if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                    {
                        Target.Throw(Main.m_ReferencePool.Spawn(item.Key) as EventHandlerBase);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}