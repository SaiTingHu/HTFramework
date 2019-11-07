using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/102956756")]
    public sealed class MainInspector : HTFEditor<Main>
    {
        private static bool _showScriptingDefine = false;
        private static bool _showMainData = false;
        private static bool _showLicense = false;
        private static bool _showParameter = false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            ScriptingDefineEnable();
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("HTFramework Main Module!", MessageType.Info);
            GUILayout.EndHorizontal();

            ScriptingDefineGUI();
            MainDataGUI();
            LicenseGUI();
            ParameterGUI();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
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
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUI.enabled = _currentScriptingDefine.IsAnyDefined;
                if (GUILayout.Button("Clear", "Minibuttonleft"))
                {
                    _currentScriptingDefine.ClearDefines();
                }
                GUI.enabled = true;
                if (GUILayout.Button("New", "MiniButtonmid"))
                {
                    _isNewDefine = !_isNewDefine;
                    _newDefine = "";
                }
                if (GUILayout.Button("Apply", "Minibuttonright"))
                {
                    _currentScriptingDefine.Apply();
                }
                GUILayout.EndHorizontal();

                if (_isNewDefine)
                {
                    GUILayout.BeginHorizontal();
                    _newDefine = EditorGUILayout.TextField(_newDefine);
                    if (GUILayout.Button("OK", "Minibuttonleft", GUILayout.Width(30)))
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
                    if (GUILayout.Button("NO", "Minibuttonright", GUILayout.Width(30)))
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
                if (GUILayout.Button(Target.MainDataType, "MiniPopup"))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                    gm.AddItem(new GUIContent("<None>"), Target.MainDataType == "<None>", () =>
                    {
                        Undo.RecordObject(target, "Set Main Data");
                        Target.MainDataType = "<None>";
                        HasChanged();
                    });
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i].IsSubclassOf(typeof(MainDataBase)))
                        {
                            int j = i;
                            gm.AddItem(new GUIContent(types[j].FullName), Target.MainDataType == types[j].FullName, () =>
                            {
                                Undo.RecordObject(target, "Set Main Data");
                                Target.MainDataType = types[j].FullName;
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
                Toggle(Target.IsPermanentLicense, out Target.IsPermanentLicense, "Permanent License");
                GUILayout.EndHorizontal();

                if (!Target.IsPermanentLicense)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Prompt:", "BoldLabel");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.EndingPrompt, out Target.EndingPrompt, "");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ending Time:", "BoldLabel");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Year:", GUILayout.Width(45));
                    int year = EditorGUILayout.IntField(Target.Year, GUILayout.Width(50));
                    if (year != Target.Year)
                    {
                        Undo.RecordObject(target, "Set Year");
                        Target.Year = year;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Year");
                        Target.Year += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Year");
                        Target.Year -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Month:", GUILayout.Width(45));
                    int month = EditorGUILayout.IntField(Target.Month, GUILayout.Width(50));
                    if (month != Target.Month)
                    {
                        Undo.RecordObject(target, "Set Month");
                        Target.Month = month;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Month");
                        Target.Month += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Month");
                        Target.Month -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Day:", GUILayout.Width(45));
                    int day = EditorGUILayout.IntField(Target.Day, GUILayout.Width(50));
                    if (day != Target.Day)
                    {
                        Undo.RecordObject(target, "Set Day");
                        Target.Day = day;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        Target.Day += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        Target.Day -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Now", "MiniButtonLeft"))
                    {
                        Undo.RecordObject(target, "Set Now");
                        Target.Year = DateTime.Now.Year;
                        Target.Month = DateTime.Now.Month;
                        Target.Day = DateTime.Now.Day;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("2 Months Later", "MiniButtonRight"))
                    {
                        Undo.RecordObject(target, "Set 2 Months Later");
                        Target.Month += 2;
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
            if (Target.Year < DateTime.Now.Year)
            {
                Target.Year = DateTime.Now.Year;
            }
            else if (Target.Year > 9999)
            {
                Target.Year = 9999;
            }

            if (Target.Month < 1)
            {
                Target.Month = 1;
            }
            else if (Target.Month > 12)
            {
                Target.Month = 12;
            }

            if (Target.Month == 2)
            {
                if (Target.Day < 1)
                {
                    Target.Day = 1;
                }
                else if (Target.Day > 28)
                {
                    Target.Day = 28;
                }
            }
            else
            {
                if (Target.Day < 1)
                {
                    Target.Day = 1;
                }
                else if (Target.Day > 30)
                {
                    Target.Day = 30;
                }
            }
        }
        #endregion

        #region Parameter
        private void ParameterGUI()
        {
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showParameter = EditorGUILayout.Foldout(_showParameter, "Parameter", true);
            GUILayout.EndHorizontal();

            if (_showParameter)
            {
                for (int i = 0; i < Target.MainParameters.Count; i++)
                {
                    MainParameter mainParameter = Target.MainParameters[i];

                    GUILayout.BeginVertical("HelpBox");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Type", GUILayout.Width(40));
                    EnumPopup(mainParameter.Type, out mainParameter.Type, "");
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Delete", "Minibutton", GUILayout.Width(50)))
                    {
                        Undo.RecordObject(target, "Delete Main Parameter");
                        Target.MainParameters.RemoveAt(i);
                        HasChanged();
                        continue;
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name:", GUILayout.Width(40));
                    TextField(mainParameter.Name, out mainParameter.Name, "");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Value:", GUILayout.Width(40));
                    switch (mainParameter.Type)
                    {
                        case MainParameter.ParameterType.String:
                            TextField(mainParameter.StringValue, out mainParameter.StringValue, "");
                            break;
                        case MainParameter.ParameterType.Integer:
                            IntField(mainParameter.IntegerValue, out mainParameter.IntegerValue, "");
                            break;
                        case MainParameter.ParameterType.Float:
                            FloatField(mainParameter.FloatValue, out mainParameter.FloatValue, "");
                            break;
                        case MainParameter.ParameterType.Boolean:
                            Toggle(mainParameter.BooleanValue, out mainParameter.BooleanValue, "");
                            break;
                        case MainParameter.ParameterType.Vector2:
                            Vector2Field(mainParameter.Vector2Value, out mainParameter.Vector2Value, "");
                            break;
                        case MainParameter.ParameterType.Vector3:
                            Vector3Field(mainParameter.Vector3Value, out mainParameter.Vector3Value, "");
                            break;
                        case MainParameter.ParameterType.Color:
                            ColorField(mainParameter.ColorValue, out mainParameter.ColorValue, "");
                            break;
                        case MainParameter.ParameterType.DataSet:
                            ObjectField(mainParameter.DataSet, out mainParameter.DataSet, false, "");
                            break;
                        case MainParameter.ParameterType.Prefab:
                            ObjectField(mainParameter.PrefabValue, out mainParameter.PrefabValue, false, "");
                            break;
                        case MainParameter.ParameterType.Texture:
                            ObjectField(mainParameter.TextureValue, out mainParameter.TextureValue, false, "");
                            break;
                        case MainParameter.ParameterType.AudioClip:
                            ObjectField(mainParameter.AudioClipValue, out mainParameter.AudioClipValue, false, "");
                            break;
                        case MainParameter.ParameterType.Material:
                            ObjectField(mainParameter.MaterialValue, out mainParameter.MaterialValue, false, "");
                            break;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("New", "Minibutton"))
                {
                    Undo.RecordObject(target, "New Main Parameter");
                    Target.MainParameters.Add(new MainParameter());
                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
        #endregion
    }
}