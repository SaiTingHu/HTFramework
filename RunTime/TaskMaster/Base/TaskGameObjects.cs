using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务游戏物体数组（场景中的）
    /// </summary>
    [Serializable]
    public sealed class TaskGameObjects
    {
        /// <summary>
        /// 游戏物体ID
        /// </summary>
        [SerializeField] internal List<string> GUIDs = new List<string>();
        /// <summary>
        /// 游戏物体路径
        /// </summary>
        [SerializeField] internal List<string> Paths = new List<string>();

        /// <summary>
        /// 数组的长度
        /// </summary>
        public int Count
        {
            get
            {
                return GUIDs.Count;
            }
        }
        /// <summary>
        /// 根据索引获取游戏物体实体
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>游戏物体实体</returns>
        public GameObject this[int index]
        {
            get
            {
                if (Main.m_TaskMaster)
                {
                    if (index >= 0 && index < GUIDs.Count)
                    {
                        TaskTarget taskTarget = Main.m_TaskMaster.GetTarget(GUIDs[index]);
                        GameObject entity = taskTarget != null ? taskTarget.gameObject : null;
                        return entity;
                    }
                }
                return null;
            }
        }

#if UNITY_EDITOR
        private string _displayName;
        private ReorderableList _reorderableList;
        private List<GameObject> _gameObjects;

        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="taskGameObjects">目标物体</param>
        /// <param name="name">绘制名称</param>
        /// <returns>绘制高度</returns>
        public static int DrawField(TaskGameObjects taskGameObjects, string name)
        {
            if (taskGameObjects == null)
                return 0;

            taskGameObjects.InitializeEditorData(name);

            taskGameObjects._reorderableList.DoLayoutList();

            int count = taskGameObjects.GUIDs.Count;
            if (count <= 0) count = 1;
            return 25 + count * 22;
        }

        /// <summary>
        /// 在场景中搜索任务目标
        /// </summary>
        private static void SearchTaskTarget(TaskGameObjects taskGameObjects, int index)
        {
            if (taskGameObjects.GUIDs[index] == "<None>")
                return;

            if (taskGameObjects._gameObjects[index] != null)
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                taskGameObjects._gameObjects[index] = prefabStage.prefabContentsRoot.FindChildren(taskGameObjects.Paths[index]);
                if (taskGameObjects._gameObjects[index] == null)
                {
                    TaskTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<TaskTarget>(true);
                    foreach (TaskTarget target in targets)
                    {
                        if (taskGameObjects.GUIDs[index] == target.GUID)
                        {
                            taskGameObjects._gameObjects[index] = target.gameObject;
                            taskGameObjects.Paths[index] = target.transform.FullName();
                            taskGameObjects.Paths[index] = taskGameObjects.Paths[index].Substring(taskGameObjects.Paths[index].IndexOf("/") + 1);
                            break;
                        }
                    }
                }
            }
            else
            {
                taskGameObjects._gameObjects[index] = GameObject.Find(taskGameObjects.Paths[index]);
                if (taskGameObjects._gameObjects[index] == null)
                {
                    TaskTarget[] targets = UnityEngine.Object.FindObjectsOfType<TaskTarget>(true);
                    foreach (TaskTarget target in targets)
                    {
                        if (taskGameObjects.GUIDs[index] == target.GUID)
                        {
                            taskGameObjects._gameObjects[index] = target.gameObject;
                            taskGameObjects.Paths[index] = target.transform.FullName();
                            break;
                        }
                    }
                }
            }

            if (taskGameObjects._gameObjects[index] != null)
            {
                TaskTarget target = taskGameObjects._gameObjects[index].GetComponent<TaskTarget>();
                if (!target)
                {
                    target = taskGameObjects._gameObjects[index].AddComponent<TaskTarget>();
                    target.GUID = taskGameObjects.GUIDs[index];
                    EditorUtility.SetDirty(taskGameObjects._gameObjects[index]);
                }
            }
        }

        /// <summary>
        /// 初始化编辑器数据
        /// </summary>
        private void InitializeEditorData(string displayName)
        {
            _displayName = displayName;
            if (_reorderableList == null)
            {
                _reorderableList = new ReorderableList(GUIDs, typeof(string), true, true, false, false);
                _reorderableList.elementHeight = 22;
                _reorderableList.footerHeight = 0;
                _reorderableList.drawHeaderCallback = (Rect rect) =>
                {
                    Rect sub = rect;
                    sub.Set(rect.x, rect.y, rect.width - 60, rect.height);
                    GUI.Label(sub, _displayName);

                    if (!EditorApplication.isPlaying)
                    {
                        GUIContent gc = new GUIContent();
                        gc.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
                        gc.tooltip = "Add a new TaskGameObject";
                        sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                        if (GUI.Button(sub, gc, "InvisibleButton"))
                        {
                            GUIDs.Add("<None>");
                            Paths.Add("");
                            _gameObjects.Add(null);
                        }

                        gc.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
                        gc.tooltip = "Remove select TaskGameObject";
                        sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                        GUI.enabled = _reorderableList.index >= 0 && _reorderableList.index < GUIDs.Count;
                        if (GUI.Button(sub, gc, "InvisibleButton"))
                        {
                            GUIDs.RemoveAt(_reorderableList.index);
                            Paths.RemoveAt(_reorderableList.index);
                            _gameObjects.RemoveAt(_reorderableList.index);
                        }
                        GUI.enabled = true;
                    }
                };
                _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0 && index < GUIDs.Count)
                    {
                        Rect sub = rect;
                        sub.y = sub.y + 2;
                        sub.height = 16;
                        GUI.color = (GUIDs[index] != "<None>") ? Color.white : Color.gray;
                        GameObject entity = EditorGUI.ObjectField(sub, _gameObjects[index], typeof(GameObject), true) as GameObject;
                        if (entity != _gameObjects[index])
                        {
                            if (entity != null)
                            {
                                TaskTarget target = entity.GetComponent<TaskTarget>();
                                if (!target)
                                {
                                    target = entity.AddComponent<TaskTarget>();
                                    EditorUtility.SetDirty(entity);
                                }
                                if (target.GUID == "<None>")
                                {
                                    target.GUID = Guid.NewGuid().ToString();
                                    EditorUtility.SetDirty(target);
                                }
                                _gameObjects[index] = entity;
                                GUIDs[index] = target.GUID;
                                Paths[index] = entity.transform.FullName();
                            }
                        }
                        GUI.color = Color.white;

                        SearchTaskTarget(this, index);
                    }
                };
                _reorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
                _reorderableList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
                {
                    string path = Paths[oldIndex];
                    Paths.RemoveAt(oldIndex);
                    Paths.Insert(newIndex, path);

                    GameObject obj = _gameObjects[oldIndex];
                    _gameObjects.RemoveAt(oldIndex);
                    _gameObjects.Insert(newIndex, obj);
                };
            }
            if (_gameObjects == null)
            {
                _gameObjects = new List<GameObject>();
                for (int i = 0; i < GUIDs.Count; i++)
                {
                    _gameObjects.Add(null);
                }
            }
            while (_gameObjects.Count != GUIDs.Count)
            {
                if (_gameObjects.Count < GUIDs.Count) _gameObjects.Add(null);
                else if (_gameObjects.Count > GUIDs.Count) _gameObjects.RemoveAt(_gameObjects.Count - 1);
            }
        }
#endif
    }
}