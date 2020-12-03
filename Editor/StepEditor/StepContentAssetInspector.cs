using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepContentAsset))]
    internal sealed class StepContentAssetInspector : HTFEditor<StepContentAsset>
    {
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
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        if (Target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + Target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, (i + 1) + "、" + Target.Content[i].Name + "\r\n" + Target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Name To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("Export Step Name", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        if (Target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + Target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, Target.Content[i].Name + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Prompt To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("Export Step Prompt", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        File.AppendAllText(path, Target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Step From Other", EditorGlobalTools.Styles.ButtonLeft))
            {
                string path = EditorUtility.OpenFilePanel("Import Step From Other", Application.dataPath, "asset");
                if (path != "")
                {
                    string assetPath = "Assets" + path.Replace(Application.dataPath, "");
                    StepContentAsset asset = AssetDatabase.LoadAssetAtPath<StepContentAsset>(assetPath);
                    if (asset && asset != Target)
                    {
                        for (int i = 0; i < asset.Content.Count; i++)
                        {
                            EditorUtility.DisplayProgressBar("Import......", i + "/" + asset.Content.Count, (float)i / asset.Content.Count);
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
                if (Target.Content[i].Ancillary != "")
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.yellow;
                    GUILayout.Label("【" + Target.Content[i].Ancillary + "】");
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(i + "." + Target.Content[i].Name);
                GUILayout.FlexibleSpace();
                GUILayout.Label("[" + Target.Content[i].Trigger + "]");
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