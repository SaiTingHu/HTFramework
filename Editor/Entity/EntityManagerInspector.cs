using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    public sealed class EntityManagerInspector : ModuleEditor
    {
        private EntityManager _target;

        private Dictionary<Type, List<EntityLogic>> _entities;

        protected override void OnEnable()
        {
            _target = target as EntityManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _entities = _target.GetType().GetField("_entities", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<Type, List<EntityLogic>>;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Entity Manager, Control all EntityLogic!", MessageType.Info);
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
            
            GUILayout.EndVertical();
        }
    }
}
