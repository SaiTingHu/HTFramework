using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务点基类
    /// </summary>
    public abstract class TaskPointBase : ScriptableObject
    {
        /// <summary>
        /// 任务点ID
        /// </summary>
        public string GUID;
        /// <summary>
        /// 任务点名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 任务点详细介绍
        /// </summary>
        public string Details;
        /// <summary>
        /// 任务点面板的锚点
        /// </summary>
        public Rect Anchor;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsDone { get; protected set; } = false;
        /// <summary>
        /// 是否开始
        /// </summary>
        internal bool IsStart { get; private set; } = false;
        /// <summary>
        /// 是否执行
        /// </summary>
        internal bool IsExecute { get; private set; } = false;

        public TaskPointBase()
        {
            GUID = "";
            Name = "New Task Point";
            Details = "New Task Point";
            Anchor = Rect.zero;
        }
        
        /// <summary>
        /// 任务点开始
        /// </summary>
        public virtual void OnStart()
        {
            
        }

        /// <summary>
        /// 任务点开始后，帧刷新
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 任务点执行
        /// </summary>
        public virtual void OnExecute()
        {

        }

        internal void OnMonitor()
        {
            if (!IsStart)
            {
                IsStart = true;

                OnStart();

                Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskPointStart>().Fill(this));
            }

            OnUpdate();

            if (!IsExecute && IsDone)
            {
                IsExecute = true;

                OnExecute();

                Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskPointExecute>().Fill(this));
            }
        }

        internal void ReSet()
        {
            IsEnable = true;
            IsDone = false;
            IsStart = false;
            IsExecute = false;
        }

