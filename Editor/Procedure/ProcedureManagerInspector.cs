using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ProcedureManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86998412")]
    internal sealed class ProcedureManagerInspector : InternalModuleInspector<ProcedureManager, IProcedureHelper>
    {
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private GUIContent _defaultGC;
        private GUIContent _editGC;
        private SerializedProperty _procedures;
        private ReorderableList _procedureList;

        protected override string Intro => "Procedure Manager, this is the beginning and the end of everything!";

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();
            
            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new procedure";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select procedure";
            _defaultGC = new GUIContent();
            _defaultGC.image = EditorGUIUtility.IconContent("SceneLoadIn").image;
            _defaultGC.tooltip = "Default procedure";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit procedure script";

            _procedures = GetProperty("ActivatedProcedures");
            _procedureList = new ReorderableList(serializedObject, _procedures, true, true, false, false);
            _procedureList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Enabled Procedures:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
                    {
                        GenericMenu gm = new GenericMenu();
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(typeof(ProcedureBase)) && !type.IsAbstract;
                        });
                        for (int i = 0; i < types.Count; i++)
                        {
                            int j = i;
                            if (Target.ActivatedProcedures.Contains(types[j].FullName))
                            {
                                gm.AddDisabledItem(new GUIContent(types[j].FullName));
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(types[j].FullName), false, () =>
                                {
                                    Undo.RecordObject(target, "Add Procedure");

                                    Target.ActivatedProcedures.Add(types[j].FullName);
                                    if (string.IsNullOrEmpty(Target.DefaultProcedure))
                                    {
                                        Target.DefaultProcedure = Target.ActivatedProcedures[0];
                                    }

                                    HasChanged();
                                });
                            }
                        }
                        gm.ShowAsContext();
                    }

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _procedureList.index >= 0 && _procedureList.index < Target.ActivatedProcedures.Count;
                    if (GUI.Button(sub, _removeGC, "InvisibleButton"))
                    {
                        Undo.RecordObject(target, "Delete Procedure");

                        if (Target.DefaultProcedure == Target.ActivatedProcedures[_procedureList.index])
                        {
                            Target.DefaultProcedure = null;
                        }

                        Target.ActivatedProcedures.RemoveAt(_procedureList.index);

                        if (string.IsNullOrEmpty(Target.DefaultProcedure) && Target.ActivatedProcedures.Count > 0)
                        {
                            Target.DefaultProcedure = Target.ActivatedProcedures[0];
                        }

                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _procedureList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.ActivatedProcedures.Count)
                {
                    Rect subrect = rect;
                    subrect.Set(rect.x, rect.y + 2, rect.width, 16);
                    GUI.Label(subrect, Target.ActivatedProcedures[index]);

                    int size = 20;
                    if (Target.DefaultProcedure == Target.ActivatedProcedures[index])
                    {
                        subrect.Set(rect.x + rect.width - size, rect.y + 2, 20, 16);
                        if (GUI.Button(subrect, _defaultGC, "InvisibleButton"))
                        {
                            GenericMenu gm = new GenericMenu();
                            for (int i = 0; i < Target.ActivatedProcedures.Count; i++)
                            {
                                int j = i;
                                gm.AddItem(new GUIContent(Target.ActivatedProcedures[j]), Target.DefaultProcedure == Target.ActivatedProcedures[j], () =>
                                {
                                    Undo.RecordObject(target, "Set Default Procedure");
                                    Target.DefaultProcedure = Target.ActivatedProcedures[j];
                                    HasChanged();
                                });
                            }
                            gm.ShowAsContext();
                        }
                        size += 20;
                    }
                    if (isActive && isFocused)
                    {
                        subrect.Set(rect.x + rect.width - size, rect.y, 20, 20);
                        if (GUI.Button(subrect, _editGC, "InvisibleButton"))
                        {
                            MonoScriptToolkit.OpenMonoScript(Target.ActivatedProcedures[index]);
                        }
                        size += 20;
                    }
                }
            };
            _procedureList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                    gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                    rect.x += 2;
                    rect.width -= 6;
                    gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            _procedureList.DoLayoutList();

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Procedure: " + Target.CurrentProcedure.GetType().Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Procedures: " + _helper.Procedures.Count);
            GUILayout.EndHorizontal();

            foreach (var procedure in _helper.Procedures)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(procedure.Key.Name);
                GUILayout.FlexibleSpace();
                GUI.enabled = Target.CurrentProcedure != procedure.Value;
                if (GUILayout.Button("Switch", EditorStyles.miniButton))
                {
                    Target.SwitchProcedure(procedure.Key);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}