using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(FSM))]
    public sealed class FSMInspector : ModuleEditor
    {
        private FSM _target;
        private Dictionary<string, string> _stateTypes;

        private Dictionary<string, Type> _stateInstances;
        private FiniteState _currentState;
        private string _currentStateName;

        protected override void OnEnable()
        {
            _target = target as FSM;

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

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();
            
            Dictionary<Type, FiniteState> states = _target.GetType().GetField("_stateInstances", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<Type, FiniteState>;
            _stateInstances = new Dictionary<string, Type>();
            foreach (KeyValuePair<Type, FiniteState> state in states)
            {
                FiniteStateNameAttribute attribute = state.Key.GetCustomAttribute<FiniteStateNameAttribute>();
                _stateInstances.Add(attribute != null ? attribute.Name : state.Key.Name, state.Key);
            }

            _currentState = _target.GetType().GetField("_currentState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as FiniteState;

            FiniteStateNameAttribute nameAttribute = _currentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
            _currentStateName = nameAttribute != null ? nameAttribute.Name : _currentState.GetType().Name;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("State Count:" + _target.StateNames.Count, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsAutoRegister, out _target.IsAutoRegister, "Auto Register");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ");
            TextField(_target.Name, out _target.Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Data: ");
            if (GUILayout.Button(_target.Data, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                gm.AddItem(new GUIContent("<None>"), _target.Data == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set FSM Data Class");
                    _target.Data = "<None>";
                    HasChanged();
                });
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(FSMData)))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), _target.Data == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set FSM Data Class");
                            _target.Data = types[j].FullName;
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _target.DefaultStateName != "";
            GUILayout.Label("Default: " + _target.DefaultStateName);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = _target.StateNames.Count > 0;
            if (GUILayout.Button("Set Default", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _target.StateNames.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(_target.StateNames[j]), _target.DefaultStateName == _target.StateNames[j], () =>
                    {
                        Undo.RecordObject(target, "Set FSM Default State");
                        _target.DefaultState = _target.States[j];
                        _target.DefaultStateName = _target.StateNames[j];
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            for (int i = 0; i < _target.StateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0}.{1}", i + 1, _target.StateNames[i]));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit", "minibuttonleft"))
                {
                    if (_stateTypes.ContainsKey(_target.States[i]))
                    {
                        UnityEngine.Object classFile = AssetDatabase.LoadAssetAtPath(_stateTypes[_target.States[i]], typeof(TextAsset));
                        if (classFile)
                            AssetDatabase.OpenAsset(classFile);
                        else
                            GlobalTools.LogError("没有找到 " + _target.States[i] + " 脚本文件！");
                    }
                    else
                    {
                        GlobalTools.LogError("没有找到 " + _target.States[i] + " 脚本文件！");
                    }
                }
                if (GUILayout.Button("Delete", "minibuttonright"))
                {
                    Undo.RecordObject(target, "Delete FSM State");
                    if (_target.DefaultStateName == _target.StateNames[i])
                    {
                        _target.DefaultState = "";
                        _target.DefaultStateName = "";
                    }

                    _target.States.RemoveAt(i);
                    _target.StateNames.RemoveAt(i);

                    if (_target.DefaultStateName == "" && _target.StateNames.Count > 0)
                    {
                        _target.DefaultState = _target.States[0];
                        _target.DefaultStateName = _target.StateNames[0];
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
                    if (types[i].IsSubclassOf(typeof(FiniteState)))
                    {
                        int j = i;
                        string stateName = types[j].FullName;
                        FiniteStateNameAttribute fsmAtt = types[j].GetCustomAttribute<FiniteStateNameAttribute>();
                        if (fsmAtt != null)
                        {
                            stateName = fsmAtt.Name;
                        }

                        if (_target.States.Contains(types[j].FullName))
                        {
                            gm.AddDisabledItem(new GUIContent(stateName));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(stateName), false, () =>
                            {
                                Undo.RecordObject(target, "Add FSM State");
                                _target.States.Add(types[j].FullName);
                                _target.StateNames.Add(stateName);

                                if (_target.DefaultStateName == "")
                                {
                                    _target.DefaultState = _target.States[0];
                                    _target.DefaultStateName = _target.StateNames[0];
                                }
                                HasChanged();
                            });
                        }
                    }
                }
                gm.ShowAsContext();
            }
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
                    _target.SwitchState(state.Value);

                    _currentState = _target.GetType().GetField("_currentState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as FiniteState;

                    FiniteStateNameAttribute nameAttribute = _currentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                    _currentStateName = nameAttribute != null ? nameAttribute.Name : _currentState.GetType().Name;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
