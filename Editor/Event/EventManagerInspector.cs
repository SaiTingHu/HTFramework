using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EventManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/85689865")]
    internal sealed class EventManagerInspector : InternalModuleInspector<EventManager>
    {
        private IEventHelper _eventHelper;
        private Dictionary<Type, HTFAction<object, EventHandlerBase>> _eventHandlerList1;
        private Dictionary<Type, HTFAction> _eventHandlerList2;
        private Dictionary<Type, HTFAction<EventHandlerBase>> _eventHandlerList3;

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

            _eventHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IEventHelper;
            _eventHandlerList1 = _eventHelper.GetType().GetField("_eventHandlerList1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_eventHelper) as Dictionary<Type, HTFAction<object, EventHandlerBase>>;
            _eventHandlerList2 = _eventHelper.GetType().GetField("_eventHandlerList2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_eventHelper) as Dictionary<Type, HTFAction>;
            _eventHandlerList3 = _eventHelper.GetType().GetField("_eventHandlerList3", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_eventHelper) as Dictionary<Type, HTFAction<EventHandlerBase>>;
        }
        
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Event List: <object, EventHandlerBase> " + _eventHandlerList1.Count);
            GUILayout.EndHorizontal();

            foreach (var item in _eventHandlerList1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(item.Key.Name);
                GUILayout.FlexibleSpace();
                GUI.enabled = item.Value != null;
                if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn(item.Key) as EventHandlerBase);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Event List: <> " + _eventHandlerList2.Count);
            GUILayout.EndHorizontal();

            foreach (var item in _eventHandlerList2)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(item.Key.Name);
                GUILayout.FlexibleSpace();
                GUI.enabled = item.Value != null;
                if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    Main.m_Event.Throw(item.Key);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Event List: <EventHandlerBase> " + _eventHandlerList3.Count);
            GUILayout.EndHorizontal();

            foreach (var item in _eventHandlerList3)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(item.Key.Name);
                GUILayout.FlexibleSpace();
                GUI.enabled = item.Value != null;
                if (GUILayout.Button("Throw", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    Main.m_Event.Throw(Main.m_ReferencePool.Spawn(item.Key) as EventHandlerBase);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}