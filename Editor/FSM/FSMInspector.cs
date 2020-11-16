using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSM))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86073351")]
    internal sealed class FSMInspector : HTFEditor<FSM>
    {
        private Dictionary<string, Type> _stateInstances;
        private string _currentStateName = "<None>";
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();
            
            Dictionary<Type, FiniteStateBase> states = Target.GetType().GetField("_stateInstances", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, FiniteStateBase>;
            _stateInstances = new Dictionary<string, Type>();
            foreach (var state in states)
            {
                FiniteStateNameAttribute attribute = state.Key.GetCustomAttribute<FiniteStateNameAttribute>();
                _stateInstances.Add(attribute != null ? attribute.Name : state.Key.Name, state.Key);
            }

            if (Target.CurrentState != null)
            {
                FiniteStateNameAttribute nameAttribute = Target.CurrentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                _currentStateName = nameAttribute != null ? nameAttribute.Name : Target.CurrentState.GetType().Name;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Finite state machine!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsAutoRegister, out Target.IsAutoRegister, "Auto Register");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.Name, out Target.Name, "Name");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.Group, out Target.Group, "Group");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.color = Target.Data == "<None>" ? Color.gray : Color.white;
            GUILayout.Label("Data", GUILayout.Width(60));
            if (GUILayout.Button(Target.Data, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("<None>"), Target.Data == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set FSM Data Class");
                    Target.Data = "<None>";
                    HasChanged();
                });
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(FSMDataBase)) && !type.IsAbstract;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(types[j].FullName), Target.Data == types[j].FullName, () =>
                    {
                        Undo.RecordObject(target, "Set FSM Data Class");
                        Target.Data = types[j].FullName;
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target.DefaultStateName != "";
            GUILayout.Label("Default: " + Target.DefaultStateName);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = Target.StateNames.Count > 0;
            if (GUILayout.Button("Set Default", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < Target.StateNames.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(Target.StateNames[j]), Target.DefaultStateName == Target.StateNames[j], () =>
                    {
                        Undo.RecordObject(target, "Set FSM Default State");
                        Target.DefaultState = Target.States[j];
                        Target.DefaultStateName = Target.StateNames[j];
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target.FinalStateName != "";
            GUILayout.Label("Final: " + Target.FinalStateName);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = Target.StateNames.Count > 0;
            if (GUILayout.Button("Set Final", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < Target.StateNames.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(Target.StateNames[j]), Target.FinalStateName == Target.StateNames[j], () =>
                    {
                        Undo.RecordObject(target, "Set FSM Final State");
                        Target.FinalState = Target.States[j];
                        Target.FinalStateName = Target.StateNames[j];
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled State:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.StateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0}.{1}", i + 1, Target.StateNames[i]));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit", EditorStyles.miniButtonLeft))
                {
                    MonoScriptToolkit.OpenMonoScript(Target.States[i]);
                }
                if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
                {
                    Undo.RecordObject(target, "Delete FSM State");

                    if (Target.DefaultStateName == Target.StateNames[i])
                    {
                        Target.DefaultState = "";
                        Target.DefaultStateName = "";
                    }
                    if (Target.FinalStateName == Target.StateNames[i])
                    {
                        Target.FinalState = "";
                        Target.FinalStateName = "";
                    }

                    Target.States.RemoveAt(i);
                    Target.StateNames.RemoveAt(i);

                    if (Target.DefaultStateName == "" && Target.StateNames.Count > 0)
                    {
                        Target.DefaultState = Target.States[0];
                        Target.DefaultStateName = Target.StateNames[0];
                    }
                    if (Target.FinalStateName == "" && Target.StateNames.Count > 0)
                    {
                        Target.FinalState = Target.States[0];
                        Target.FinalStateName = Target.StateNames[0];
                    }

                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add State", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(FiniteStateBase)) && !type.IsAbstract;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    int j = i;
                    string stateName = types[j].FullName;
                    FiniteStateNameAttribute fsmAtt = types[j].GetCustomAttribute<FiniteStateNameAttribute>();
                    if (fsmAtt != null)
                    {
                        stateName = fsmAtt.Name;
                    }

                    if (Target.States.Contains(types[j].FullName))
                    {
                        gm.AddDisabledItem(new GUIContent(stateName));
                    }
                    else
                    {
                        gm.AddItem(new GUIContent(stateName), false, () =>
                        {
                            Undo.RecordObject(target, "Add FSM State");
                            Target.States.Add(types[j].FullName);
                            Target.StateNames.Add(stateName);

                            if (Target.DefaultStateName == "")
                            {
                                Target.DefaultState = Target.States[0];
                                Target.DefaultStateName = Target.StateNames[0];
                            }
                            if (Target.FinalStateName == "")
                            {
                                Target.FinalState = Target.States[0];
                                Target.FinalStateName = Target.StateNames[0];
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
            GUILayout.Label("Current State: " + _currentStateName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("States: " + _stateInstances.Count);
            GUILayout.EndHorizontal();

            foreach (var state in _stateInstances)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(state.Key);
                GUILayout.FlexibleSpace();
                GUI.enabled = _currentStateName != state.Key;
                if (GUILayout.Button("Switch", EditorStyles.miniButton))
                {
                    Target.SwitchState(state.Value);

                    FiniteStateNameAttribute nameAttribute = Target.CurrentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                    _currentStateName = nameAttribute != null ? nameAttribute.Name : Target.CurrentState.GetType().Name;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Renewal", EditorStyles.miniButtonLeft))
            {
                Target.Renewal();

                FiniteStateNameAttribute nameAttribute = Target.CurrentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                _currentStateName = nameAttribute != null ? nameAttribute.Name : Target.CurrentState.GetType().Name;
            }
            if (GUILayout.Button("Final", EditorStyles.miniButtonRight))
            {
                Target.Final();

                FiniteStateNameAttribute nameAttribute = Target.CurrentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                _currentStateName = nameAttribute != null ? nameAttribute.Name : Target.CurrentState.GetType().Name;
            }
            GUILayout.EndHorizontal();
        }
    }
}