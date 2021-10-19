using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务内容基类
    /// </summary>
    public abstract class TaskContentBase : ScriptableObject
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [SerializeField] internal string GUID = "";
        /// <summary>
        /// 任务名称
        /// </summary>
        [SerializeField] internal string Name = "";
        /// <summary>
        /// 任务细节
        /// </summary>
        [SerializeField] internal string Details = "";
        /// <summary>
        /// 任务目标
        /// </summary>
        [SerializeField] internal TaskGameObject Target = new TaskGameObject();
        /// <summary>
        /// 所有任务点
        /// </summary>
        [SerializeField] internal List<TaskPointBase> Points = new List<TaskPointBase>();
        /// <summary>
        /// 所有任务点依赖
        /// </summary>
        [SerializeField] internal List<TaskDepend> Depends = new List<TaskDepend>();

        /// <summary>
        /// 获取任务ID
        /// </summary>
        public string GetID
        {
            get
            {
                return GUID;
            }
        }
        /// <summary>
        /// 获取任务名称
        /// </summary>
        public string GetName
        {
            get
            {
                return Name;
            }
        }
        /// <summary>
        /// 获取任务细节
        /// </summary>
        public string GetDetails
        {
            get
            {
                return Details;
            }
        }
        /// <summary>
        /// 获取任务目标
        /// </summary>
        public GameObject GetTarget
        {
            get
            {
                return Target.Entity;
            }
        }
        /// <summary>
        /// 获取所有任务点
        /// </summary>
        public List<TaskPointBase> GetPoints
        {
            get
            {
                return Points;
            }
        }
        /// <summary>
        /// 是否开始
        /// </summary>
        public bool IsStart { get; private set; } = false;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; private set; } = false;

        /// <summary>
        /// 任务内容开始
        /// </summary>
        protected virtual void OnStart()
        {
            
        }
        /// <summary>
        /// 任务内容开始后，帧刷新
        /// </summary>
        protected virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 任务内容完成
        /// </summary>
        protected virtual void OnComplete()
        {
            
        }
        
        /// <summary>
        /// 任务内容完成
        /// </summary>
        internal void Complete()
        {
            if (!IsStart)
            {
                IsStart = true;

                OnStart();

                Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskContentStart>().Fill(this, false));
            }

            if (IsComplete)
                return;

            IsComplete = true;

            OnComplete();
            
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskContentComplete>().Fill(this, false));
        }
        /// <summary>
        /// 任务内容自动完成（未完成的任务点会自动完成）
        /// </summary>
        /// <returns>是否成功</returns>
        internal bool AutoComplete()
        {
            if (!IsStart)
            {
                IsStart = true;

                OnStart();

                Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskContentStart>().Fill(this, true));
            }

            if (IsComplete)
                return false;

            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].IsCompleting)
                    return false;
            }

            IsComplete = true;

            OnComplete();

            List<TaskPointBase> uncompletePoints = new List<TaskPointBase>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (!Points[i].IsComplete && !Points[i].IsCompleting)
                {
                    uncompletePoints.Add(Points[i]);
                }
            }
            while (uncompletePoints.Count > 0)
            {
                for (int i = 0; i < uncompletePoints.Count; i++)
                {
                    if (uncompletePoints[i].IsDependComplete())
                    {
                        uncompletePoints[i].AutoComplete();
                        uncompletePoints.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskContentComplete>().Fill(this, true));
            return true;
        }
        /// <summary>
        /// 任务内容开始后，每帧监测
        /// </summary>
        /// <returns>所有任务点是否已完成</returns>
        internal bool OnMonitor()
        {
            if (!IsStart)
            {
                IsStart = true;

                OnStart();

                Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventTaskContentStart>().Fill(this, false));
            }

            OnUpdate();

            bool isComplete = true;
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].IsComplete)
                    continue;

                isComplete = false;

                if (Points[i].IsCompleting)
                    continue;

                if (Points[i].IsDependComplete())
                {
                    if (Points[i].IsEnable && Points[i].IsEnableRunTime)
                    {
                        Points[i].OnMonitor();
                    }
                    else
                    {
                        Points[i].AutoComplete();
                    }
                }
            }
            return isComplete;
        }
        /// <summary>
        /// 重置状态
        /// </summary>
        internal void ReSet()
        {
            IsStart = false;
            IsComplete = false;
        }
        /// <summary>
        /// 获取一个未完成的、已激活的（前置依赖已完成）任务点
        /// </summary>
        /// <returns>任务点</returns>
        internal TaskPointBase GetActivatedTaskPoint()
        {
            for (int i = 0; i < Points.Count; i++)
            {
                TaskPointBase point = Points[i];
                if (!point.IsEnable || !point.IsEnableRunTime || point.IsComplete || point.IsCompleting)
                    continue;

                if (IsDependComplete(point))
                {
                    return point;
                }
            }
            return null;
        }
        /// <summary>
        /// 任务点的深层前置依赖是否已完成（忽略未启用的）
        /// </summary>
        internal bool IsDependComplete(TaskPointBase taskPoint)
        {
            foreach (var point in taskPoint.LastPoints)
            {
                if (point.IsEnable && point.IsEnableRunTime && !point.IsComplete)
                {
                    return false;
                }
                else
                {
                    if (point.LastPoints.Count > 0)
                    {
                        bool value = IsDependComplete(point);
                        if (!value) return false;
                    }
                }
            }
            return true;
        }

