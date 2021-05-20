using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/101541066")]
    internal sealed class EntityManagerInspector : InternalModuleInspector<EntityManager, IEntityHelper>
    {
        private Dictionary<Type, bool> _entityFoldouts;
        private Dictionary<Type, bool> _objectPoolFoldouts;

        protected override string Intro
        {
            get
            {
                return "Entity Manager, Everything is entity except UI, this is a controller of all entity!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _entityFoldouts = new Dictionary<Type, bool>();
            _objectPoolFoldouts = new Dictionary<Type, bool>();
            foreach (var entity in _helper.Entities)
            {
                _entityFoldouts.Add(entity.Key, true);
                _objectPoolFoldouts.Add(entity.Key, true);
            }
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Define Entity:");
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.DefineEntityNames.Count; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(40));
                if (GUILayout.Button(Target.DefineEntityNames[i], EditorGlobalTools.Styles.MiniPopup))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                    {
                        return type.IsSubclassOf(typeof(EntityLogicBase)) && !type.IsAbstract;
                    });
                    for (int m = 0; m < types.Count; m++)
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
                    gm.ShowAsContext();
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    Undo.RecordObject(target, "Delete Define Entity");
                    Target.DefineEntityNames.RemoveAt(i);
                    Target.DefineEntityTargets.RemoveAt(i);
                    HasChanged();
                    continue;
                }
                GUI.backgroundColor = Color.white;
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
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                Undo.RecordObject(target, "New Define Entity");
                Target.DefineEntityNames.Add("<None>");
                Target.DefineEntityTargets.Add(null);
                HasChanged();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper.Entities.Count == 0 && _helper.ObjectPools.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var entity in _helper.Entities)
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
                        if (GUILayout.Button("Show", EditorStyles.miniButtonLeft, GUILayout.Width(40)))
                        {
                            Target.ShowEntity(entity.Value[i]);
                        }
                        GUI.enabled = entity.Value[i].IsShowed;
                        if (GUILayout.Button("Hide", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                        {
                            Target.HideEntity(entity.Value[i]);
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }
                }
            }

            foreach (var pool in _helper.ObjectPools)
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