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
        private static List<LnkTools> LnkToolss = new List<LnkTools>();
        private static float Height;

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

            if (IsEnableLnkTools)
            {
                LnkToolss.Clear();
                List<Type> types = EditorReflectionToolkit.GetTypesInEditorAssemblies(false);
                for (int i = 0; i < types.Count; i++)
                {
                    MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < methods.Length; j++)
                    {
                        if (methods[j].IsDefined(typeof(LnkToolsAttribute), false))
                        {
                            LnkTools lnkTools = new LnkTools(methods[j]);
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

                Height = (Mathf.Ceil(LnkToolss.Count / 2f) + 1) * 22 + 3;

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
            float x = sceneView.position.width - 80;
            float y = sceneView.in2DMode ? 5 : 120;

            rect.Set(x, y, 75, Height);
            GUI.Box(rect, "LnkTools", "Window");

            int index = 1;
            int height = 22;
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

                if (index == 1)
                {
                    rect.Set(x + 5, y + height, 30, 20);
                    index = 2;
                }
                else if (index == 2)
                {
                    rect.Set(x + 40, y + height, 30, 20);
                    height += 22;
                    index = 1;
                }

                GUI.backgroundColor = LnkToolss[i].BGColor;
                if (GUI.Button(rect, LnkToolss[i].Content))
                {
                    LnkToolss[i].Method.Invoke(null, null);
                }
                GUI.backgroundColor = Color.white;
            }
            GUI.enabled = true;

            Handles.EndGUI();
        }

        [LnkTools("Save Scene", 0, 1, 0, 1, "SaveAs", LnkToolsMode.OnlyEditor, -5)]
        private static void SaveScene()
        {
            EditorApplication.ExecuteMenuItem("File/Save");
        }
        [LnkTools("Save Project", 0, 1, 0, 1, "SaveAs", LnkToolsMode.OnlyEditor, -4)]
        private static void SaveProject()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }
        [LnkTools("Set Mouse Ray Target", 1, 0.92f, 0.016f, 1, "PhysicsRaycaster Icon", LnkToolsMode.OnlyEditor, -3)]
        private static void SetMouseRayTarget()
        {
            if (Selection.gameObjects.Length > 0)
            {
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Add Bounds Box Collider");
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Set Mouse Ray Target");
            }
        }
        [LnkTools("Set Mouse Ray UI Target", 1, 0.92f, 0.016f, 1, "GraphicRaycaster Icon", LnkToolsMode.OnlyEditor, -2)]
        private static void SetMouseRayUITarget()
        {
            if (Selection.gameObjects.Length > 0)
            {
                EditorApplication.ExecuteMenuItem("HTFramework/Batch/Set Mouse Ray UI Target");
            }
        }
        [LnkTools("Look At Selected Object", "LookAtConstraint Icon", LnkToolsMode.OnlyRuntime, -1)]
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
            public GUIContent Content;
            public Color BGColor;
            public LnkToolsMode Mode;
            public int Priority;
            public MethodInfo Method;

            public LnkTools(MethodInfo method)
            {
                LnkToolsAttribute attribute = method.GetCustomAttribute<LnkToolsAttribute>();
                GUIContent icon = EditorGUIUtility.IconContent(attribute.BuiltinIcon);

                Content = new GUIContent();
                Content.image = icon != null ? icon.image : null;
                Content.tooltip = attribute.Tooltip;
                BGColor = new Color(attribute.R, attribute.G, attribute.B, attribute.A);
                Mode = attribute.Mode;
                Priority = attribute.Priority;
                Method = method;
            }
        }
    }
}