using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/101541066")]
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

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Define Entity", "BoldLabel");
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.DefineEntityNames.Count; i++)
            {
                GUILayout.BeginVertical("HelpBox");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(40));
                if (GUILayout.Button(Target.DefineEntityNames[i], "MiniPopup"))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                    for (int m = 0; m < types.Count; m++)
                    {
                        if (types[m].IsSubclassOf(typeof(EntityLogicBase)))
                        {
                            int j = i;
                            int n = m;
                            if (Target.DefineEntityNames.Contains(types[n].FullName))
                            {
                                gm.AddDisabledItem(new GUIContent(types[n].FullName));
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(types[n].FullName), Target.DefineEntityNames[j] == types[n].FullName, () =>
                                {
                                    Undo.RecordObject(target, "Set Define Entity Name");
                                    Target.DefineEntityNames[j] = types[n].FullName;
                                    HasChanged();
                                });
                            }
                        }
                    }
                    gm.ShowAsContext();
                }
                if (GUILayout.Button("Delete", "Minibutton", GUILayout.Width(50)))
                {
                    Undo.RecordObject(target, "Delete Define Entity");
                    Target.DefineEntityNames.RemoveAt(i);
                    Target.DefineEntityTargets.RemoveAt(i);
                    HasChanged();
                    continue;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Entity", GUILayout.Width(40));
                GameObject entity = Target.DefineEntityTargets[i];
                ObjectField(Target.DefineEntityTargets[i], out entity, false, "");
                if (entity != Target.DefineEntityTargets[i])
                {
                    Target.DefineEntityTargets[i] = entity;
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New", "Minibutton"))
            {
                Undo.RecordObject(target, "New Define Entity");
                Target.DefineEntityNames.Add("<None>");
                Target.DefineEntityTargets.Add(null);
                HasChanged();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_entities.Count == 0 && _objectPool.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var entity in _entities)
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

            foreach (var pool in _objectPool)
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