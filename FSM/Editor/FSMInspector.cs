using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace HT.Framework
{
    [CustomEditor(typeof(FSM))]
    public sealed class FSMInspector : Editor
    {
        private FSM _target;

        private void OnEnable()
        {
            _target = target as FSM;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("State Count:" + _target.StateNames.Count, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name: ");
            this.TextField(_target.Name, out _target.Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Data: ");
            if (GUILayout.Button(_target.Data, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(FSMData));
                Type[] types = assembly.GetTypes();
                gm.AddItem(new GUIContent("<None>"), _target.Data == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set FSM Data Class");
                    _target.Data = "<None>";
                    this.HasChanged();
                });
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(FSMData))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), _target.Data == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set FSM Data Class");
                            _target.Data = types[j].FullName;
                            this.HasChanged();
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
                        this.HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            for (int i = 0; i < _target.StateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + _target.StateNames[i]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", "MiniButton"))
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
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add State", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(FiniteState));
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(FiniteState))
                    {
                        int j = i;
                        string stateName = types[j].FullName;
                        object[] atts = types[j].GetCustomAttributes(false);
                        foreach (object att in atts)
                        {
                            FiniteStateNameAttribute fsmAtt = att as FiniteStateNameAttribute;
                            if (fsmAtt != null)
                            {
                                stateName = fsmAtt.Name;
                                break;
                            }
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
                                this.HasChanged();
                            });
                        }
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }
    }
}
