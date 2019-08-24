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
        private Dictionary<Type, List<EntityLogicBase>> _entities;
        private Dictionary<Type, Queue<GameObject>> _objectPool;
        private Dictionary<Type, bool> _entityFoldouts;
        private Dictionary<Type, bool> _objectPoolFoldouts;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _entities = Target.GetType().GetField("_entities", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, List<EntityLogicBase>>;
            _objectPool = Target.GetType().GetField("_objectPool", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, Queue<GameObject>>;
            _entityFoldouts = new Dictionary<Type, bool>();
            _objectPoolFoldouts = new Dictionary<Type, bool>();
            foreach (KeyValuePair<Type, List<EntityLogicBase>> entity in _entities)
            {
                _entityFoldouts.Add(entity.Key, true);
                _objectPoolFoldouts.Add(entity.Key, true);
            }
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

            foreach (KeyValuePair<Type, List<EntityLogicBase>> entity in _entities)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                _entityFoldouts[entity.Key] = EditorGUILayout.Foldout(_entityFoldouts[entity.Key], entity.Key.FullName, true);
                GUILayout.EndHorizontal();

                if (_entityFoldouts[entity.Key])
                {
                    for (int i = 0; i < entity.Value.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.ObjectField(entity.Value[i].Entity, typeof(GameObject), true);
                        GUILayout.FlexibleSpace();
                        GUI.enabled = !entity.Value[i].IsShowed;
                        if (GUILayout.Button("Show", "minibuttonleft", GUILayout.Width(40)))
                        {
                            Main.m_Entity.ShowEntity(entity.Value[i]);
                        }
                        GUI.enabled = entity.Value[i].IsShowed;
                        if (GUILayout.Button("Hide", "minibuttonright", GUILayout.Width(40)))
                        {
                            Main.m_Entity.HideEntity(entity.Value[i]);
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }
                }
            }

            foreach (KeyValuePair<Type, Queue<GameObject>> pool in _objectPool)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                _objectPoolFoldouts[pool.Key] = EditorGUILayout.Foldout(_objectPoolFoldouts[pool.Key], pool.Key.FullName + " [Object Pool]", true);
                GUILayout.EndHorizontal();

                if (_objectPoolFoldouts[pool.Key])
                {
                    foreach (GameObject obj in pool.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
