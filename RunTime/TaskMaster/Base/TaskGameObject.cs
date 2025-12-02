using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务游戏物体（场景中的）
    /// </summary>
    [Serializable]
    public sealed class TaskGameObject
    {
        /// <summary>
        /// 游戏物体实体
        /// </summary>
        internal GameObject AgentEntity;
        /// <summary>
        /// 游戏物体ID
        /// </summary>
        [SerializeField] internal string GUID = "<None>";
        /// <summary>
        /// 游戏物体路径
        /// </summary>
        [SerializeField] internal string Path = "";

        /// <summary>
        /// 游戏物体实体
        /// </summary>
        public GameObject Entity
        {
            get
            {
                if (AgentEntity == null)
                {
                    if (Main.m_TaskMaster)
                    {
                        TaskTarget taskTarget = Main.m_TaskMaster.GetTarget(GUID);
                        AgentEntity = taskTarget != null ? taskTarget.gameObject : null;
                    }
                }
                return AgentEntity;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="taskGameObject">目标物体</param>
        /// <param name="name">绘制名称</param>
        /// <param name="nameWidth">名称宽度</param>
        /// <param name="totalWidth">总宽度</param>
        /// <param name="copy">复制菜单的名称</param>
        /// <param name="paste">粘贴菜单的名称</param>
        public static void DrawField(TaskGameObject taskGameObject, string name, float nameWidth, float totalWidth, string copy = "Copy", string paste = "Paste")
        {
            if (taskGameObject == null)
                return;
            
            GUILayout.BeginHorizontal();

            GUIContent gUIContent = new GUIContent(name);
            gUIContent.tooltip = "GUID: " + taskGameObject.GUID;
            if (GUILayout.Button(gUIContent, "Label", GUILayout.Width(nameWidth)))
            {
                GenericMenu gm = new GenericMenu();
                if (taskGameObject.GUID == "<None>")
                {
                    gm.AddDisabledItem(new GUIContent(copy));
                }
                else
                {
                    gm.AddItem(new GUIContent(copy), false, () =>
                    {
                        GUIUtility.systemCopyBuffer = $"{taskGameObject.GUID}|{taskGameObject.Path}";
                    });
                }
                if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    gm.AddDisabledItem(new GUIContent(paste));
                }
                else
                {
                    gm.AddItem(new GUIContent(paste), false, () =>
                    {
                        string[] buffer = GUIUtility.systemCopyBuffer.Split('|');
                        if (buffer.Length == 2)
                        {
                            taskGameObject.GUID = buffer[0];
                            taskGameObject.Path = buffer[1];
                        }
                    });
                }
                gm.ShowAsContext();
            }

            GUI.color = (taskGameObject.GUID != "<None>") ? Color.white : Color.gray;
            GameObject newEntity = EditorGUILayout.ObjectField(taskGameObject.AgentEntity, typeof(GameObject), true, GUILayout.Width(totalWidth - nameWidth - 35)) as GameObject;
            if (newEntity != taskGameObject.AgentEntity)
            {
                if (newEntity != null)
                {
                    TaskTarget target = newEntity.GetComponent<TaskTarget>();
                    if (!target)
                    {
                        target = newEntity.AddComponent<TaskTarget>();
                        EditorUtility.SetDirty(newEntity);
                    }
                    if (target.GUID == "<None>")
                    {
                        target.GUID = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(target);
                    }
                    taskGameObject.AgentEntity = newEntity;
                    taskGameObject.GUID = target.GUID;
                    taskGameObject.Path = newEntity.transform.FullName();
                }
            }
            GUI.color = Color.white;

            SearchTaskTarget(taskGameObject);

            gUIContent.text = null;
            gUIContent.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            gUIContent.tooltip = "Delete";
            GUI.enabled = taskGameObject.GUID != "<None>";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                taskGameObject.AgentEntity = null;
                taskGameObject.GUID = "<None>";
                taskGameObject.Path = "";
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 在场景中搜索任务目标
        /// </summary>
        private static void SearchTaskTarget(TaskGameObject taskGameObject)
        {
            if (taskGameObject.GUID == "<None>")
                return;

            if (taskGameObject.IsEntityMatched())
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                taskGameObject.AgentEntity = prefabStage.prefabContentsRoot.FindChildren(taskGameObject.Path);
                if (!taskGameObject.IsEntityMatched())
                {
                    taskGameObject.AgentEntity = null;
                    TaskTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<TaskTarget>(true);
                    foreach (TaskTarget target in targets)
                    {
                        if (taskGameObject.GUID == target.GUID)
                        {
                            taskGameObject.AgentEntity = target.gameObject;
                            taskGameObject.Path = target.transform.FullName();
                            taskGameObject.Path = taskGameObject.Path.Substring(taskGameObject.Path.IndexOf("/") + 1);
                            break;
                        }
                    }
                }
            }
            else
            {
                taskGameObject.AgentEntity = GameObject.Find(taskGameObject.Path);
                if (!taskGameObject.IsEntityMatched())
                {
                    taskGameObject.AgentEntity = null;
                    TaskTarget[] targets = UnityEngine.Object.FindObjectsOfType<TaskTarget>(true);
                    foreach (TaskTarget target in targets)
                    {
                        if (taskGameObject.GUID == target.GUID)
                        {
                            taskGameObject.AgentEntity = target.gameObject;
                            taskGameObject.Path = target.transform.FullName();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当前是否存在游戏物体实例，且游戏物体的ID匹配
        /// </summary>
        private bool IsEntityMatched()
        {
            if (AgentEntity == null)
                return false;

            TaskTarget target = AgentEntity.GetComponent<TaskTarget>();
            return target && GUID == target.GUID;
        }
#endif
    }
}