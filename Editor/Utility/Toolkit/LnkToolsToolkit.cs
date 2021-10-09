using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 快捷工具箱
    /// </summary>
    [InitializeOnLoad]
    internal static class LnkToolsToolkit
    {
        private static bool IsEnableLnkTools = false;
        private static bool IsExpansionLnkTools = false;
        private static List<LnkTools> LnkToolss = new List<LnkTools>();

        static LnkToolsToolkit()
        {
            OnInitLnkTools();
        }
        
        /// <summary>
        /// 初始化快捷工具箱
        /// </summary>
        private static void OnInitLnkTools()
        {
            IsEnableLnkTools = EditorPrefs.GetBool(EditorPrefsTable.LnkTools_Enable, true);
            IsExpansionLnkTools = EditorPrefs.GetBool(EditorPrefsTable.LnkTools_Expansion, true);

            if (IsEnableLnkTools)
            {
                LnkToolss.Clear();
                List<Type> types = EditorReflectionToolkit.GetTypesInEditorAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < methods.Length; j++)
                    {
                        if (methods[j].IsDefined(typeof(LnkToolsAttribute), false))
                        {
                            LnkToolsAttribute attribute = methods[j].GetCustomAttribute<LnkToolsAttribute>();
                            LnkTools lnkTools = new LnkTools(attribute.Tooltip, new Color(attribute.R, attribute.G, attribute.B, attribute.A), attribute.Mode, attribute.Priority, methods[j]);
                            LnkToolss.Add(lnkTools);
                        }
                    }
                }

                LnkToolss.Sort((x, y) =>
                {
                    if (x.Priority < y.Priority) return -1;
                    else if (x.Priority == y.Priority) return 0;
                    else return 1;
                });

                SceneView.duringSceneGui += OnLnkToolsGUI;
            }
        }
        /// <summary>
        /// 快捷工具箱界面
        /// </summary>
        private static void OnLnkToolsGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            Rect rect = Rect.zero;
            int h = sceneView.in2DMode ? 5 : 120;

            if (IsExpansionLnkTools)
            {
                rect.Set(sceneView.position.width - 135, h, 130, (LnkToolss.Count + 1) * 22 + 8);
                GUI.Box(rect, "");
            }

            rect.Set(sceneView.position.width - 130, h + 5, 120, 20);
            bool expansion = GUI.Toggle(rect, IsExpansionLnkTools, "LnkTools", "Prebutton");
            if (expansion != IsExpansionLnkTools)
            {
                IsExpansionLnkTools = expansion;
                EditorPrefs.SetBool(EditorPrefsTable.LnkTools_Expansion, IsExpansionLnkTools);
            }
            rect.y += 22;

            if (IsExpansionLnkTools)
            {
                for (int i = 0; i < LnkToolss.Count; i++)
                {
                    if (LnkToolss[i].Mode == LnkToolsMode.OnlyRuntime)
                    {
                        GUI.enabled = EditorApplication.isPlaying;
                    }
                    else if (LnkToolss[i].Mode == LnkToolsMode.OnlyEditor)
                    {
                        GUI.enabled = !EditorApplication.isPlaying;
                    }
                    else
                    {
                        GUI.enabled = true;
                    }

                    GUI.backgroundColor = LnkToolss[i].BGColor;
                    if (GUI.Button(rect, LnkToolss[i].Tooltip))
                    {
                        LnkToolss[i].Method.Invoke(null, null);
                    }
                    GUI.backgroundColor = Color.white;
                    rect.y += 22;
                }
                GUI.enabled = true;
            }
            
            Handles.EndGUI();
        }

        [LnkTools("Save Scene", 0, 1, 0, 1, LnkToolsMode.OnlyEditor, -5)]
        private static void SaveScene()
        {
            EditorApplication.ExecuteMenuItem("File/Save");
        }
        [LnkTools("Save Project", 0, 1, 0, 1, LnkToolsMode.OnlyEditor, -4)]
        private static void SaveProject()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
        [LnkTools("Set Ray Target", 1, 0.92f, 0.016f, 1, LnkToolsMode.OnlyEditor, -3)]
        private static void SetMouseRayTarget()
        {
            if (Selection.gameObjects.Length > 0)
            {
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Add Bounds Box Collider");
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Set Mouse Ray Target");
            }
        }
        [LnkTools("Set Ray UI Target", 1, 0.92f, 0.016f, 1, LnkToolsMode.OnlyEditor, -2)]
        private static void SetMouseRayUITarget()
        {
            if (Selection.gameObjects.Length > 0)
            {
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Set Mouse Ray UI Target");
            }
        }
        [LnkTools("Look At", LnkToolsMode.OnlyRuntime, -1)]
        private static void LookAt()
        {
            if (Main.m_Controller != null)
            {
                if (Selection.activeGameObject != null)
                {
                    Main.m_Controller.SetLookPoint(Selection.activeGameObject.transform.position);
                }
                else
                {
                    Log.Warning("请选中一个LookAt的目标！");
                }
            }
            else
            {
                Log.Warning("场景中不存在框架的Controller模块！");
            }
        }

        private class LnkTools
        {
            public string Tooltip;
            public Color BGColor;
            public LnkToolsMode Mode;
            public int Priority;
            public MethodInfo Method;

            public LnkTools(string tooltip, Color bgColor, LnkToolsMode mode, int priority, MethodInfo method)
            {
                Tooltip = tooltip;
                BGColor = bgColor;
                Mode = mode;
                Priority = priority;
                Method = method;
            }
        }
    }
}