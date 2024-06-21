using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
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
        [SerializeField] internal string GUID = "";
        /// <summary>
        /// 任务点名称
        /// </summary>
        [SerializeField] internal string Name = "";
        /// <summary>
        /// 任务点细节
        /// </summary>
        [SerializeField] internal string Details = "";
        /// <summary>
        /// 是否展示任务点细节
        /// </summary>
        [SerializeField] internal bool IsDisplayDetails = false;
        /// <summary>
        /// 任务点目标
        /// </summary>
        [SerializeField] internal TaskGameObject Target = new TaskGameObject();
        /// <summary>
        /// 任务点面板的锚点
        /// </summary>
        [SerializeField] internal Rect Anchor = Rect.zero;
        /// <summary>
        /// 是否启用，未启用时任务点将自动完成
        /// </summary>
        [SerializeField] internal bool IsEnable = true;
        /// <summary>
        /// 是否在编辑器中展开
        /// </summary>
        [SerializeField] internal bool IsExpand = true;

        private TaskTarget _target;

        /// <summary>
        /// 获取任务点ID
        /// </summary>
        public string GetID
        {
            get
            {
                return GUID;
            }
        }
        /// <summary>
        /// 获取任务点名称
        /// </summary>
        public string GetName
        {
            get
            {
                return Name;
            }
        }
        /// <summary>
        /// 获取任务点细节
        /// </summary>
        public string GetDetails
        {
            get
            {
                return Details;
            }
        }
        /// <summary>
        /// 获取是否展示任务点细节
        /// </summary>
        public bool GetIsDisplayDetails
        {
            get
            {
                return IsDisplayDetails;
            }
        }
        /// <summary>
        /// 获取任务点目标
        /// </summary>
        public GameObject GetTarget
        {
            get
            {
                return Target.Entity;
            }
        }
        /// <summary>
        /// 获取任务点目标
        /// </summary>
        public TaskTarget GetTaskTarget
        {
            get
            {
                if (GetTarget == null)
                {
                    Log.Error($"任务点 {GetName} 的目标为空，这是不被允许的！");
                    return null;
                }

                if (_target == null)
                {
                    _target = GetTarget.GetComponent<TaskTarget>();
                }
                return _target;
            }
        }
        /// <summary>
        /// 是否启用，未启用时任务点将自动完成（运行时）
        /// </summary>
        public bool IsEnableRunTime { get; set; } = true;
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
        public bool IsCompleting { get; private set; } = false;
        /// <summary>
        /// 前置任务点
        /// </summary>
        internal HashSet<TaskPointBase> LastPoints { get; private set; } = new HashSet<TaskPointBase>();
        /// <summary>
        /// 后续任务点
        /// </summary>
        internal HashSet<TaskPointBase> NextPoints { get; private set; } = new HashSet<TaskPointBase>();

        /// <summary>
        /// 任务点开始
        /// </summary>
        protected virtual void OnStart()
        {

        }
        /// <summary>
        /// 任务点开始后，帧更新
        /// </summary>
        protected virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 任务点指引
        /// </summary>
        protected virtual void OnGuide()
        {

        }
        /// <summary>
        /// 任务点完成
        /// </summary>
        protected virtual IEnumerator OnComplete()
        {
            yield return null;
        }
        /// <summary>
        /// 任务点自动完成
        /// </summary>
        protected virtual void OnAutoComplete()
        {

        }
        /// <summary>
        /// 任务点结束
        /// </summary>
        protected virtual void OnEnd()
        {

        }

        /// <summary>
        /// 任务点开始
        /// </summary>
        /// <param name="isAuto">是否自动完成</param>
        private void StartPoint(bool isAuto)
        {
            if (IsStart)
                return;

            IsStart = true;

            try
            {
                OnStart();
            }
            catch (Exception e)
            {
                Log.Error($"任务控制器：开始任务点【{Name}】时出错！错误描述：{e}", null, true);
            }

            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskPointStart>().Fill(this, IsEnable && IsEnableRunTime, isAuto));
        }
        /// <summary>
        /// 任务点的前置依赖完成后，每帧监测
        /// </summary>
        internal void MonitorPoint()
        {
            StartPoint(false);

            OnUpdate();
        }
        /// <summary>
        /// 任务点指引
        /// </summary>
        internal void GuidePoint()
        {
            StartPoint(false);

            if (IsComplete)
                return;

            if (IsCompleting)
                return;

            try
            {
                OnGuide();
            }
            catch (Exception e)
            {
                Log.Error($"任务控制器：指引任务点【{Name}】时出错！错误描述：{e}", null, true);
            }
        }
        /// <summary>
        /// 完成任务点
        /// </summary>
        public void Complete()
        {
            StartPoint(false);

            if (IsComplete)
                return;

            if (IsCompleting)
                return;

            IsCompleting = true;
            Main.Current.StartCoroutine(CompleteCoroutine());
        }
        /// <summary>
        /// 完成任务点协程
        /// </summary>
        /// <param name="completeAction">完成后执行的操作</param>
        private IEnumerator CompleteCoroutine()
        {
            yield return OnComplete();

            if (GetTaskTarget != null)
            {
                GetTaskTarget.OnTaskPointComplete.Invoke();
                if (!GetTaskTarget.CompletingTime.Approximately(0))
                {
                    yield return YieldInstructioner.GetWaitForSeconds(GetTaskTarget.CompletingTime);
                }
            }

            IsComplete = true;
            IsCompleting = false;

            EndPoint();

            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskPointComplete>().Fill(this, IsEnable && IsEnableRunTime, false));
        }
        /// <summary>
        /// 自动完成任务点
        /// </summary>
        /// <returns>是否成功</returns>
        internal bool AutoComplete()
        {
            StartPoint(true);

            if (IsComplete)
                return false;

            if (IsCompleting)
                return false;

            try
            {
                OnAutoComplete();
            }
            catch (Exception e)
            {
                Log.Error($"任务控制器：自动完成任务点【{Name}】时出错！错误描述：{e}", null, true);
            }

            if (GetTaskTarget != null)
            {
                GetTaskTarget.OnTaskPointAutoComplete.Invoke();
            }

            IsComplete = true;

            EndPoint();

            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskPointComplete>().Fill(this, IsEnable && IsEnableRunTime, true));
            return true;
        }
        /// <summary>
        /// 任务点结束
        /// </summary>
        private void EndPoint()
        {
            IsStart = false;

            try
            {
                OnEnd();
            }
            catch (Exception e)
            {
                Log.Error($"任务控制器：结束任务点【{Name}】时出错！错误描述：{e}", null, true);
            }
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        internal void ReSet()
        {
            IsEnableRunTime = true;
            IsStart = false;
            IsComplete = false;
            IsCompleting = false;
            LastPoints.Clear();
            NextPoints.Clear();
        }
        /// <summary>
        /// 任务点的前置依赖是否已完成
        /// </summary>
        /// <returns>依赖任务点是否已完成</returns>
        internal bool IsDependComplete()
        {
            foreach (var point in LastPoints)
            {
                if (!point.IsComplete)
                    return false;
            }
            return true;
        }

#if UNITY_EDITOR
        protected int _width = 200;
        private string _showName = null;
        private bool _isWired = false;
        private bool _isWiredRight = false;
        private Rect _leftWiredOrigin;
        private Rect _rightWiredOrigin;

        /// <summary>
        /// 显示名称
        /// </summary>
        internal string ShowName
        {
            get
            {
                if (string.IsNullOrEmpty(_showName))
                {
                    TaskPointAttribute attribute = GetType().GetCustomAttribute<TaskPointAttribute>();
                    _showName = attribute != null ? attribute.Name : "未定义名称";
                }
                return _showName;
            }
        }
        /// <summary>
        /// 左侧连线点的位置
        /// </summary>
        internal Vector2 LeftPosition
        {
            get
            {
                return new Vector2(Anchor.x + 15, Anchor.y + 32);
            }
        }
        /// <summary>
        /// 右侧连线点的位置
        /// </summary>
        internal Vector2 RightPosition
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width - 15, Anchor.y + 32);
            }
        }
        /// <summary>
        /// 左侧连线点的切线
        /// </summary>
        internal Vector2 LeftTangent
        {
            get
            {
                return new Vector2(Anchor.x - 200, Anchor.y + 32);
            }
        }
        /// <summary>
        /// 右侧连线点的切线
        /// </summary>
        internal Vector2 RightTangent
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width + 200, Anchor.y + 32);
            }
        }
        /// <summary>
        /// 是否选中（在编辑器中）
        /// </summary>
        public bool IsSelected { get; private set; } = false;
        /// <summary>
        /// 是否拖拽中（在编辑器中）
        /// </summary>
        public bool IsDraging { get; private set; } = false;

        /// <summary>
        /// 克隆
        /// </summary>
        public virtual TaskPointBase Clone()
        {
            TaskPointBase taskPoint = Main.Clone(this);
            return taskPoint;
        }
        /// <summary>
        /// 绘制编辑器GUI
        /// </summary>
        internal void OnEditorGUI(TaskContentAsset asset, TaskContentBase content, HTFFunc<string, string> getWord, bool isLockID, bool isExpandOnlySelected, bool isShowFullName)
        {
            if (IsComplete)
            {
                GUI.backgroundColor = Color.green;
            }
            else if (!IsEnable)
            {
                GUI.backgroundColor = Color.gray;
            }
            else
            {
                GUI.backgroundColor = IsSelected ? Color.yellow : Color.white;
            }

            GUILayout.BeginArea(Anchor, ShowName, "Window");

            GUI.backgroundColor = Color.white;

            int height = 30;

            height += OnDependGUI(getWord);

            if (isExpandOnlySelected) IsExpand = IsSelected;

            if (IsExpand)
            {
                height += OnToolbarGUI(asset, content);

                height += OnBaseGUI(getWord, isLockID);

                height += OnPropertyGUI();
            }
            else
            {
                height += OnCollapseGUI(getWord);
            }

            Anchor.width = _width;
            if ((int)Anchor.height != height)
            {
                Anchor.height = height;
                GUI.changed = true;
            }

            GUILayout.EndArea();

            if (IsSelected && isShowFullName)
            {
                GUI.contentColor = Color.yellow;
                GUI.Label(new Rect(Anchor.x, Anchor.y - 30, Anchor.width, 30), ShowName);
                GUI.contentColor = Color.white;
            }

            string icon = IsEnable ? "animationvisibilitytoggleon" : "animationvisibilitytoggleoff";
            GUIContent gUIContent = new GUIContent();
            gUIContent.text = null;
            gUIContent.image = EditorGUIUtility.IconContent(icon).image;
            gUIContent.tooltip = IsEnable ? "Enable" : "Disable";
            if (GUI.Button(new Rect(Anchor.x + Anchor.width - 40, Anchor.y, 20, 20), gUIContent, "InvisibleButton"))
            {
                IsEnable = !IsEnable;
                GUI.changed = true;
            }

            gUIContent.text = null;
            gUIContent.image = EditorGUIUtility.IconContent("Exposure").image;
            gUIContent.tooltip = IsExpand ? "Expand" : "Collapse";
            GUI.enabled = !isExpandOnlySelected;
            if (GUI.Button(new Rect(Anchor.x + Anchor.width - 22, Anchor.y, 20, 20), gUIContent, "InvisibleButton"))
            {
                IsExpand = !IsExpand;
                GUI.changed = true;
            }
            GUI.enabled = true;

            OnWiredGUI();
        }
        /// <summary>
        /// 绘制依赖关系
        /// </summary>
        /// <returns>绘制高度</returns>
        private int OnDependGUI(HTFFunc<string, string> getWord)
        {
            int height = 0;

            GUILayout.BeginHorizontal();

            GUIContent gUIContent = new GUIContent();
            gUIContent.image = EditorGUIUtility.IconContent("DotFrameDotted").image;
            gUIContent.tooltip = getWord("Dependent task point");
            GUILayout.Box(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20));
            _leftWiredOrigin = GUILayoutUtility.GetLastRect();

            GUILayout.FlexibleSpace();

            bool enable = IsEnable && IsEnableRunTime;
            GUI.color = enable ? Color.cyan : Color.red;
            GUILayout.Label(getWord(enable ? "ENABLE" : "DISABLED"));
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();

            gUIContent.tooltip = getWord("Be dependent task point");
            GUILayout.Box(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20));
            _rightWiredOrigin = GUILayoutUtility.GetLastRect();

            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }
        /// <summary>
        /// 绘制工具栏
        /// </summary>
        /// <returns>绘制高度</returns>
        private int OnToolbarGUI(TaskContentAsset asset, TaskContentBase content)
        {
            int height = 0;
            
            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();

            GUIContent gUIContent = new GUIContent();
            gUIContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            gUIContent.tooltip = "Edit Point Script";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                MonoScript monoScript = MonoScript.FromScriptableObject(this);
                AssetDatabase.OpenAsset(monoScript);
            }

            gUIContent.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            gUIContent.tooltip = "Delete";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog("Delete Task Point", "Are you sure you want to delete task point [" + Name + "]?", "Yes", "No"))
                {
                    DeletePoint(asset, content);
                    GUI.changed = true;
                }
            }
            
            GUILayout.EndHorizontal();
            
            height += 20;

            return height;
        }
        /// <summary>
        /// 绘制基础属性
        /// </summary>
        /// <returns>绘制高度</returns>
        private int OnBaseGUI(HTFFunc<string, string> getWord, bool isLockID)
        {
            int height = 0;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("ID") + ":", GUILayout.Width(50));
            if (IsSelected)
            {
                GUI.enabled = !isLockID;
                GUID = EditorGUILayout.TextField(GUID);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.LabelField(GUID);
            }
            GUILayout.EndHorizontal();

            height += 20;

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Name") + ":", GUILayout.Width(50));
            if (IsSelected)
            {
                Name = EditorGUILayout.TextField(Name);
            }
            else
            {
                EditorGUILayout.LabelField(Name);
            }
            GUILayout.EndHorizontal();

            height += 20;

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Details") + ":", GUILayout.Width(50));
            if (IsSelected)
            {
                Details = EditorGUILayout.TextField(Details);
            }
            else
            {
                EditorGUILayout.LabelField(Details);
            }
            GUILayout.EndHorizontal();
            
            height += 20;

            GUILayout.BeginHorizontal();
            GUILayout.Space(55);
            IsDisplayDetails = GUILayout.Toggle(IsDisplayDetails, getWord("Is Display Details"), GUILayout.Width(120));
            GUILayout.EndHorizontal();

            height += 20;

            TaskGameObject.DrawField(Target, getWord("Target") + ":", 50, Anchor.width, getWord("Copy"), getWord("Paste"));

            height += 20;

            return height;
        }
        /// <summary>
        /// 绘制连线
        /// </summary>
        private void OnWiredGUI()
        {
            if (_isWired)
            {
                if (_isWiredRight)
                {
                    Handles.DrawBezier(RightPosition, Event.current.mousePosition, RightTangent, Event.current.mousePosition, Color.yellow, null, 3);
                }
                else
                {
                    Handles.DrawBezier(LeftPosition, Event.current.mousePosition, LeftTangent, Event.current.mousePosition, Color.yellow, null, 3);
                }
            }
        }
        /// <summary>
        /// 绘制收起时GUI
        /// </summary>
        /// <returns>绘制高度</returns>
        private int OnCollapseGUI(HTFFunc<string, string> getWord)
        {
            int height = 0;

            GUILayout.BeginHorizontal();

            GUILayout.Label(getWord("Name") + ":", GUILayout.Width(50));
            EditorGUILayout.LabelField(Name);

            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }
        /// <summary>
        /// 绘制属性GUI
        /// </summary>
        /// <returns>绘制高度</returns>
        protected virtual int OnPropertyGUI()
        {
            return 0;
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        internal void OnPointEventHandle(Event e, TaskContentAsset asset, TaskContentBase content, HTFFunc<string, string> getWord)
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
                            e.Use();
                        }
                        else if (_rightWiredOrigin.Contains(e.mousePosition))
                        {
                            _isWired = true;
                            _isWiredRight = true;
                            e.Use();
                        }
                        else if (Anchor.Contains(e.mousePosition))
                        {
                            IsDraging = true;
                            IsSelected = true;
                            GUI.changed = true;
                            GUI.FocusControl(null);
                        }
                        else
                        {
                            IsSelected = false;
                            GUI.changed = true;
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (IsSelected)
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
                                            e.Use();
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
                                            e.Use();
                                        });
                                    }
                                }
                                gm.ShowAsContext();
                            }
                            else if (Anchor.Contains(e.mousePosition))
                            {
                                GenericMenu gm = new GenericMenu();
                                gm.AddItem(new GUIContent(getWord("Edit Point Script")), false, () =>
                                {
                                    MonoScript monoScript = MonoScript.FromScriptableObject(this);
                                    AssetDatabase.OpenAsset(monoScript);
                                    e.Use();
                                });
                                gm.AddItem(new GUIContent(getWord("Delete Point")), false, () =>
                                {
                                    if (EditorUtility.DisplayDialog("Delete Task Point", $"Are you sure you want to delete task point [{Name}]?", "Yes", "No"))
                                    {
                                        DeletePoint(asset, content);
                                        GUI.changed = true;
                                        e.Use();
                                    }
                                });
                                OnRightClickMenu(gm);
                                gm.ShowAsContext();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (_isWired)
                    {
                        int chooseIndex;
                        if (ChoosePoint(e.mousePosition, content, out chooseIndex))
                        {
                            int originIndex = content.Points.IndexOf(this);
                            if (originIndex != chooseIndex)
                            {
                                if (_isWiredRight)
                                {
                                    if (content.IsExistDepend(chooseIndex, originIndex))
                                        content.DisconnectDepend(chooseIndex, originIndex);
                                    else
                                        content.ConnectDepend(chooseIndex, originIndex);
                                }
                                else
                                {
                                    if (content.IsExistDepend(originIndex, chooseIndex))
                                        content.DisconnectDepend(originIndex, chooseIndex);
                                    else
                                        content.ConnectDepend(originIndex, chooseIndex);
                                }
                            }
                            e.Use();
                        }

                        _isWired = false;
                        GUI.changed = true;
                    }
                    IsDraging = false;
                    break;
                case EventType.MouseDrag:
                    if (_isWired)
                    {
                        GUI.changed = true;
                    }
                    else if (IsDraging)
                    {
                        OnDrag(e.delta);
                        GUI.changed = true;
                    }
                    break;
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.Delete:
                            if (IsSelected)
                            {
                                if (EditorUtility.DisplayDialog("Delete Task Point", $"Are you sure you want to delete task point [{Name}]?", "Yes", "No"))
                                {
                                    DeletePoint(asset, content);
                                    GUI.changed = true;
                                }
                            }
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 拖拽任务点
        /// </summary>
        internal void OnDrag(Vector2 delta)
        {
            Anchor.position += delta;
        }
        /// <summary>
        /// 选中连线任务点
        /// </summary>
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
        /// <summary>
        /// 鼠标右键菜单
        /// </summary>
        /// <param name="gm">菜单</param>
        protected virtual void OnRightClickMenu(GenericMenu gm)
        {

        }

        /// <summary>
        /// 删除任务点
        /// </summary>
        private void DeletePoint(TaskContentAsset asset, TaskContentBase content)
        {
            int index = content.Points.IndexOf(this);
            for (int i = 0; i < content.Depends.Count; i++)
            {
                TaskDepend depend = content.Depends[i];
                if (depend.OriginalPoint == index || depend.DependPoint == index)
                {
                    content.Depends.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    if (depend.OriginalPoint > index)
                    {
                        depend.OriginalPoint -= 1;
                    }
                    if (depend.DependPoint > index)
                    {
                        depend.DependPoint -= 1;
                    }
                }
            }

            content.Points.Remove(this);
            TaskContentAsset.DestroySerializeSubObject(this, asset);
        }
#endif
    }
}