#if UNITY_EDITOR
        private static readonly int _width = 200;
        private int _height = 0;

        /// <summary>
        /// 克隆
        /// </summary>
        public virtual TaskContentBase Clone()
        {
            TaskContentBase taskContent = Main.Clone(this);
            return taskContent;
        }
        /// <summary>
        /// 绘制编辑器GUI
        /// </summary>
        internal void OnEditorGUI(TaskContentAsset asset, HTFFunc<string, string> getWord, bool isLockID)
        {
            GUILayout.BeginVertical("ChannelStripBg", GUILayout.Width(_width), GUILayout.Height(_height));

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Task Content Property") + ":");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            int height = 30;

            height += OnBaseGUI(getWord, isLockID);

            height += OnPropertyGUI();

            if (_height != height)
            {
                _height = height;
                GUI.changed = true;
            }

            GUILayout.EndVertical();
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
            GUI.enabled = !isLockID;
            GUID = EditorGUILayout.TextField(GUID);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            height += 20;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Name") + ":", GUILayout.Width(50));
            Name = EditorGUILayout.TextField(Name);
            GUILayout.EndHorizontal();

            height += 20;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Details") + ":", GUILayout.Width(50));
            Details = EditorGUILayout.TextField(Details);
            GUILayout.EndHorizontal();

            height += 20;
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Points") + ":", GUILayout.Width(50));
            GUILayout.Label(Points.Count.ToString());
            int disabled = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                if (!Points[i].IsEnable || !Points[i].IsEnableRunTime)
                {
                    disabled += 1;
                }
            }
            if (disabled > 0)
            {
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                GUILayout.Label(getWord("DISABLED") + ":" + disabled.ToString());
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();

            height += 20;

            TaskGameObject.DrawField(Target, getWord("Target") + ":", 50, _width, getWord("Copy"), getWord("Paste"));

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
        /// 是否存在依赖关系
        /// </summary>
        /// <param name="originalPoint">原始任务点</param>
        /// <param name="dependPoint">依赖的任务点</param>
        /// <returns>是否存在</returns>
        internal bool IsExistDepend(int originalPoint, int dependPoint)
        {
            for (int i = 0; i < Depends.Count; i++)
            {
                if (Depends[i].OriginalPoint == originalPoint && Depends[i].DependPoint == dependPoint)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 断开依赖关系
        /// </summary>
        /// <param name="originalPoint">原始任务点</param>
        /// <param name="dependPoint">依赖的任务点</param>
        internal void DisconnectDepend(int originalPoint, int dependPoint)
        {
            for (int i = 0; i < Depends.Count; i++)
            {
                if (Depends[i].OriginalPoint == originalPoint && Depends[i].DependPoint == dependPoint)
                {
                    Depends.RemoveAt(i);
                    i -= 1;
                }
            }
        }
        /// <summary>
        /// 建立依赖关系
        /// </summary>
        /// <param name="originalPoint">原始任务点</param>
        /// <param name="dependPoint">依赖的任务点</param>
        internal void ConnectDepend(int originalPoint, int dependPoint)
        {
            Depends.Add(new TaskDepend(originalPoint, dependPoint));
        }
#endif
    }
}