using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ProcedureManager))]
    public sealed class ProcedureManagerInspector : ModuleEditor
    {
        private ProcedureManager _target;
        private Dictionary<string, string> _procedureTypes = new Dictionary<string, string>();

        protected override void OnEnable()
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
                        Undo.RecordObject(target, "Set Default Procedure");
                        _target.DefaultProcedure = _target.ActivatedProcedures[j];
                        HasChanged();
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
                if (GUILayout.Button("▲", "MiniButtonleft"))
                {
                    if (i > 0)
                    {
                        Undo.RecordObject(target, "Set Procedure Order");
                        string procedure = _target.ActivatedProcedures[i];
                        _target.ActivatedProcedures.RemoveAt(i);
                        _target.ActivatedProcedures.Insert(i - 1, procedure);
                        HasChanged();
                        continue;
                    }
                }
                if (GUILayout.Button("▼", "MiniButtonmid"))
                {
                    if (i < _target.ActivatedProcedures.Count - 1)
                    {
                        Undo.RecordObject(target, "Set Procedure Order");
                        string procedure = _target.ActivatedProcedures[i];
                        _target.ActivatedProcedures.RemoveAt(i);
                        _target.ActivatedProcedures.Insert(i + 1, procedure);
                        HasChanged();
                        continue;
                    }
                }
                if (GUILayout.Button("Edit", "MiniButtonmid"))
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
                if (GUILayout.Button("Delete", "minibuttonright"))
                {
                    Undo.RecordObject(target, "Delete Procedure");
                    if (_target.DefaultProcedure == _target.ActivatedProcedures[i])
                    {
                        _target.DefaultProcedure = "";
                    }

                    _target.ActivatedProcedures.RemoveAt(i);

                    if (_target.DefaultProcedure == "" && _target.ActivatedProcedures.Count > 0)
                    {
                        _target.DefaultProcedure = _target.ActivatedProcedures[0];
                    }
                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Procedure", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(Procedure)))
                    {
                        int j = i;
                        if (_target.ActivatedProcedures.Contains(types[j].FullName))
                        {
                            gm.AddDisabledItem(new GUIContent(types[j].FullName));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(types[j].FullName), false, () =>
                            {
                                Undo.RecordObject(target, "Add Procedure");
                                _target.ActivatedProcedures.Add(types[j].FullName);

                                if (_target.DefaultProcedure == "")
                                {
                                    _target.DefaultProcedure = _target.ActivatedProcedures[0];
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
    }
}
