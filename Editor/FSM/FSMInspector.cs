using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FSM))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86073351")]
    internal sealed class FSMInspector : HTFEditor<FSM>
    {
        private GUIContent _stateGC;
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private GUIContent _defaultGC;
        private GUIContent _finalGC;
        private GUIContent _editGC;
        private SerializedProperty _states;
        private ReorderableList _stateList;
        private Dictionary<string, string> _stateNames;
        private Editor _argsEditor;

        private void OnDestroy()
        {
            if (_argsEditor != null)
            {
                DestroyImmediate(_argsEditor);
                _argsEditor = null;
            }
        }
        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            if (Targets.Length > 1)
                return;

            _stateGC = new GUIContent();
            _stateGC.image = EditorGUIUtility.IconContent("AnimatorState Icon").image;
            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new state";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select state";
            _defaultGC = new GUIContent();
            _defaultGC.image = EditorGUIUtility.IconContent("SceneLoadIn").image;
            _defaultGC.tooltip = "Default state";
            _finalGC = new GUIContent();
            _finalGC.image = EditorGUIUtility.IconContent("SceneLoadOut").image;
            _finalGC.tooltip = "Final state";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit state script";

            _states = GetProperty("States");
            _stateList = new ReorderableList(serializedObject, _states, true, true, false, false);
            _stateList.footerHeight = 0;
            _stateList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Enabled States:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
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
                                gm.AddDisabledItem(new GUIContent(stateName), true);
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(stateName), false, () =>
                                {
                                    Undo.RecordObject(target, "Add FSM State");
                                    Target.States.Add(types[j].FullName);
                                    if (string.IsNullOrEmpty(Target.DefaultState))
                                    {
                                        Target.DefaultState = Target.States[0];
                                    }
                                    if (string.IsNullOrEmpty(Target.FinalState))
                                    {
                                        Target.FinalState = Target.States[0];
                                    }
                                    HasChanged();
                                });
                            }
                        }
                        gm.ShowAsContext();
                    }

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _stateList.index >= 0 && _stateList.index < Target.States.Count;
                    if (GUI.Button(sub, _removeGC, "InvisibleButton"))
                    {
                        Undo.RecordObject(target, "Delete FSM State");

                        if (Target.DefaultState == Target.States[_stateList.index])
                        {
                            Target.DefaultState = null;
                        }
                        if (Target.FinalState == Target.States[_stateList.index])
                        {
                            Target.FinalState = null;
                        }

                        Target.States.RemoveAt(_stateList.index);

                        if (string.IsNullOrEmpty(Target.DefaultState) && Target.States.Count > 0)
                        {
                            Target.DefaultState = Target.States[0];
                        }
                        if (string.IsNullOrEmpty(Target.FinalState) && Target.States.Count > 0)
                        {
                            Target.FinalState = Target.States[0];
                        }

                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _stateList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.States.Count)
                {
                    SerializedProperty property = _states.GetArrayElementAtIndex(index);
                    string stateType = property.stringValue;
                    if (!_stateNames.ContainsKey(stateType))
                    {
                        Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(stateType);
                        if (type != null)
                        {
                            string stateName = type.FullName;
                            FiniteStateNameAttribute fsmAtt = type.GetCustomAttribute<FiniteStateNameAttribute>();
                            if (fsmAtt != null)
                            {
                                stateName = fsmAtt.Name;
                            }
                            _stateNames.Add(stateType, stateName);
                        }
                        else
                        {
                            _stateNames.Add(stateType, "<Missing>");
                        }
                    }

                    Rect subrect = rect;
                    subrect.Set(rect.x, rect.y + 2, rect.width, 16);
                    _stateGC.text = _stateNames[stateType];
                    GUI.color = _stateGC.text != "<Missing>" ? Color.white : Color.red;
                    GUI.Label(subrect, _stateGC);
                    GUI.color = Color.white;

                    int size = 20;
                    if (Target.FinalState == stateType)
                    {
                        subrect.Set(rect.x + rect.width - size, rect.y + 2, 20, 16);
                        if (GUI.Button(subrect, _finalGC, "InvisibleButton"))
                        {
                            GenericMenu gm = new GenericMenu();
                            foreach (var state in _stateNames)
                            {
                                gm.AddItem(new GUIContent(state.Value), Target.FinalState == state.Key, () =>
                                {
                                    Undo.RecordObject(target, "Set FSM Final State");
                                    Target.FinalState = state.Key;
                                    HasChanged();
                                });
                            }
                            gm.ShowAsContext();
                        }
                        size += 20;
                    }
                    if (Target.DefaultState == stateType)
                    {
                        subrect.Set(rect.x + rect.width - size, rect.y + 2, 20, 16);
                        if (GUI.Button(subrect, _defaultGC, "InvisibleButton"))
                        {
                            GenericMenu gm = new GenericMenu();
                            foreach (var state in _stateNames)
                            {
                                gm.AddItem(new GUIContent(state.Value), Target.DefaultState == state.Key, () =>
                                {
                                    Undo.RecordObject(target, "Set FSM Default State");
                                    Target.DefaultState = state.Key;
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
                            CSharpScriptToolkit.OpenScript(stateType);
                        }
                        size += 20;
                    }
                }
            };
            _stateList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
            _stateNames = new Dictionary<string, string>();

            if (Target.Args == null)
            {
                FSMArgsBase args = Target.GetComponent<FSMArgsBase>();
                if (args != null)
                {
                    Target.Args = args;
                    HasChanged();
                }
            }
            if (Target.Args != null)
            {
                Target.Args.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            }
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            if (Targets.Length > 1)
            {
                EditorGUILayout.HelpBox("FSM cannot be multi-edited.", MessageType.None);
                return;
            }

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Finite state machine!", MessageType.Info);
            GUILayout.EndHorizontal();

            PropertyField(nameof(FSM.IsAutoRegister), "Auto Register");
            PropertyField(nameof(FSM.Name), "Name");
            PropertyField(nameof(FSM.Group), "Group");

            _stateList.DoLayoutList();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Data", GUILayout.Width(LabelWidth));
            if (GUILayout.Button(Target.Data, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("<None>"), Target.Data == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set FSM Data Class");
                    Target.Data = "<None>";
                    HasChanged();
                });
                gm.AddItem(new GUIContent("<New Data Script>"), false, () =>
                {
                    EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_FSMData_Folder, "FSMData", "FSMDataTemplate");
                });
                gm.AddSeparator("");
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
            GUILayout.EndHorizontal();
            
            GUI.enabled = true;

            OnArgsGUI();
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Targets.Length > 1)
                return;

            GUILayout.BeginHorizontal();
            string currentStateName = "<None>";
            if (Target.CurrentState != null)
            {
                FiniteStateNameAttribute nameAttribute = Target.CurrentState.GetType().GetCustomAttribute<FiniteStateNameAttribute>();
                currentStateName = nameAttribute != null ? nameAttribute.Name : Target.CurrentState.GetType().FullName;
            }
            GUILayout.Label("Current State: " + currentStateName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("States: " + _stateNames.Count);
            GUILayout.EndHorizontal();

            foreach (var state in _stateNames)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(state.Value);
                GUILayout.FlexibleSpace();
                GUI.enabled = Target.CurrentState.GetType().FullName != state.Key;
                if (GUILayout.Button("Switch", EditorStyles.miniButton))
                {
                    Target.SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(state.Key));
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Renewal", EditorStyles.miniButtonLeft))
            {
                Target.Renewal();
            }
            if (GUILayout.Button("Final", EditorStyles.miniButtonRight))
            {
                Target.Final();
            }
            GUILayout.EndHorizontal();
        }
        private void OnArgsGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Args", GUILayout.Width(LabelWidth));
            string argsName = Target.Args != null ? Target.Args.GetType().FullName : "<None>";
            if (GUILayout.Button(argsName, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("<None>"), Target.Args == null, () =>
                {
                    if (_argsEditor != null)
                    {
                        DestroyImmediate(_argsEditor);
                        _argsEditor = null;
                    }
                    if (Target.Args != null)
                    {
                        Undo.DestroyObjectImmediate(Target.Args);
                        Target.Args = null;
                        HasChanged();
                    }
                });
                gm.AddItem(new GUIContent("<New Args Script>"), false, () =>
                {
                    EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_FSMArgs_Folder, "FSMArgs", "FSMArgsTemplate");
                });
                gm.AddSeparator("");
                Type argsType = Target.Args != null ? Target.Args.GetType() : null;
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(FSMArgsBase)) && !type.IsAbstract;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(types[j].FullName), argsType == types[j], () =>
                    {
                        if (argsType != types[j])
                        {
                            if (_argsEditor != null)
                            {
                                DestroyImmediate(_argsEditor);
                                _argsEditor = null;
                            }
                            if (Target.Args != null)
                            {
                                Undo.DestroyObjectImmediate(Target.Args);
                                Target.Args = null;
                                HasChanged();
                            }

                            Target.Args = Undo.AddComponent(Target.gameObject, types[j]) as FSMArgsBase;
                            Target.Args.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                            HasChanged();
                        }
                    });
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            
            if (Target.Args != null)
            {
                if (_argsEditor == null)
                {
                    _argsEditor = CreateEditor(Target.Args);
                }

                GUILayout.Label("", "RL DragHandle", GUILayout.ExpandWidth(true));

                GUILayout.BeginVertical();
                GUILayout.Space(5);

                _argsEditor.OnInspectorGUI();

                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
        }
    }
}