#if UNITY_EDITOR
        private bool _isDraging = false;
        private bool _isSelected = false;
        private bool _isEditID = false;
        private bool _isEditName = false;
        private bool _isEditDetails = false;
        private int _height = 0;

        public Vector2 LeftPosition
        {
            get
            {
                return new Vector2(Anchor.x + 15, Anchor.y + 30);
            }
        }

        public Vector2 RightPosition
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width - 15, Anchor.y + 30);
            }
        }

        public Vector2 LeftTangent
        {
            get
            {
                return new Vector2(Anchor.x - 200, Anchor.y + 30);
            }
        }

        public Vector2 RightTangent
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width + 200, Anchor.y + 30);
            }
        }

        internal void OnEditorGUI(TaskContentBase taskContent)
        {
            if (!IsEnable)
            {
                GUI.backgroundColor = Color.gray;
            }
            else if (IsDone)
            {
                GUI.backgroundColor = Color.green;
            }
            else
            {
                GUI.backgroundColor = _isSelected ? Color.yellow : Color.white;
            }

            GUILayout.BeginArea(Anchor, Name, "Window");

            GUI.backgroundColor = Color.white;

            _height = 25;

            _height += OnDependGUI(taskContent);

            _height += OnPropertyGUI();
            
            Anchor.height = _height;

            GUILayout.EndArea();
        }

        internal void OnDrag(Vector2 delta)
        {
            Anchor.position += delta;
        }

        internal void OnPointEventHandle(Event e, TaskContentBase content)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (Anchor.Contains(e.mousePosition))
                        {
                            _isDraging = true;
                            _isSelected = true;
                            GUI.changed = true;
                            GUI.FocusControl(null);
                        }
                        else
                        {
                            _isSelected = false;
                            _isEditID = false;
                            _isEditName = false;
                            _isEditDetails = false;
                            GUI.changed = true;
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (_isSelected && Anchor.Contains(e.mousePosition))
                        {
                            RightClickMenu(content);
                            e.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    _isDraging = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && _isDraging)
                    {
                        OnDrag(e.delta);
                        e.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }

        internal int OnDependGUI(TaskContentBase taskContent)
        {
            int height = 0;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = EditorGUIUtility.IconContent("DotFrameDotted");
            gUIContent.tooltip = "Dependent task point";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                int index = taskContent.Points.IndexOf(this);
                for (int i = 0; i < taskContent.Points.Count; i++)
                {
                    if (i != index)
                    {
                        int m = i;
                        bool isExist = taskContent.IsExistDepend(index, m);
                        gm.AddItem(new GUIContent(taskContent.Points[m].Name), isExist, () =>
                        {
                            if (isExist)
                            {
                                taskContent.DisconnectDepend(index, m);
                            }
                            else
                            {
                                taskContent.ConnectDepend(index, m);
                            }
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.FlexibleSpace();
            gUIContent.tooltip = "Be dependent task point";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                int index = taskContent.Points.IndexOf(this);
                for (int i = 0; i < taskContent.Points.Count; i++)
                {
                    if (i != index)
                    {
                        int m = i;
                        bool isExist = taskContent.IsExistDepend(m, index);
                        gm.AddItem(new GUIContent(taskContent.Points[m].Name), isExist, () =>
                        {
                            if (isExist)
                            {
                                taskContent.DisconnectDepend(m, index);
                            }
                            else
                            {
                                taskContent.ConnectDepend(m, index);
                            }
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }

        public virtual int OnPropertyGUI()
        {
            int height = 0;
            
            #region ID
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID:", GUILayout.Width(50));
            if (_isEditID)
            {
                GUID = EditorGUILayout.TextField(GUID, GUILayout.Width(110));
            }
            else
            {
                GUILayout.Label(GUID, GUILayout.Width(110));
            }
            GUILayout.FlexibleSpace();
            if (_isSelected)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), "IconButton"))
                {
                    _isEditID = !_isEditID;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            #endregion

            height += 20;

            #region Name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            if (_isEditName)
            {
                Name = EditorGUILayout.TextField(Name, GUILayout.Width(110));
            }
            else
            {
                GUILayout.Label(Name, GUILayout.Width(110));
            }
            GUILayout.FlexibleSpace();
            if (_isSelected)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), "IconButton"))
                {
                    _isEditName = !_isEditName;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            #endregion

            height += 20;

            #region Details
            GUILayout.BeginHorizontal();
            GUILayout.Label("Details:", GUILayout.Width(50));
            if (_isEditDetails)
            {
                Details = EditorGUILayout.TextField(Details, GUILayout.Width(110));
            }
            else
            {
                GUILayout.Label(Details, GUILayout.Width(110));
            }
            GUILayout.FlexibleSpace();
            if (_isSelected)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), "IconButton"))
                {
                    _isEditDetails = !_isEditDetails;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            #endregion

            height += 20;

            return height;
        }
        
        private void RightClickMenu(TaskContentBase content)
        {
            GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Delete " + Name), false, () =>
            {
                if (EditorUtility.DisplayDialog("Delete Task Point", "Are you sure you want to delete task point [" + Name + "]?", "Yes", "No"))
                {
                    DeletePoint(content);
                }
            });
            gm.AddItem(new GUIContent("Edit Point Script"), false, () =>
            {
                MonoScript monoScript = MonoScript.FromScriptableObject(this);
                AssetDatabase.OpenAsset(monoScript);
            });
            gm.ShowAsContext();
        }

        private void DeletePoint(TaskContentBase content)
        {
            int index = content.Points.IndexOf(this);
            for (int i = 0; i < content.Depends.Count; i++)
            {
                if (content.Depends[i].OriginalPoint == index || content.Depends[i].DependPoint == index)
                {
                    content.Depends.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    if (content.Depends[i].OriginalPoint > index)
                    {
                        content.Depends[i].OriginalPoint -= 1;
                    }
                    if (content.Depends[i].DependPoint > index)
                    {
                        content.Depends[i].DependPoint -= 1;
                    }
                }
            }

            TaskContentBase.DestroySerializeSubObject(this, content);
            content.Points.Remove(this);
        }

        protected void TaskGameObjectField(ref TaskGameObject taskGameObject, string name, float nameWidth)
        {
            if (taskGameObject == null)
            {
                taskGameObject = new TaskGameObject();
            }

            GUILayout.BeginHorizontal();

            GUIContent gUIContent = new GUIContent(name);
            gUIContent.tooltip = "GUID: " + taskGameObject.GUID;
            GUILayout.Label(gUIContent, GUILayout.Width(nameWidth));

            GUI.color = taskGameObject.AgentEntity ? Color.white : Color.gray;
            GameObject newEntity = EditorGUILayout.ObjectField(taskGameObject.AgentEntity, typeof(GameObject), true, GUILayout.Width(Anchor.width - nameWidth - 35)) as GameObject;
            if (newEntity != taskGameObject.AgentEntity)
            {
                if (newEntity != null)
                {
                    TaskTarget target = newEntity.GetComponent<TaskTarget>();
                    if (!target)
                    {
                        target = newEntity.AddComponent<TaskTarget>();
                    }
                    if (target.GUID == "<None>")
                    {
                        target.GUID = Guid.NewGuid().ToString();
                    }
                    taskGameObject.AgentEntity = newEntity;
                    taskGameObject.GUID = target.GUID;
                    taskGameObject.Path = newEntity.transform.FullName();
                }
            }
            GUI.color = Color.white;

            if (taskGameObject.AgentEntity == null && taskGameObject.GUID != "<None>")
            {
                taskGameObject.AgentEntity = GameObject.Find(taskGameObject.Path);
                if (taskGameObject.AgentEntity == null)
                {
                    TaskTarget[] targets = FindObjectsOfType<TaskTarget>();
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
                else
                {
                    TaskTarget target = taskGameObject.AgentEntity.GetComponent<TaskTarget>();
                    if (!target)
                    {
                        target = taskGameObject.AgentEntity.AddComponent<TaskTarget>();
                        target.GUID = taskGameObject.GUID;
                    }
                }
            }

            gUIContent = EditorGUIUtility.IconContent("TreeEditor.Trash");
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
#endif
    }
}