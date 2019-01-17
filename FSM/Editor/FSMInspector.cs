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
            _target.Name = EditorGUILayout.TextField(_target.Name);
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
                    _target.Data = "<None>";
                    //挂载此脚本的对象是预制体时，必须设置，否则重新编译后属性会被预制体还原
                    EditorUtility.SetDirty(_target);
                });
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(FSMData))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].Name), _target.Data == types[j].Name, () =>
                        {
                            _target.Data = types[j].Name;
                            //挂载此脚本的对象是预制体时，必须设置，否则重新编译后属性会被预制体还原
                            EditorUtility.SetDirty(_target);
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
                        _target.DefaultState = _target.States[j];
                        _target.DefaultStateName = _target.StateNames[j];
                        //挂载此脚本的对象是预制体时，必须设置，否则重新编译后属性会被预制体还原
                        EditorUtility.SetDirty(_target);
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

                    EditorUtility.SetDirty(_target);
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
                        string stateName = types[j].Name;
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
                        
                        if (_target.States.Contains(types[j].Name))
                        {
                            gm.AddDisabledItem(new GUIContent(stateName));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(stateName), false, () =>
                            {
                                _target.States.Add(types[j].Name);
                                _target.StateNames.Add(stateName);

                                if (_target.DefaultStateName == "")
                                {
                                    _target.DefaultState = _target.States[0];
                                    _target.DefaultStateName = _target.StateNames[0];
                                }

                                EditorUtility.SetDirty(_target);
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
