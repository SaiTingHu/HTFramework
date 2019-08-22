using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EventManager))]
    public sealed class EventManagerInspector : HTFEditor<EventManager>
    {
        private Dictionary<Type, HTFAction<object, EventHandler>> _eventHandlerList;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _eventHandlerList = Target.GetType().GetField("_eventHandlerList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, HTFAction<object, EventHandler>>;
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

            foreach (KeyValuePair<Type, HTFAction<object, EventHandler>> item in _eventHandlerList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(item.Key.Name);
                GUILayout.FlexibleSpace();
                GUI.enabled = item.Value != null;
                if (GUILayout.Button("Throw", "Minibutton", GUILayout.Width(50)))
                {
                    Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn(item.Key) as EventHandler);
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }
        }
    }
}
