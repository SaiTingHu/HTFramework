using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ProcedureManager))]
    public sealed class ProcedureManagerInspector : Editor
    {
        private ProcedureManager _target;
        private Dictionary<string, string> _procedureTypes = new Dictionary<string, string>();

        private void OnEnable()
        {
            _target = target as ProcedureManager;

            _procedureTypes.Clear();
            string[] typePaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < typePaths.Length; i++)
            {
                if (typePaths[i].EndsWith(".cs"))
                {
                    string className = typePaths[i].Substring(typePaths[i].LastIndexOf("/") + 1).Replace(".cs", "");
                    if (!_procedureTypes.ContainsKey(className))
                    {
                        _procedureTypes.Add(className, typePaths[i]);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Activated Procedure Count:" + _target.ActivatedProcedures.Count, MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _target.DefaultProcedure != "";
            GUILayout.Label("Default: " + _target.DefaultProcedure);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.enabled = _target.ActivatedProcedures.Count > 0;
            if (GUILayout.Button("Set Default", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _target.ActivatedProcedures.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(_target.ActivatedProcedures[j]), _target.DefaultProcedure == _target.ActivatedProcedures[j], () =>
                    {
                        _target.DefaultProcedure = _target.ActivatedProcedures[j];
                        //挂载此脚本的对象是预制体时，必须设置，否则重新编译后属性会被预制体还原
                        EditorUtility.SetDirty(_target);
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            for (int i = 0; i < _target.ActivatedProcedures.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + _target.ActivatedProcedures[i]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Edit", "MiniButton"))
                {
                    if (_procedureTypes.ContainsKey(_target.ActivatedProcedures[i]))
                    {
                        UnityEngine.Object classFile = AssetDatabase.LoadAssetAtPath(_procedureTypes[_target.ActivatedProcedures[i]], typeof(TextAsset));
                        if (classFile)
                            AssetDatabase.OpenAsset(classFile);
                        else
                            GlobalTools.LogError("没有找到 " + _target.ActivatedProcedures[i] + " 脚本文件！");
                    }
                    else
                    {
                        GlobalTools.LogError("没有找到 " + _target.ActivatedProcedures[i] + " 脚本文件！");
                    }
                }
                if (GUILayout.Button("Delete", "MiniButton"))
                {
                    if (_target.DefaultProcedure == _target.ActivatedProcedures[i])
                    {
                        _target.DefaultProcedure = "";
                    }

                    _target.ActivatedProcedures.RemoveAt(i);

                    if (_target.DefaultProcedure == "" && _target.ActivatedProcedures.Count > 0)
                    {
                        _target.DefaultProcedure = _target.ActivatedProcedures[0];
                    }

                    EditorUtility.SetDirty(_target);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Procedure", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(Procedure));
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(Procedure))
                    {
                        int j = i;
                        if (_target.ActivatedProcedures.Contains(types[j].Name))
                        {
                            gm.AddDisabledItem(new GUIContent(types[j].Name));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(types[j].Name), false, () =>
                            {
                                _target.ActivatedProcedures.Add(types[j].Name);

                                if (_target.DefaultProcedure == "")
                                {
                                    _target.DefaultProcedure = _target.ActivatedProcedures[0];
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
