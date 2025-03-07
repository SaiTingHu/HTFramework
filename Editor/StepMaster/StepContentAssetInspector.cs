using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤内容序列化资源 - 检视器
    /// </summary>
    [CustomEditor(typeof(StepContentAsset))]
    public sealed class StepContentAssetInspector : HTFEditor<StepContentAsset>
    {
        private static HashSet<string> ShowParameterNames = new HashSet<string>();

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            StepContentAsset asset = EditorUtility.InstanceIDToObject(instanceID) as StepContentAsset;
            if (asset)
            {
                StepEditorWindow.ShowWindow(asset);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加需要在StepContentAsset的检视面板显示的参数名称
        /// </summary>
        /// <param name="paraName">参数名称</param>
        public static void AddShowParameterName(string paraName)
        {
            ShowParameterNames.Add(paraName);
        }

        private Vector2 _scroll = Vector2.zero;

        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Step Number: " + Target.Content.Count);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Data To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("Export Step Data", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", $"{i}/{Target.Content.Count}", (float)i / Target.Content.Count);
                        if (!string.IsNullOrEmpty(Target.Content[i].Ancillary))
                        {
                            File.AppendAllText(path, $"【{Target.Content[i].Ancillary}】\r\n", Encoding.UTF8);
                        }
                        File.AppendAllText(path, $"{i + 1}、{Target.Content[i].Name}\r\n{Target.Content[i].Prompt}\r\n", Encoding.UTF8);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Name To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("Export Step Name", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", $"{i}/{Target.Content.Count}", (float)i / Target.Content.Count);
                        if (!string.IsNullOrEmpty(Target.Content[i].Ancillary))
                        {
                            File.AppendAllText(path, $"【{Target.Content[i].Ancillary}】\r\n", Encoding.UTF8);
                        }
                        File.AppendAllText(path, $"{Target.Content[i].Name}\r\n", Encoding.UTF8);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Prompt To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("Export Step Prompt", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", $"{i}/{Target.Content.Count}", (float)i / Target.Content.Count);
                        File.AppendAllText(path, $"{Target.Content[i].Prompt}\r\n", Encoding.UTF8);
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Import Step From Other", EditorGlobalTools.Styles.ButtonLeft))
            {
                string path = EditorUtility.OpenFilePanel("Import Step From Other", Application.dataPath, "asset");
                if (!string.IsNullOrEmpty(path))
                {
                    string assetPath = "Assets" + path.Replace(Application.dataPath, "");
                    StepContentAsset asset = AssetDatabase.LoadAssetAtPath<StepContentAsset>(assetPath);
                    if (asset && asset != Target)
                    {
                        for (int i = 0; i < asset.Content.Count; i++)
                        {
                            EditorUtility.DisplayProgressBar("Import......", $"{i}/{asset.Content.Count}", (float)i / asset.Content.Count);
                            Target.Content.Add(asset.Content[i].Clone());
                        }
                        HasChanged();
                        EditorUtility.ClearProgressBar();
                    }
                }
            }
            GUI.enabled = Target.Content.Count > 0;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Clear Step", EditorGlobalTools.Styles.ButtonRight))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to clear all step? It is not allow regrets!", "Yes", "No"))
                {
                    Target.Content.Clear();
                    HasChanged();
                }
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Target.Content.Count; i++)
            {
                if (!string.IsNullOrEmpty(Target.Content[i].Ancillary))
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.yellow;
                    GUILayout.Label($"【{Target.Content[i].Ancillary}】");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{i}.{Target.Content[i].Name}");
                GUILayout.FlexibleSpace();
                for (int j = 0; j < Target.Content[i].Parameters.Count; j++)
                {
                    StepParameter stepParameter = Target.Content[i].Parameters[j];
                    if (ShowParameterNames.Contains(stepParameter.Name))
                    {
                        GUILayout.Label(stepParameter.ToString());
                        GUILayout.Space(5);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            GUI.Label(new Rect(45, 25, 110, 20), "StepContentAsset", "AssetLabel");
        }
    }
}