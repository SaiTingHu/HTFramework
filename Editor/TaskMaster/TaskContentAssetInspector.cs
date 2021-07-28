using System;
using System.IO;
using System.Text;
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
                if (asset.IsExistMissed())
                {
                    Log.Error("任务资源存在丢失脚本的对象，请先点击 Clear Missed Task 清空丢失脚本的对象！");
                    return false;
                }
                else
                {
                    TaskEditorWindow.ShowWindow(asset);
                    return true;
                }
            }
            return false;
        }

        private int _taskContentCount = 0;
        private int _taskPointCount = 0;
        private bool _isExistMissed = false;
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
                if (Target.Content[i] != null)
                {
                    _taskPointCount += Target.Content[i].Points.Count;
                }
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
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _taskContentCount, (float)i / _taskContentCount);
                        File.AppendAllText(path, "【" + Target.Content[i].Name + "】\r\n", Encoding.UTF8);
                        for (int j = 0; j < Target.Content[i].Points.Count; j++)
                        {
                            File.AppendAllText(path, Target.Content[i].Points[j].Name + "\r\n", Encoding.UTF8);
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
                if (!string.IsNullOrEmpty(path))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _taskContentCount, (float)i / _taskContentCount);
                        File.AppendAllText(path, "【" + Target.Content[i].Details + "】\r\n", Encoding.UTF8);
                        for (int j = 0; j < Target.Content[i].Points.Count; j++)
                        {
                            File.AppendAllText(path, Target.Content[i].Points[j].Details + "\r\n", Encoding.UTF8);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            if (_isExistMissed)
            {
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Clear Missed Task"))
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        TaskContentBase content = Target.Content[i];
                        if (content == null)
                        {
                            Target.Content.RemoveAt(i);
                            i -= 1;
                        }
                        else
                        {
                            for (int j = 0; j < content.Points.Count; j++)
                            {
                                TaskPointBase point = content.Points[j];
                                if (point == null)
                                {
                                    for (int d = 0; d < content.Depends.Count; d++)
                                    {
                                        if (content.Depends[d].OriginalPoint == j || content.Depends[d].DependPoint == j)
                                        {
                                            content.Depends.RemoveAt(d);
                                            d -= 1;
                                        }
                                        else
                                        {
                                            if (content.Depends[d].OriginalPoint > j)
                                            {
                                                content.Depends[d].OriginalPoint -= 1;
                                            }
                                            if (content.Depends[d].DependPoint > j)
                                            {
                                                content.Depends[d].DependPoint -= 1;
                                            }
                                        }
                                    }
                                    content.Points.RemoveAt(j);
                                    j -= 1;
                                }
                            }
                        }
                    }
                    _isExistMissed = false;
                    HasChanged();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUI.enabled = false;
                GUILayout.Button("Clear Missed Task");
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Target.Content.Count; i++)
            {
                TaskContentBase content = Target.Content[i];
                if (content != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i + "." + content.Name);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    for (int j = 0; j < content.Points.Count; j++)
                    {
                        TaskPointBase point = content.Points[j];
                        if (point != null)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.Label(j + "." + point.Name);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUI.color = Color.red;
                            GUILayout.Space(20);
                            GUILayout.Label(j + ".<Missing script>");
                            GUILayout.FlexibleSpace();
                            GUI.color = Color.white;
                            GUILayout.EndHorizontal();
                            _isExistMissed = true;
                        }
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUI.color = Color.red;
                    GUILayout.Label(i + ".<Missing script>");
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    _isExistMissed = true;
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