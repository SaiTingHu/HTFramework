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

            RefreshCount();
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
            GUI.enabled = !_isExistMissed;
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
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = !_isExistMissed;
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
            GUI.enabled = true;
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

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Import Task From Other", EditorGlobalTools.Styles.ButtonLeft))
            {
                string path = EditorUtility.OpenFilePanel("Import Task From Other", Application.dataPath, "asset");
                if (!string.IsNullOrEmpty(path))
                {
                    ImportTaskFromOther(path);
                }
            }
            GUI.enabled = Target.Content.Count > 0;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Clear Task", EditorGlobalTools.Styles.ButtonRight))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to clear all task? It is not allow regrets!", "Yes", "No"))
                {
                    ClearTask();
                }
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Target.Content.Count; i++)
            {
                TaskContentBase content = Target.Content[i];
                if (content != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i + "." + content.Name);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = !_isExistMissed;
                    if (GUILayout.Button("Reorder"))
                    {
                        ReorderPoints(content);
                    }
                    GUI.enabled = true;
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

        /// <summary>
        /// 刷新任务数量
        /// </summary>
        private void RefreshCount()
        {
            _taskContentCount = Target.Content.Count;
            for (int i = 0; i < Target.Content.Count; i++)
            {
                if (Target.Content[i] != null)
                {
                    _taskPointCount += Target.Content[i].Points.Count;
                }
            }
        }
        /// <summary>
        /// 从其他资源导入任务
        /// </summary>
        private void ImportTaskFromOther(string path)
        {
            string assetPath = "Assets" + path.Replace(Application.dataPath, "");
            TaskContentAsset asset = AssetDatabase.LoadAssetAtPath<TaskContentAsset>(assetPath);
            if (asset && asset != Target)
            {
                for (int i = 0; i < asset.Content.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Import......", i + "/" + asset.Content.Count, (float)i / asset.Content.Count);
                    CloneContent(asset.Content[i]);
                }
                HasChanged();
                RefreshCount();
                EditorUtility.ClearProgressBar();
            }
        }
        /// <summary>
        /// 重新排序任务点
        /// </summary>
        private void ReorderPoints(TaskContentBase content)
        {
            TaskPointReorderWindow.ShowWindow(content);
        }
        /// <summary>
        /// 清空所有任务
        /// </summary>
        private void ClearTask()
        {
            for (int i = 0; i < Target.Content.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Clear......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                TaskContentBase content = Target.Content[i];
                for (int j = 0; j < content.Points.Count; j++)
                {
                    TaskContentAsset.DestroySerializeSubObject(content.Points[j], Target);
                }
                TaskContentAsset.DestroySerializeSubObject(content, Target);
            }
            Target.Content.Clear();
            HasChanged();
            RefreshCount();
            EditorUtility.ClearProgressBar();
        }
        /// <summary>
        /// 克隆任务内容
        /// </summary>
        private void CloneContent(TaskContentBase content)
        {
            TaskContentBase taskContent = content.Clone();
            taskContent.Points.Clear();
            for (int i = 0; i < content.Points.Count; i++)
            {
                ClonePoint(taskContent, content.Points[i], content.Points[i].Anchor.position);
            }
            taskContent.GUID = Target.TaskIDName + Target.TaskIDSign.ToString();
            Target.TaskIDSign += 1;
            Target.Content.Add(taskContent);

            TaskContentAsset.GenerateSerializeSubObject(taskContent, Target);
        }
        /// <summary>
        /// 克隆任务点
        /// </summary>
        private void ClonePoint(TaskContentBase content, TaskPointBase point, Vector2 pos)
        {
            TaskPointBase taskPoint = point.Clone();
            taskPoint.Anchor = new Rect(pos.x, pos.y, 0, 0);
            taskPoint.GUID = Target.TaskPointIDName + Target.TaskPointIDSign.ToString();
            Target.TaskPointIDSign += 1;
            content.Points.Add(taskPoint);

            TaskContentAsset.GenerateSerializeSubObject(taskPoint, Target);
        }
    }
}