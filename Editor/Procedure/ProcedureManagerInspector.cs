using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ProcedureManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86998412")]
    internal sealed class ProcedureManagerInspector : InternalModuleInspector<ProcedureManager>
    {
        private Dictionary<Type, ProcedureBase> _procedureInstances;

        protected override string Intro
        {
            get
            {
                return "Procedure Manager, this is the beginning and the end of everything!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _procedureInstances = Target.GetType().GetField("_procedureInstances", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, ProcedureBase>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target.DefaultProcedure != "";
            GUILayout.Label("Default: " + Target.DefaultProcedure);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = Target.ActivatedProcedures.Count > 0;
            if (GUILayout.Button("Set Default", EditorGlobalTools.Styles.MiniPopup))
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
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Procedures:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.ActivatedProcedures.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + Target.ActivatedProcedures[i]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("▲", EditorStyles.miniButtonLeft))
                {
                    if (i > 0)
                    {
                        Undo.RecordObject(target, "Set Procedure Order");
                        string procedure = Target.ActivatedProcedures[i];
                        Target.ActivatedProcedures.RemoveAt(i);
                        Target.ActivatedProcedures.Insert(i - 1, procedure);
                        HasChanged();
                        continue;
                    }
                }
                if (GUILayout.Button("▼", EditorStyles.miniButtonMid))
                {
                    if (i < Target.ActivatedProcedures.Count - 1)
                    {
                        Undo.RecordObject(target, "Set Procedure Order");
                        string procedure = Target.ActivatedProcedures[i];
                        Target.ActivatedProcedures.RemoveAt(i);
                        Target.ActivatedProcedures.Insert(i + 1, procedure);
                        HasChanged();
                        continue;
                    }
                }
                if (GUILayout.Button("Edit", EditorStyles.miniButtonMid))
                {
                    MonoScriptToolkit.OpenMonoScript(Target.ActivatedProcedures[i]);
                }
                if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
                {
                    Undo.RecordObject(target, "Delete Procedure");

                    if (Target.DefaultProcedure == Target.ActivatedProcedures[i])
                    {
                        Target.DefaultProcedure = "";
                    }

                    Target.ActivatedProcedures.RemoveAt(i);

                    if (Target.DefaultProcedure == "" && Target.ActivatedProcedures.Count > 0)
                    {
                        Target.DefaultProcedure = Target.ActivatedProcedures[0];
                    }

                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Procedure", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(ProcedureBase));
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
                            if (Target.DefaultProcedure == "")
                            {
                                Target.DefaultProcedure = Target.ActivatedProcedures[0];
                            }

                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Procedure: " + Target.CurrentProcedure);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Procedures: " + _procedureInstances.Count);
            GUILayout.EndHorizontal();

            foreach (var procedure in _procedureInstances)
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