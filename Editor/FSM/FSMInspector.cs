using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSM))]
    public sealed class FSMInspector : HTFEditor<FSM>
    {
        private Dictionary<string, string> _stateTypes;
        private Dictionary<string, Type> _stateInstances;
        private FiniteStateBase _currentState;
        private string _currentStateName;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _stateTypes = new Dictionary<string, string>();
            string[] states = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].EndsWith(".cs"))
                {
                    string className = states[i].Substring(states[i].LastIndexOf("/") + 1).Replace(".cs", "");
                    if (!_stateTypes.ContainsKey(className))
                    {
                        _stateTypes.Add(className, states[i]);
                    }
                }
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();
            
            Dictionary<Type, FiniteStateBase> states = Target.GetType().GetField("_stateInstances", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, FiniteStateBase>;
            _stateInstances = new Dictionary<string, Type>();
            foreach (KeyValuePair<Type, FiniteStateBase> state in states)
            {
                FiniteStateNameAttribute attribute = state.Key.GetCustomAttribute<FiniteStateNameAttribute>();
                _stateInstances.Add(attribute != null ? attribute.Name : state.Key.Name, state.Key);
            }

            _currentState = Target.GetType().GetField("_currentState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as FiniteStateBase;

            FiniteStateNameAttribute nameAttribute = _currentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
            _currentStateName = nameAttribute != null ? nameAttribute.Name : _currentState.GetType().Name;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("State Count:" + Target.StateNames.Count, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsAutoRegister, out Target.IsAutoRegister, "Auto Register");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ");
            TextField(Target.Name, out Target.Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Data: ");
            if (GUILayout.Button(Target.Data, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                gm.AddItem(new GUIContent("<None>"), Target.Data == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set FSM Data Class");
                    Target.Data = "<None>";
                    HasChanged();
                });
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(FSMDataBase)))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.Data == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set FSM Data Class");
                            Target.Data = types[j].FullName;
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target.DefaultStateName != "";
            GUILayout.Label("Default: " + Target.DefaultStateName);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = Target.StateNames.Count > 0;
            if (GUILayout.Button("Set Default", "MiniPopup"))
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

            for (int i = 0; i < Target.StateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0}.{1}", i + 1, Target.StateNames[i]));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit", "minibuttonleft"))
                {
                    if (_stateTypes.ContainsKey(Target.States[i]))
                    {
                        UnityEngine.Object classFile = AssetDatabase.LoadAssetAtPath(_stateTypes[Target.States[i]], typeof(TextAsset));
                        if (classFile)
                            AssetDatabase.OpenAsset(classFile);
                        else
                            GlobalTools.LogError("没有找到 " + Target.States[i] + " 脚本文件！");
                    }
                    else
                    {
                        GlobalTools.LogError("没有找到 " + Target.States[i] + " 脚本文件！");
                    }
                }
                if (GUILayout.Button("Delete", "minibuttonright"))
                {
                    Undo.RecordObject(target, "Delete FSM State");
                    if (Target.DefaultStateName == Target.StateNames[i])
                    {
                        Target.DefaultState = "";
                        Target.DefaultStateName = "";
                    }

                    Target.States.RemoveAt(i);
                    Target.StateNames.RemoveAt(i);

                    if (Target.DefaultStateName == "" && Target.StateNames.Count > 0)
                    {
                        Target.DefaultState = Target.States[0];
                        Target.DefaultStateName = Target.StateNames[0];
                    }
                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add State", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(FiniteStateBase)))
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
                                HasChanged();
                            });
                        }
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Current State: " + _currentStateName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("States: ");
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<string, Type> state in _stateInstances)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(state.Key);
                GUILayout.FlexibleSpace();
                GUI.enabled = _currentStateName != state.Key;
                if (GUILayout.Button("Switch", "Minibutton"))
                {
                    Target.SwitchState(state.Value);

                    _currentState = Target.GetType().GetField("_currentState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as FiniteStateBase;

                    FiniteStateNameAttribute nameAttribute = _currentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                    _currentStateName = nameAttribute != null ? nameAttribute.Name : _currentState.GetType().Name;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}
