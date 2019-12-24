using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
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
        private static bool _showSetting = false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            ScriptingDefineEnable();
            ParameterEnable();
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
            SettingGUI();
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
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

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
                EditorGUILayout.TextField(_currentScriptingDefine.Defined);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUI.enabled = _currentScriptingDefine.IsAnyDefined;
                if (GUILayout.Button("Clear", EditorStyles.miniButtonLeft))
                {
                    _currentScriptingDefine.ClearDefines();
                }
                GUI.enabled = true;
                if (GUILayout.Button("New", EditorStyles.miniButtonMid))
                {
                    _isNewDefine = !_isNewDefine;
                    _newDefine = "";
                }
                if (GUILayout.Button("Apply", EditorStyles.miniButtonRight))
                {
                    _currentScriptingDefine.Apply();
                }
                GUILayout.EndHorizontal();

                if (_isNewDefine)
                {
                    GUILayout.BeginHorizontal();
                    _newDefine = EditorGUILayout.TextField(_newDefine);
                    if (GUILayout.Button("OK", EditorStyles.miniButtonLeft, GUILayout.Width(30)))
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
                    if (GUILayout.Button("NO", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                    {
                        _isNewDefine = false;
                        _newDefine = "";
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Historical record");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear record", EditorStyles.miniButton, GUILayout.Width(80)))
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
                        if (GUILayout.Button("Use", EditorStyles.miniButton, GUILayout.Width(30)))
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
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showMainData = EditorGUILayout.Foldout(_showMainData, "Main Data", true);
            GUILayout.EndHorizontal();

            if (_showMainData)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("MainData");
                if (GUILayout.Button(Target.MainDataType, EditorGlobalTools.Styles.MiniPopup))
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
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

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
                    GUILayout.Label("Prompt:", EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    TextField(Target.EndingPrompt, out Target.EndingPrompt, "");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ending Time:", EditorStyles.boldLabel);
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
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLPlus, GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Year");
                        Target.Year += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLMinus, GUILayout.Width(15)))
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
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLPlus, GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Month");
                        Target.Month += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLMinus, GUILayout.Width(15)))
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
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLPlus, GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        Target.Day += 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("", EditorGlobalTools.Styles.OLMinus, GUILayout.Width(15)))
                    {
                        Undo.RecordObject(target, "Set Day");
                        Target.Day -= 1;
                        CorrectDateTime();
                        HasChanged();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Now", EditorStyles.miniButtonLeft))
                    {
                        Undo.RecordObject(target, "Set Now");
                        Target.Year = DateTime.Now.Year;
                        Target.Month = DateTime.Now.Month;
                        Target.Day = DateTime.Now.Day;
                        CorrectDateTime();
                        HasChanged();
                    }
                    if (GUILayout.Button("2 Months Later", EditorStyles.miniButtonRight))
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
        private SerializedProperty _mainParameters;
        private ReorderableList _parameterList;        

        private void ParameterEnable()
        {
            _mainParameters = GetProperty("MainParameters");
            _parameterList = new ReorderableList(serializedObject, _mainParameters, true, true, true, true);
            _parameterList.elementHeight = 65;
            _parameterList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Parameter");
            };
            _parameterList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                SerializedProperty mainParameter = _mainParameters.GetArrayElementAtIndex(index);
                SerializedProperty nameProperty = mainParameter.FindPropertyRelative("Name");
                SerializedProperty typeProperty = mainParameter.FindPropertyRelative("Type");
                
                Rect subrect = rect;

                subrect.Set(rect.x, rect.y + 2, 50, 16);
                GUI.Label(subrect, "Name:");
                if (selected)
                {
                    subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                    nameProperty.stringValue = EditorGUI.TextField(subrect, nameProperty.stringValue);
                }
                else
                {
                    subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                    GUI.Label(subrect, nameProperty.stringValue);
                }

                subrect.Set(rect.x, rect.y + 22, 50, 16);
                GUI.Label(subrect, "Type:");
                subrect.Set(rect.x + 50, rect.y + 22, rect.width - 50, 16);
                typeProperty.enumValueIndex = (int)(MainParameter.ParameterType)EditorGUI.EnumPopup(subrect, (MainParameter.ParameterType)typeProperty.enumValueIndex);

                subrect.Set(rect.x, rect.y + 42, 50, 16);
                GUI.Label(subrect, "Value:");
                subrect.Set(rect.x + 50, rect.y + 42, rect.width - 50, 16);
                SerializedProperty valueProperty;
                switch ((MainParameter.ParameterType)typeProperty.enumValueIndex)
                {
                    case MainParameter.ParameterType.String:
                        valueProperty = mainParameter.FindPropertyRelative("StringValue");
                        valueProperty.stringValue = EditorGUI.TextField(subrect, valueProperty.stringValue);
                        break;
                    case MainParameter.ParameterType.Integer:
                        valueProperty = mainParameter.FindPropertyRelative("IntegerValue");
                        valueProperty.intValue = EditorGUI.IntField(subrect, valueProperty.intValue);
                        break;
                    case MainParameter.ParameterType.Float:
                        valueProperty = mainParameter.FindPropertyRelative("FloatValue");
                        valueProperty.floatValue = EditorGUI.FloatField(subrect, valueProperty.floatValue);
                        break;
                    case MainParameter.ParameterType.Boolean:
                        valueProperty = mainParameter.FindPropertyRelative("BooleanValue");
                        valueProperty.boolValue = EditorGUI.Toggle(subrect, valueProperty.boolValue);
                        break;
                    case MainParameter.ParameterType.Vector2:
                        valueProperty = mainParameter.FindPropertyRelative("Vector2Value");
                        valueProperty.vector2Value = EditorGUI.Vector2Field(subrect, "", valueProperty.vector2Value);
                        break;
                    case MainParameter.ParameterType.Vector3:
                        valueProperty = mainParameter.FindPropertyRelative("Vector3Value");
                        valueProperty.vector3Value = EditorGUI.Vector3Field(subrect, "", valueProperty.vector3Value);
                        break;
                    case MainParameter.ParameterType.Color:
                        valueProperty = mainParameter.FindPropertyRelative("ColorValue");
                        valueProperty.colorValue = EditorGUI.ColorField(subrect, valueProperty.colorValue);
                        break;
                    case MainParameter.ParameterType.DataSet:
                        valueProperty = mainParameter.FindPropertyRelative("DataSet");
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(DataSetBase), false);
                        break;
                    case MainParameter.ParameterType.Prefab:
                        valueProperty = mainParameter.FindPropertyRelative("PrefabValue");
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(GameObject), true);
                        break;
                    case MainParameter.ParameterType.Texture:
                        valueProperty = mainParameter.FindPropertyRelative("TextureValue");
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(Texture), false);
                        break;
                    case MainParameter.ParameterType.AudioClip:
                        valueProperty = mainParameter.FindPropertyRelative("AudioClipValue");
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(AudioClip), false);
                        break;
                    case MainParameter.ParameterType.Material:
                        valueProperty = mainParameter.FindPropertyRelative("MaterialValue");
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(Material), false);
                        break;
                }
            };
        }
        private void ParameterGUI()
        {
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showParameter = EditorGUILayout.Foldout(_showParameter, "Parameter", true);
            GUILayout.EndHorizontal();

            if (_showParameter)
            {
                _parameterList.DoLayoutList();
            }

            GUILayout.EndVertical();
        }
        #endregion

        #region Setting
        private void SettingGUI()
        {
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _showSetting = EditorGUILayout.Foldout(_showSetting, "Setting", true);
            GUILayout.EndHorizontal();

            if (_showSetting)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Log", EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnabledLogInfo, out Target.IsEnabledLogInfo, "Enabled Log Info");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnabledLogWarning, out Target.IsEnabledLogWarning, "Enabled Log Warning");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnabledLogError, out Target.IsEnabledLogError, "Enabled Log Error");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
        #endregion
    }
}