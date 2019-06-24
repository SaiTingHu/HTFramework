using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    public sealed class MainInspector : ModuleEditor
    {
        private Main _target;
        private static bool _showScriptingDefine = false;
        private static bool _showMainData = false;
        private static bool _showLicense = false;

        protected override void OnEnable()
        {
            _target = target as Main;

            ScriptingDefineEnable();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("HTFramework Main Module!", MessageType.Info);
            GUILayout.EndHorizontal();

            ScriptingDefineGUI();
            MainDataGUI();
            LicenseGUI();
        }

        #region ScriptingDefine
        private ScriptingDefine _currentScriptingDefine;
        private bool _isNewDefine = false;
        private string _newDefine = "";

        private void ScriptingDefineEnable()
        {
            _currentScriptingDefine = new ScriptingDefine();
        }
        private void ScriptingDefineGUI()
        {
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showScriptingDefine = EditorGUILayout.Foldout(_showScriptingDefine, "Scripting Define", true);
            GUILayout.EndHorizontal();

            if (_showScriptingDefine)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Defined");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.TextField(_currentScriptingDefine.Defined);
                GUI.enabled = _currentScriptingDefine.IsAnyDefined;
                if (GUILayout.Button("Clear", "Minibutton", GUILayout.Width(40)))
                {
                    _currentScriptingDefine.ClearDefines();
                }
                GUI.enabled = true;
                if (GUILayout.Button("New", "Minibutton", GUILayout.Width(40)))
                {
                    _isNewDefine = !_isNewDefine;
                    _newDefine = "";
                }
                if (GUILayout.Button("Apply", "Minibutton", GUILayout.Width(45)))
                {
                    _currentScriptingDefine.Apply();
                }
                GUILayout.EndHorizontal();

                if (_isNewDefine)
                {
                    GUILayout.BeginHorizontal();
                    _newDefine = EditorGUILayout.TextField(_newDefine);
                    if (GUILayout.Button("OK", "Minibutton", GUILayout.Width(30)))
                    {
                        if (_newDefine != "")
                        {
                            _currentScriptingDefine.AddDefine(_newDefine);
                            _isNewDefine = false;
                            _newDefine = "";
                        }
                        else
                        {
                            GlobalTools.LogError("输入的宏定义不能为空！");
                        }
                    }
                    if (GUILayout.Button("NO", "Minibutton", GUILayout.Width(30)))
                    {
                        _isNewDefine = false;
                        _newDefine = "";
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Historical record");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear record", "Minibutton", GUILayout.Width(80)))
                    {
                        _currentScriptingDefine.ClearDefinesRecord();
                    }
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < _currentScriptingDefine.DefinedsRecord.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        GUILayout.Label(_currentScriptingDefine.DefinedsRecord[i], "PR PrefabLabel");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Use", "Minibutton", GUILayout.Width(30)))
                        {
                            _newDefine += _currentScriptingDefine.DefinedsRecord[i] + ";";
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
        }
        
        private sealed class ScriptingDefine
        {
            private BuildTargetGroup _buildTargetGroup = BuildTargetGroup.Standalone;

            public List<string> DefinedsRecord { get; } = new List<string>();
            public List<string> Defineds { get; } = new List<string>();
            public string DefinedRecord { get; private set; } = "";
            public string Defined { get; private set; } = "";
            public bool IsAnyDefined
            {
                get
                {
                    return Defined != "";
                }
            }

            public ScriptingDefine()
            {
                AddDefineRecord(EditorPrefs.GetString(EditorPrefsTable.ScriptingDefine_Record, ""));

                _buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                AddDefine(PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildTargetGroup));
            }
            public bool IsDefined(string define)
            {
                return Defineds.Contains(define);
            }
            public void AddDefine(string define)
            {
                if (string.IsNullOrEmpty(define) || define == "")
                {
                    return;
                }

                string[] defines = define.Split(';');
                for (int i = 0; i < defines.Length; i++)
                {
                    if (defines[i] != "" && !Defineds.Contains(defines[i]))
                    {
                        Defined += defines[i] + ";";
                        Defineds.Add(defines[i]);
                    }
                }
                AddDefineRecord(define);
            }
            public void ClearDefines()
            {
                Defined = "";
                Defineds.Clear();
            }
            public void Apply()
            {
                _buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, Defined);
            }

            public void ClearDefinesRecord()
            {
                DefinedRecord = "";
                DefinedsRecord.Clear();
                EditorPrefs.SetString(EditorPrefsTable.ScriptingDefine_Record, DefinedRecord);
            }
            private void AddDefineRecord(string define)
            {
                if (string.IsNullOrEmpty(define) || define == "")
                {
                    return;
                }

                string[] defines = define.Split(';');
                for (int i = 0; i < defines.Length; i++)
                {
                    if (defines[i] != "" && !DefinedsRecord.Contains(defines[i]))
                    {
                        DefinedRecord += defines[i] + ";";
                        DefinedsRecord.Add(defines[i]);
                    }
                }
                EditorPrefs.SetString(EditorPrefsTable.ScriptingDefine_Record, DefinedRecord);
            }
        }
        #endregion

        #region MainData
        private void MainDataGUI()
        {
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showMainData = EditorGUILayout.Foldout(_showMainData, "Main Data", true);
            GUILayout.EndHorizontal();

            if (_showMainData)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("MainData");
                if (GUILayout.Button(_target.MainDataType, "MiniPopup"))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                    gm.AddItem(new GUIContent("<None>"), _target.MainDataType == "<None>", () =>
                    {
                        Undo.RecordObject(target, "Set Main Data");
                        _target.MainDataType = "<None>";
                        HasChanged();
                    });
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i].BaseType == typeof(MainData))
                        {
                            int j = i;
                            gm.AddItem(new GUIContent(types[j].FullName), _target.MainDataType == types[j].FullName, () =>
                            {
                                Undo.RecordObject(target, "Set Main Data");
                                _target.MainDataType = types[j].FullName;
                                HasChanged();
                            });
                        }
                    }
                    gm.ShowAsContext();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
        #endregion

        #region License
        private void LicenseGUI()
        {
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showLicense = EditorGUILayout.Foldout(_showLicense, "License", true);
            GUILayout.EndHorizontal();

            if (_showLicense)
            {
                GUILayout.BeginHorizontal();
                Toggle(_target.IsPermanentLicense, out _target.IsPermanentLicense, "Permanent License");
                GUILayout.EndHorizontal();

                if (!_target.IsPermanentLicense)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Prompt:", "BoldLabel");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(_target.EndingPrompt, out _target.EndingPrompt);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ending Time:", "BoldLabel");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Year:", GUILayout.Width(45));
                    int year = EditorGUILayout.IntField(_target.Year, GUILayout.Width(50));
                    if (year != _target.Year)
                    {
                        Undo.RecordObject(target, "Set Year");
                        _target.Year = year;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Year");
                        _target.Year += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Year");
                        _target.Year -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Month:", GUILayout.Width(45));
                    int month = EditorGUILayout.IntField(_target.Month, GUILayout.Width(50));
                    if (month != _target.Month)
                    {
                        Undo.RecordObject(target, "Set Month");
                        _target.Month = month;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Month");
                        _target.Month += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Month");
                        _target.Month -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Day:", GUILayout.Width(45));
                    int day = EditorGUILayout.IntField(_target.Day, GUILayout.Width(50));
                    if (day != _target.Day)
                    {
                        Undo.RecordObject(target, "Set Day");
                        _target.Day = day;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        _target.Day += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        _target.Day -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Now", "MiniButton"))
                    {
                        Undo.RecordObject(target, "Set Now");
                        _target.Year = DateTime.Now.Year;
                        _target.Month = DateTime.Now.Month;
                        _target.Day = DateTime.Now.Day;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("2 Months Later", "MiniButton"))
                    {
                        Undo.RecordObject(target, "Set 2 Months Later");
                        _target.Month += 2;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 纠正时间
        /// </summary>
        private void CorrectDateTime()
        {
            if (_target.Year < DateTime.Now.Year)
            {
                _target.Year = DateTime.Now.Year;
            }
            else if (_target.Year > 9999)
            {
                _target.Year = 9999;
            }

            if (_target.Month < 1)
            {
                _target.Month = 1;
            }
            else if (_target.Month > 12)
            {
                _target.Month = 12;
            }

            if (_target.Month == 2)
            {
                if (_target.Day < 1)
                {
                    _target.Day = 1;
                }
                else if (_target.Day > 28)
                {
                    _target.Day = 28;
                }
            }
            else
            {
                if (_target.Day < 1)
                {
                    _target.Day = 1;
                }
                else if (_target.Day > 30)
                {
                    _target.Day = 30;
                }
            }
        }
        #endregion
    }
}
