using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EventManager))]
    public sealed class EventManagerInspector : ModuleEditor
    {
        private EventManager _target;

        private Dictionary<Type, HTFAction<object, EventHandler>> _eventHandlerList;

        protected override void OnEnable()
        {
            _target = target as EventManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _eventHandlerList = _target.GetType().GetField("_eventHandlerList", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<Type, HTFAction<object, EventHandler>>;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Event Manager, you can subscribe any events, also custom events!", MessageType.Info);
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

            GUILayout.EndVertical();
        }
    }
}
