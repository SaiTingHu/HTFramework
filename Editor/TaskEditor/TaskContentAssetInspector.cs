using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(TaskContentAsset))]
    internal sealed class TaskContentAssetInspector : HTFEditor<TaskContentAsset>
    {
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            TaskContentAsset asset = EditorUtility.InstanceIDToObject(instanceID) as TaskContentAsset;
            if (asset)
            {
                TaskEditorWindow.ShowWindow(asset);
                return true;
            }
            return false;
        }

        private int _taskContentCount = 0;
        private int _taskPointCount = 0;
        private Vector2 _scroll = Vector2.zero;

        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _taskContentCount = Target.Content.Count;
            for (int i = 0; i < Target.Content.Count; i++)
            {
                _taskPointCount += Target.Content[i].Points.Count;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Task Content Number: " + _taskContentCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Task Point Number: " + _taskPointCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Task Name To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _taskContentCount, (float)i / _taskContentCount);
                        File.AppendAllText(path, "【" + Target.Content[i].Name + "】\r\n");
                        for (int j = 0; j < Target.Content[i].Points.Count; j++)
                        {
                            File.AppendAllText(path, Target.Content[i].Points[j].Name + "\r\n");
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Task Details To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _taskContentCount, (float)i / _taskContentCount);
                        File.AppendAllText(path, "【" + Target.Content[i].Details + "】\r\n");
                        for (int j = 0; j < Target.Content[i].Points.Count; j++)
                        {
                            File.AppendAllText(path, Target.Content[i].Points[j].Details + "\r\n");
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Target.Content.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(i + "." + Target.Content[i].Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                for (int j = 0; j < Target.Content[i].Points.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(j + "." + Target.Content[i].Points[j].Name);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            GUI.Label(new Rect(45, 25, 110, 20), "TaskContentAsset", "AssetLabel");
        }
    }
}