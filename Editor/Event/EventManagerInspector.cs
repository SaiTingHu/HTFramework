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
    public sealed class EventManagerInspector : HTFEditor<EventManager>
    {
        private Dictionary<Type, HTFAction<object, EventHandlerBase>> _eventHandlerList;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _eventHandlerList = Target.GetType().GetField("_eventHandlerList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, HTFAction<object, EventHandlerBase>>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Event Manager, you can subscribe any events, also custom events!", MessageType.Info);
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Event List: " + _eventHandlerList.Count);
            GUILayout.EndHorizontal();

            foreach (var item in _eventHandlerList)
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
        }
    }
}