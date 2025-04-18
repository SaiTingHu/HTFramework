﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("HTFramework.AI.Editor")]
[assembly: InternalsVisibleTo("HTFramework.Deployment.Editor")]
[assembly: InternalsVisibleTo("HTFramework.GC.Editor")]

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/102956756")]
    internal sealed class MainInspector : InternalModuleInspector<Main, IMainHelper>
    {
        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            PageEnable();
            ScriptingDefineEnable();
            DataModelEnable();
            ParameterEnable();
#if HOTFIX_HybridCLR
            HybridCLREnable();
#endif
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PageGUI();
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            Target.Pause = EditorGUILayout.Toggle("Pause", Target.Pause);
            GUILayout.EndHorizontal();
        }

        #region Page
        private PagePainter _pagePainter;

        private void PageEnable()
        {
            _pagePainter = new PagePainter(this);
            _pagePainter.AddPage("Scripting Define", EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image, ScriptingDefineGUI);
            _pagePainter.AddPage("Data Model", EditorGUIUtility.IconContent("SceneViewOrtho").image, DataModelGUI);
            _pagePainter.AddPage("License", EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image, LicenseGUI);
            _pagePainter.AddPage("Parameter", EditorGUIUtility.IconContent("UnityEditor.HierarchyWindow").image, ParameterGUI);
            _pagePainter.AddPage("Setting", EditorGUIUtility.IconContent("SettingsIcon").image, SettingGUI);
#if HOTFIX_HybridCLR
            _pagePainter.AddPage("HybridCLR", EditorGUIUtility.IconContent("Update-Available").image, HybridCLRGUI);
#endif
        }
        private void PageGUI()
        {
            _pagePainter.Painting();
        }
        #endregion

        #region ScriptingDefine
        private ScriptingDefine _currentScriptingDefine;
        private bool _isNewDefine = false;
        private string _newDefine = "";
        private GUIContent _predefinedGC;

        private void ScriptingDefineEnable()
        {
            _currentScriptingDefine = new ScriptingDefine();
            _predefinedGC = new GUIContent();
        }
        private void ScriptingDefineGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Defined");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField(_currentScriptingDefine.Defined);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = _currentScriptingDefine.IsAnyDefined;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove", EditorGlobalTools.Styles.ButtonLeft))
            {
                GenericMenu gm = new GenericMenu();
                foreach (var item in _currentScriptingDefine.Defineds)
                {
                    gm.AddItem(new GUIContent(item), false, () =>
                    {
                        _currentScriptingDefine.RemoveDefine(item);
                    });
                }
                gm.ShowAsContext();
            }
            if (GUILayout.Button("Clear", EditorGlobalTools.Styles.ButtonMid))
            {
                _currentScriptingDefine.ClearDefines();
            }
            GUI.enabled = true;
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("New", EditorGlobalTools.Styles.ButtonMid))
            {
                _isNewDefine = !_isNewDefine;
                _newDefine = "";
            }
            if (GUILayout.Button("Apply", EditorGlobalTools.Styles.ButtonRight))
            {
                _currentScriptingDefine.Apply();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            if (_isNewDefine)
            {
                GUILayout.BeginHorizontal();
                _newDefine = EditorGUILayout.TextField(_newDefine);
                if (GUILayout.Button("OK", EditorStyles.miniButtonLeft, GUILayout.Width(30)))
                {
                    if (!string.IsNullOrEmpty(_newDefine))
                    {
                        _currentScriptingDefine.AddDefine(_newDefine);
                        _isNewDefine = false;
                        _newDefine = "";
                    }
                    else
                    {
                        Log.Error("输入的脚本定义不能为空！");
                    }
                }
                if (GUILayout.Button("NO", EditorStyles.miniButtonRight, GUILayout.Width(30)))
                {
                    _isNewDefine = false;
                    _newDefine = "";
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Predefined:");
                GUILayout.EndHorizontal();

                DrawPredefined("DISABLE_ASPECTTRACK", "禁用 AspectTrack 模块");
                DrawPredefined("DISABLE_BUILDER", "禁用框架构建器");
                DrawPredefined("HOTFIX_HybridCLR", "启用 HybridCLR 热更新");

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Historical Record:");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear Record", GUILayout.Width(90)))
                {
                    _currentScriptingDefine.ClearDefinesRecord();
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < _currentScriptingDefine.DefinedsRecord.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUI.enabled = !_currentScriptingDefine.Defined.Contains(_currentScriptingDefine.DefinedsRecord[i]);
                    GUILayout.Label(_currentScriptingDefine.DefinedsRecord[i], "PR PrefabLabel");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Use", GUILayout.Width(40)))
                    {
                        _newDefine += $"{_currentScriptingDefine.DefinedsRecord[i]};";
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
            }
        }
        private void DrawPredefined(string define, string tooltip)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _predefinedGC.text = define;
            _predefinedGC.tooltip = tooltip;
            GUI.enabled = !_currentScriptingDefine.Defined.Contains(define);
            GUILayout.Label(_predefinedGC, "PR PrefabLabel");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Use", GUILayout.Width(40)))
            {
                _newDefine += $"{define};";
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private sealed class ScriptingDefine
        {
            /// <summary>
            /// 宏定义记录
            /// </summary>
            public List<string> DefinedsRecord { get; } = new List<string>();
            /// <summary>
            /// 宏定义
            /// </summary>
            public HashSet<string> Defineds { get; } = new HashSet<string>();
            /// <summary>
            /// 宏定义记录
            /// </summary>
            public string DefinedRecord { get; private set; } = "";
            /// <summary>
            /// 宏定义
            /// </summary>
            public string Defined { get; private set; } = "";
            /// <summary>
            /// 是否存在任意宏定义
            /// </summary>
            public bool IsAnyDefined
            {
                get
                {
                    return !string.IsNullOrEmpty(Defined);
                }
            }

            public ScriptingDefine()
            {
                AddDefineRecord(EditorPrefs.GetString(EditorPrefsTable.ScriptingDefine_Record, null));

                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                AddDefine(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));
            }

            /// <summary>
            /// 添加宏定义
            /// </summary>
            public void AddDefine(string define)
            {
                if (string.IsNullOrEmpty(define))
                    return;

                string[] defines = define.Split(';');
                for (int i = 0; i < defines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(defines[i]) && !Defineds.Contains(defines[i]))
                    {
                        Defined += $"{defines[i]};";
                        Defineds.Add(defines[i]);
                    }
                }
                AddDefineRecord(define);
            }
            /// <summary>
            /// 移除宏定义
            /// </summary>
            public void RemoveDefine(string define)
            {
                if (string.IsNullOrEmpty(define))
                    return;

                if (Defineds.Contains(define))
                {
                    Defined = Defined.Replace($"{define};", "");
                    Defineds.Remove(define);
                }
            }
            /// <summary>
            /// 清空所有宏定义
            /// </summary>
            public void ClearDefines()
            {
                Defined = "";
                Defineds.Clear();
            }
            /// <summary>
            /// 应用宏定义
            /// </summary>
            public void Apply()
            {
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, Defined);
            }
            /// <summary>
            /// 清空宏定义记录
            /// </summary>
            public void ClearDefinesRecord()
            {
                DefinedRecord = "";
                DefinedsRecord.Clear();
                EditorPrefs.SetString(EditorPrefsTable.ScriptingDefine_Record, DefinedRecord);
            }

            private void AddDefineRecord(string define)
            {
                if (string.IsNullOrEmpty(define))
                    return;

                string[] defines = define.Split(';');
                for (int i = 0; i < defines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(defines[i]) && !DefinedsRecord.Contains(defines[i]))
                    {
                        DefinedRecord += $"{defines[i]};";
                        DefinedsRecord.Add(defines[i]);
                    }
                }
                EditorPrefs.SetString(EditorPrefsTable.ScriptingDefine_Record, DefinedRecord);
            }
        }
        #endregion

        #region DataModel
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private GUIContent _editGC;
        private SerializedProperty _dataModelTypes;
        private ReorderableList _dataModelList;

        private void DataModelEnable()
        {
            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new DataModel";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select DataModel";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit DataModel script";

            _dataModelTypes = GetProperty("DataModelTypes");
            _dataModelList = new ReorderableList(serializedObject, _dataModelTypes, true, true, false, false);
            _dataModelList.footerHeight = 0;
            _dataModelList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Data Models:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, EditorGlobalTools.Styles.InvisibleButton))
                    {
                        GenericMenu gm = new GenericMenu();
                        List<Type> types = ReflectionToolkit.GetTypesInAllAssemblies(type =>
                        {
                            return type.IsSubclassOf(typeof(DataModelBase));
                        }, false);
                        for (int i = 0; i < types.Count; i++)
                        {
                            int j = i;
                            if (Target.DataModelTypes.Contains(types[j].FullName))
                            {
                                gm.AddDisabledItem(new GUIContent(types[j].FullName), true);
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(types[j].FullName), false, () =>
                                {
                                    Undo.RecordObject(target, "Add Data Model");
                                    Target.DataModelTypes.Add(types[j].FullName);
                                    HasChanged();
                                });
                            }
                        }
                        gm.ShowAsContext();
                    }

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _dataModelList.index >= 0 && _dataModelList.index < Target.DataModelTypes.Count;
                    if (GUI.Button(sub, _removeGC, EditorGlobalTools.Styles.InvisibleButton))
                    {
                        Undo.RecordObject(target, "Remove Data Model");
                        Target.DataModelTypes.RemoveAt(_dataModelList.index);
                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _dataModelList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.DataModelTypes.Count)
                {
                    Rect subrect = rect;
                    subrect.Set(rect.x, rect.y + 2, rect.width, 16);
                    GUI.Label(subrect, Target.DataModelTypes[index]);

                    if (isActive && isFocused)
                    {
                        subrect.Set(rect.x + rect.width - 20, rect.y, 20, 20);
                        if (GUI.Button(subrect, _editGC, EditorGlobalTools.Styles.InvisibleButton))
                        {
                            CSharpScriptToolkit.OpenScript(Target.DataModelTypes[index]);
                        }
                    }
                }
            };
            _dataModelList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
        }
        private void DataModelGUI()
        {
            _dataModelList.DoLayoutList();
        }
        #endregion

        #region License
        private void LicenseGUI()
        {
            PropertyField(nameof(Main.IsPermanentLicense), "Permanent License");

            if (!Target.IsPermanentLicense)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Licenser", GUILayout.Width(LabelWidth));
                if (GUILayout.Button(Target.LicenserType, EditorGlobalTools.Styles.MiniPopup))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = ReflectionToolkit.GetTypesInAllAssemblies(type =>
                    {
                        return type.IsSubclassOf(typeof(LicenserBase)) && !type.IsAbstract;
                    }, false);
                    gm.AddItem(new GUIContent("<None>"), Target.LicenserType == "<None>", () =>
                    {
                        Undo.RecordObject(target, "Set Licenser");
                        Target.LicenserType = "<None>";
                        HasChanged();
                    });
                    for (int i = 0; i < types.Count; i++)
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.LicenserType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set Licenser");
                            Target.LicenserType = types[j].FullName;
                            HasChanged();
                        });
                    }
                    gm.ShowAsContext();
                }
                GUILayout.EndHorizontal();
            }
        }
        #endregion

        #region Parameter
        private SerializedProperty _mainParameters;
        private ReorderableList _parameterList;        

        private void ParameterEnable()
        {
            _mainParameters = GetProperty("MainParameters");
            _parameterList = new ReorderableList(serializedObject, _mainParameters, true, false, true, true);
            _parameterList.headerHeight = 2;
            _parameterList.elementHeight = 65;
            _parameterList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Parameter");
            };
            _parameterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty mainParameter = _mainParameters.GetArrayElementAtIndex(index);
                SerializedProperty nameProperty = mainParameter.FindPropertyRelative("Name");
                SerializedProperty typeProperty = mainParameter.FindPropertyRelative("Type");
                
                Rect subrect = rect;

                subrect.Set(rect.x, rect.y + 2, 50, 16);
                GUI.Label(subrect, "Name:");
                if (isActive)
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
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(subrect, valueProperty.objectReferenceValue, typeof(GameObject), false);
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
            _parameterList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
        }
        private void ParameterGUI()
        {
            _parameterList.DoLayoutList();
        }
        #endregion

        #region Setting
        private void SettingGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GUI", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(Main.DefaultSkin), "Default Skin");

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Log", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(Main.IsEnabledLogInfo), "Enabled Log Info");
            PropertyField(nameof(Main.IsEnabledLogWarning), "Enabled Log Warning");
            PropertyField(nameof(Main.IsEnabledLogError), "Enabled Log Error");

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(Main.IsAllowSceneAddBuild), "Allow Scene Add Build");
        }
        #endregion

        #region HybridCLR
