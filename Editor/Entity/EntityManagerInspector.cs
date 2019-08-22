using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    public sealed class EntityManagerInspector : HTFEditor<EntityManager>
    {
        private Dictionary<Type, List<EntityLogic>> _entities;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _entities = Target.GetType().GetField("_entities", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, List<EntityLogic>>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Entity Manager, Control all EntityLogic!", MessageType.Info);
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            foreach (KeyValuePair<Type, List<EntityLogic>> entity in _entities)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(entity.Key.FullName + ":");
                GUILayout.EndHorizontal();

                for (int i = 0; i < entity.Value.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    EditorGUILayout.ObjectField(entity.Value[i].Entity, typeof(GameObject), true);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = !entity.Value[i].IsShowed;
                    if (GUILayout.Button("Show", "minibuttonleft"))
                    {
                        Main.m_Entity.ShowEntity(entity.Value[i]);
                    }
                    GUI.enabled = entity.Value[i].IsShowed;
                    if (GUILayout.Button("Hide", "minibuttonright"))
                    {
                        Main.m_Entity.HideEntity(entity.Value[i]);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
