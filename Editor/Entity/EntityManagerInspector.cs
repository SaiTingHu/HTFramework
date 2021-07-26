using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/101541066")]
    internal sealed class EntityManagerInspector : InternalModuleInspector<EntityManager, IEntityHelper>
    {
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private ReorderableList _entityList;
        private Dictionary<Type, bool> _entityFoldouts;
        private Dictionary<Type, bool> _objectPoolFoldouts;

        protected override string Intro
        {
            get
            {
                return "Entity Manager, Everything is entity except UI, this is a controller of all entity!";
            }
        }

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new define";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select define";

            _entityList = new ReorderableList(Target.DefineEntityNames, typeof(string), true, true, false, false);
            _entityList.elementHeight = 45;
            _entityList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Define Entity:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
                    {
                        Target.DefineEntityNames.Add("<None>");
                        Target.DefineEntityTargets.Add(null);
                        HasChanged();
                    }

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _entityList.index >= 0 && _entityList.index < Target.DefineEntityNames.Count;
                    if (GUI.Button(sub, _removeGC, "InvisibleButton"))
                    {
                        Target.DefineEntityNames.RemoveAt(_entityList.index);
                        Target.DefineEntityTargets.RemoveAt(_entityList.index);
                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _entityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.DefineEntityNames.Count)
                {
                    Rect subrect = rect;

                    subrect.Set(rect.x, rect.y + 2, 50, 16);
                    GUI.Label(subrect, "Type");
                    if (isActive)
                    {
                        subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                        if (GUI.Button(subrect, Target.DefineEntityNames[index], EditorGlobalTools.Styles.MiniPopup))
                        {
                            GenericMenu gm = new GenericMenu();
                            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                            {
                                return type.IsSubclassOf(typeof(EntityLogicBase)) && !type.IsAbstract;
                            });
                            for (int m = 0; m < types.Count; m++)
                            {
                                int j = index;
                                int n = m;
                                if (Target.DefineEntityNames.Contains(types[n].FullName))
                                {
                                    gm.AddDisabledItem(new GUIContent(types[n].FullName));
                                }
                                else
                                {
                                    gm.AddItem(new GUIContent(types[n].FullName), Target.DefineEntityNames[j] == types[n].FullName, () =>
                                    {
                                        Target.DefineEntityNames[j] = types[n].FullName;
                                        HasChanged();
                                    });
                                }
                            }
                            gm.ShowAsContext();
                        }
                    }
                    else
                    {
                        subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                        GUI.Label(subrect, Target.DefineEntityNames[index]);
                    }

                    subrect.Set(rect.x, rect.y + 22, 50, 16);
                    GUI.Label(subrect, "Entity");
                    subrect.Set(rect.x + 50, rect.y + 22, rect.width - 50, 16);
                    GameObject entity = EditorGUI.ObjectField(subrect, Target.DefineEntityTargets[index], typeof(GameObject), false) as GameObject;
                    if (entity != Target.DefineEntityTargets[index])
                    {
                        Target.DefineEntityTargets[index] = entity;
                        HasChanged();
                    }
                }
            };
            _entityList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "CN EntryBackodd";
                    gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                    rect.x += 2;
                    rect.width -= 6;
                    gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
            _entityList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
            {
                GameObject entity = Target.DefineEntityTargets[oldIndex];
                Target.DefineEntityTargets.RemoveAt(oldIndex);
                Target.DefineEntityTargets.Insert(newIndex, entity);
                HasChanged();
            };
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

            _entityList.DoLayoutList();

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