#if HOTFIX_HybridCLR
        private bool _isInstalled;
        private string _localVersion;

        private void HybridCLREnable()
        {
            string localIl2CppDir = $"{PathToolkit.ProjectPath}HybridCLRData/LocalIl2CppData-{Application.platform}/il2cpp";
            string hybridclrDir = $"{localIl2CppDir}/libil2cpp/hybridclr";
            string localVersionFile = $"{localIl2CppDir}/libil2cpp/hybridclr/generated/libil2cpp-version.txt";
            _isInstalled = Directory.Exists(hybridclrDir);
            _localVersion = File.Exists(localVersionFile) ? ("v" + File.ReadAllText(localVersionFile, Encoding.UTF8)) : "Unknown";
        }
        private void HybridCLRGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Installed", _isInstalled.ToString(), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Installed Version", _localVersion, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel = 1;

            PropertyField(nameof(Main.MetadataNames), "Metadata Names");
            PropertyField(nameof(Main.HotfixAssemblyNames), "Hotfix Assembly Names");

            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (EditorGUILayout.LinkButton("Quick Start"))
            {
                Application.OpenURL("https://hybridclr.doc.code-philosophy.com/docs/beginner/quickstart");
            }
            EditorGUILayout.EndHorizontal();
        }
#endif
        #endregion
    }
}