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
        public string GUID = "";
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 任务细节
        /// </summary>
        public string Details = "";
        /// <summary>
        /// 任务目标
        /// </summary>
        public TaskGameObject Target = new TaskGameObject();
        /// <summary>
        /// 所有任务点
        /// </summary>
        public List<TaskPointBase> Points = new List<TaskPointBase>();
        /// <summary>
        /// 所有任务点依赖
        /// </summary>
        public List<TaskDepend> Depends = new List<TaskDepend>();

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
        /// 任务内容开始
        /// </summary>
        internal void Start()
        {
            OnStart();
        }
        /// <summary>
        /// 任务内容完成
        /// </summary>
        internal void Complete()
        {
            OnComplete();
        }
        /// <summary>
        /// 任务内容自动完成（未完成的任务点）
        /// </summary>
        internal void AutoComplete()
        {
            if (IsComplete)
                return;

            List<TaskPointBase> uncompletePoints = new List<TaskPointBase>();
            List<int> uncompletePointIndexs = new List<int>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].IsEnable && !Points[i].IsComplete && !Points[i].IsCompleting)
                {
                    uncompletePoints.Add(Points[i]);
                    uncompletePointIndexs.Add(i);
                }
            }
            while (uncompletePoints.Count > 0)
            {
                for (int i = 0; i < uncompletePoints.Count; i++)
                {
                    if (IsDependComplete(uncompletePointIndexs[i]))
                    {
                        uncompletePoints[i].AutoComplete();
                        uncompletePoints.RemoveAt(i);
                        uncompletePointIndexs.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
            IsComplete = true;
        }
        /// <summary>
        /// 任务内容开始后，每帧监测
        /// </summary>
        internal void OnMonitor()
        {
            OnUpdate();

            bool isComplete = true;
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].IsEnable && !Points[i].IsComplete && !Points[i].IsCompleting)
                {
                    isComplete = false;

                    if (IsDependComplete(i))
                    {
                        Points[i].OnMonitor();
                    }
                }
            }
            IsComplete = isComplete;
        }
        /// <summary>
        /// 重置状态
        /// </summary>
        internal void ReSet()
        {
            IsComplete = false;
        }
        /// <summary>
        /// 任务点的所有依赖任务点是否已完成
        /// </summary>
        /// <param name="taskPointIndex">任务点</param>
        /// <returns>依赖任务点是否已完成</returns>
        internal bool IsDependComplete(int taskPointIndex)
        {
            for (int i = 0; i < Depends.Count; i++)
            {
                if (Depends[i].OriginalPoint == taskPointIndex)
                {
                    int depend = Depends[i].DependPoint;
                    if (Points[depend].IsEnable && !Points[depend].IsComplete)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

#if UNITY_EDITOR
        private static readonly int _width = 200;
        private int _height = 0;

        /// <summary>
        /// 绘制编辑器GUI
        /// </summary>
        internal void OnEditorGUI(TaskContentAsset asset, HTFFunc<string, string> getWord)
        {
            GUILayout.BeginVertical("ChannelStripBg", GUILayout.Width(_width), GUILayout.Height(_height));

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Task Content Property") + ":");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            _height = 20;

            _height += OnBaseGUI(getWord);

            _height += OnPropertyGUI();

            GUILayout.EndVertical();
        }
        /// <summary>
        /// 绘制基础属性
        /// </summary>
        /// <returns>绘制高度</returns>
        private int OnBaseGUI(HTFFunc<string, string> getWord)
        {
            int height = 0;

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("ID") + ":", GUILayout.Width(50));
            GUID = EditorGUILayout.TextField(GUID);
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