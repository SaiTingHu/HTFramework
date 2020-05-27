using UnityEngine;
using System;
using System.Collections;
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
        /// 是否开始
        /// </summary>
        public bool IsStart { get; private set; } = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; private set; } = false;
        /// <summary>
        /// 是否完成中
        /// </summary>
        private bool _isCompleting { get; set; } = false;

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
        protected virtual void OnStart()
        {
            
        }

        /// <summary>
        /// 任务点开始后，帧刷新
        /// </summary>
        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 完成任务点
        /// </summary>
        /// <param name="completeAction">完成后执行的操作</param>
        public void Complete(HTFAction completeAction = null)
        {
            if (_isCompleting)
            {
                return;
            }

            _isCompleting = true;
            Main.Current.StartCoroutine(CompleteCoroutine(completeAction));
        }

        /// <summary>
        /// 完成任务点协程
        /// </summary>
        /// <param name="completeAction">完成后执行的操作</param>
        private IEnumerator CompleteCoroutine(HTFAction completeAction)
        {
            yield return OnBeforeComplete();
            IsComplete = true;
            completeAction?.Invoke();
            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskPointComplete>().Fill(this, false));
        }

        /// <summary>
        /// 任务点触发完成之前
        /// </summary>
        protected virtual IEnumerator OnBeforeComplete()
        {
            yield return null;
        }

        /// <summary>
        /// 自动完成任务点
        /// </summary>
        public virtual void OnAutoComplete()
        {
            IsComplete = true;
            _isCompleting = true;
            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskPointComplete>().Fill(this, true));
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
        }

        internal void ReSet()
        {
            IsEnable = true;
            IsStart = false;
            IsComplete = false;
            _isCompleting = false;
        }

#if UNITY_EDITOR
        private bool _isDraging = false;
        private bool _isSelected = false;
        private bool _isEditID = false;
        private bool _isEditName = false;
        private bool _isEditDetails = false;
        private bool _isWired = false;
        private bool _isWiredRight = false;
        private Rect _leftWiredOrigin;
        private Rect _rightWiredOrigin;
        private int _height = 0;

        internal Vector2 LeftPosition
        {
            get
            {
                return new Vector2(Anchor.x + 15, Anchor.y + 30);
            }
        }

        internal Vector2 RightPosition
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width - 15, Anchor.y + 30);
            }
        }

        internal Vector2 LeftTangent
        {
            get
            {
                return new Vector2(Anchor.x - 200, Anchor.y + 30);
            }
        }

        internal Vector2 RightTangent
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
            else if (IsComplete)
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

            OnWired();
        }

        internal void OnDrag(Vector2 delta)
        {
            Anchor.position += delta;
        }

        private void OnWired()
        {
            if (_isWired)
            {
                if (_isWiredRight)
                {
                    Handles.DrawBezier(RightPosition, Event.current.mousePosition, RightTangent, Event.current.mousePosition, Color.white, null, 3);
                }
                else
                {
                    Handles.DrawBezier(LeftPosition, Event.current.mousePosition, LeftTangent, Event.current.mousePosition, Color.white, null, 3);
                }
            }
        }

        internal void OnPointEventHandle(Event e, TaskContentBase content)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        _leftWiredOrigin.x += Anchor.x;
                        _leftWiredOrigin.y += Anchor.y;
                        _rightWiredOrigin.x += Anchor.x;
                        _rightWiredOrigin.y += Anchor.y;

                        if (_leftWiredOrigin.Contains(e.mousePosition))
                        {
                            _isWired = true;
                            _isWiredRight = false;
                        }
                        else if (_rightWiredOrigin.Contains(e.mousePosition))
                        {
                            _isWired = true;
                            _isWiredRight = true;
                        }
                        else if (Anchor.Contains(e.mousePosition))
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
                        if (_isSelected)
                        {
                            _leftWiredOrigin.x += Anchor.x;
                            _leftWiredOrigin.y += Anchor.y;
                            _rightWiredOrigin.x += Anchor.x;
                            _rightWiredOrigin.y += Anchor.y;

                            if (_leftWiredOrigin.Contains(e.mousePosition))
                            {
                                GenericMenu gm = new GenericMenu();
                                int index = content.Points.IndexOf(this);
                                for (int i = 0; i < content.Points.Count; i++)
                                {
                                    if (i != index)
                                    {
                                        int m = i;
                                        bool isExist = content.IsExistDepend(index, m);
                                        gm.AddItem(new GUIContent(content.Points[m].Name), isExist, () =>
                                        {
                                            if (isExist)
                                            {
                                                content.DisconnectDepend(index, m);
                                            }
                                            else
                                            {
                                                content.ConnectDepend(index, m);
                                            }
                                        });
                                    }
                                }
                                gm.ShowAsContext();
                            }
                            else if (_rightWiredOrigin.Contains(e.mousePosition))
                            {
                                GenericMenu gm = new GenericMenu();
                                int index = content.Points.IndexOf(this);
                                for (int i = 0; i < content.Points.Count; i++)
                                {
                                    if (i != index)
                                    {
                                        int m = i;
                                        bool isExist = content.IsExistDepend(m, index);
                                        gm.AddItem(new GUIContent(content.Points[m].Name), isExist, () =>
                                        {
                                            if (isExist)
                                            {
                                                content.DisconnectDepend(m, index);
                                            }
                                            else
                                            {
                                                content.ConnectDepend(m, index);
                                            }
                                        });
                                    }
                                }
                                gm.ShowAsContext();
                            }
                            else if (Anchor.Contains(e.mousePosition))
                            {
                                RightClickMenu(content);
                                e.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    int upIndex;
                    if (ChoosePoint(Event.current.mousePosition, content, out upIndex))
                    {
                        if (_isWired)
                        {
                            int originIndex = content.Points.IndexOf(this);
                            if (originIndex != upIndex)
                            {
                                if (_isWiredRight)
                                {
                                    if (content.IsExistDepend(upIndex, originIndex))
                                        content.DisconnectDepend(upIndex, originIndex);
                                    else
                                        content.ConnectDepend(upIndex, originIndex);
                                }
                                else
                                {
                                    if (content.IsExistDepend(originIndex, upIndex))
                                        content.DisconnectDepend(originIndex, upIndex);
                                    else
                                        content.ConnectDepend(originIndex, upIndex);
                                }
                            }
                        }
                    }

                    _isDraging = false;
                    if (_isWired)
                    {
                        _isWired = false;
                        GUI.changed = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (_isWired)
                    {
                        GUI.changed = true;
                    }
                    else if (_isDraging)
                    {
                        OnDrag(e.delta);
                        e.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }
        
        private bool ChoosePoint(Vector2 mousePosition, TaskContentBase content, out int index)
        {
            for (int i = 0; i < content.Points.Count; i++)
            {
                if (content.Points[i].Anchor.Contains(mousePosition))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        internal int OnDependGUI(TaskContentBase taskContent)
        {
            int height = 0;

            GUILayout.BeginHorizontal();

            GUIContent gUIContent = EditorGUIUtility.IconContent("DotFrameDotted");
            gUIContent.tooltip = "Dependent task point";
            GUILayout.Box(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20));
            _leftWiredOrigin = GUILayoutUtility.GetLastRect();

            GUILayout.FlexibleSpace();

            gUIContent.tooltip = "Be dependent task point";
            GUILayout.Box(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20));
            _rightWiredOrigin = GUILayoutUtility.GetLastRect();

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