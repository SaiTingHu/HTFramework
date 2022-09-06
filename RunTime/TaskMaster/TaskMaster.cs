﻿using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    [InternalModule(HTFrameworkModule.TaskMaster)]
    public sealed class TaskMaster : InternalModuleBase<ITaskMasterHelper>
    {
        /// <summary>
        /// 任务资源
        /// </summary>
        public TaskContentAsset ContentAsset;
        /// <summary>
        /// 指引目标的高亮方式
        /// </summary>
        [SerializeField] internal MouseRay.HighlightingType GuideHighlighting = MouseRay.HighlightingType.Flash;
        /// <summary>
        /// 默认高亮颜色
        /// </summary>
        [SerializeField] internal Color NormalColor = Color.cyan;
        /// <summary>
        /// 闪光高亮颜色1
        /// </summary>
        [SerializeField] internal Color FlashColor1 = Color.red;
        /// <summary>
        /// 闪光高亮颜色2
        /// </summary>
        [SerializeField] internal Color FlashColor2 = Color.white;
        /// <summary>
        /// 轮廓发光强度
        /// </summary>
        [SerializeField] internal float OutlineIntensity = 1;

        //所有的 任务目标 <任务目标ID、任务目标>
        private Dictionary<string, TaskTarget> _targets = new Dictionary<string, TaskTarget>();
        //所有的 任务内容
        private List<TaskContentBase> _taskContentsList = new List<TaskContentBase>();
        //所有的 任务内容 <任务内容ID、任务内容>
        private Dictionary<string, TaskContentBase> _taskContents = new Dictionary<string, TaskContentBase>();
        //所有的 任务点 <任务点ID、任务点>
        private Dictionary<string, TaskPointBase> _taskPoints = new Dictionary<string, TaskPointBase>();
        //当前的任务内容索引
        private int _currentTaskContentIndex = 0;
        //当前的任务内容
        private TaskContentBase _currentTaskContent;
        //任务控制者运行中
        private bool _running = false;
        //任务控制者暂停中
        private bool _pause = false;

        /// <summary>
        /// 当前是否运行中
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _running;
            }
            private set
            {
                _running = value;
            }
        }
        /// <summary>
        /// 暂停任务控制者
        /// </summary>
        public bool Pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
            }
        }
        /// <summary>
        /// 当前所有的任务内容
        /// </summary>
        public List<TaskContentBase> AllTaskContent
        {
            get
            {
                return _taskContentsList;
            }
        }
        /// <summary>
        /// 当前激活的任务内容索引
        /// </summary>
        public int CurrentTaskContentIndex
        {
            get
            {
                return _currentTaskContentIndex;
            }
            private set
            {
                _currentTaskContentIndex = value;
            }
        }
        /// <summary>
        /// 当前激活的任务内容
        /// </summary>
        public TaskContentBase CurrentTaskContent
        {
            get
            {
                return _currentTaskContent;
            }
            set
            {
                if (_currentTaskContent == value)
                    return;

                if (_currentTaskContent != null)
                {
                    _currentTaskContent.EndContent();
                    _currentTaskContent = null;
                }
                _currentTaskContent = value;
                if (_currentTaskContent != null)
                {
                    CurrentTaskContentIndex = AllTaskContent.IndexOf(_currentTaskContent);
                    _currentTaskContent.StartContent();
                }
                else
                {
                    CurrentTaskContentIndex = 0;
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (Pause)
                return;

            if (IsRunning)
            {
                if (CurrentTaskContent != null)
                {
                    if (CurrentTaskContent.MonitorContent())
                    {
                        CurrentTaskContent.Complete();

                        CurrentTaskContentIndex += 1;
                        if (CurrentTaskContentIndex < _taskContentsList.Count)
                        {
                            CurrentTaskContent = _taskContentsList[CurrentTaskContentIndex];
                        }
                        else
                        {
                            End();
                        }
                    }
                }
            }
        }
        public override void OnTerminate()
        {
            base.OnTerminate();

            _targets.Clear();
            _taskContentsList.Clear();
            _taskContents.Clear();
            _taskPoints.Clear();
        }
        public override void OnPause()
        {
            base.OnPause();

            Pause = true;
        }
        public override void OnResume()
        {
            base.OnResume();

            Pause = false;
        }

        /// <summary>
        /// 根据任务点ID获取任务点的启用标记
        /// </summary>
        /// <param name="stepID">任务点ID</param>
        /// <returns>是否启用</returns>
        public bool TaskPointIsEnable(string pointID)
        {
            if (_taskPoints.ContainsKey(pointID))
            {
                return _taskPoints[pointID].IsEnable && _taskPoints[pointID].IsEnableRunTime;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 通过ID获取任务目标
        /// </summary>
        /// <param name="id">任务目标ID</param>
        /// <returns>任务目标</returns>
        public TaskTarget GetTarget(string id)
        {
            if (_targets.ContainsKey(id))
            {
                return _targets[id];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通过ID获取任务内容
        /// </summary>
        /// <param name="id">任务内容ID</param>
        /// <returns>任务内容</returns>
        public TaskContentBase GetTaskContent(string id)
        {
            if (_taskContents.ContainsKey(id))
            {
                return _taskContents[id];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 随机获取一个当前任务内容中未完成的、已激活的（依赖已完成）任务点
        /// </summary>
        /// <returns>任务点</returns>
        public TaskPointBase GetActivatedTaskPoint()
        {
            if (CurrentTaskContent == null)
                return null;

            return CurrentTaskContent.GetActivatedTaskPoint();
        }

        /// <summary>
        /// 重新编译任务内容，在更改任务资源 ContentAsset 后，必须重新编译一次才可以开始任务流程
        /// </summary>
        /// <param name="disableTaskIDs">禁用的任务点ID集合（当为null时启用所有任务，禁用的任务点不会触发）</param>
        public void RecompileTaskContent(HashSet<string> disableTaskIDs = null)
        {
            if (ContentAsset)
            {
                #region 搜寻任务目标
                //搜寻场景中所有任务目标
                _targets.Clear();
                List<GameObject> rootObjs = new List<GameObject>();
                List<TaskTarget> targetCaches = new List<TaskTarget>();
                GlobalTools.GetRootGameObjectsInAllScene(rootObjs);
                foreach (GameObject rootObj in rootObjs)
                {
                    targetCaches.Clear();
                    rootObj.transform.GetComponentsInChildren(true, targetCaches);
                    for (int i = 0; i < targetCaches.Count; i++)
                    {
                        if (!_targets.ContainsKey(targetCaches[i].GUID))
                        {
                            _targets.Add(targetCaches[i].GUID, targetCaches[i]);
                        }
                        else
                        {
                            Log.Warning(string.Format("任务控制者：发现相同GUID的目标！GUID：{0}\r\n目标物体：{1} 和 {2}", targetCaches[i].GUID, _targets[targetCaches[i].GUID].transform.FullName(), targetCaches[i].transform.FullName()));
                        }
                    }
                }
                #endregion

                #region 判断任务ID是否重复
                _taskContentsList.Clear();
                _taskContents.Clear();
                _taskPoints.Clear();
                for (int i = 0; i < ContentAsset.Content.Count; i++)
                {
                    TaskContentBase content = ContentAsset.Content[i];
                    if (_taskContents.ContainsKey(content.GUID))
                    {
                        Log.Error(string.Format("任务控制者：发现相同GUID的任务内容！GUID：{0}\r\n任务内容：{1} 和 {2}", content.GUID, _taskContents[content.GUID].Name, content.Name));
                    }
                    else
                    {
                        _taskContents.Add(content.GUID, content);
                        _taskContentsList.Add(content);
                    }

                    for (int j = 0; j < content.Points.Count; j++)
                    {
                        TaskPointBase point = content.Points[j];
                        if (_taskPoints.ContainsKey(point.GUID))
                        {
                            Log.Error(string.Format("任务控制者：发现相同GUID的任务点！GUID：{0}\r\n任务点：{1} 和 {2}", point.GUID, _taskPoints[point.GUID].Name, point.Name));
                        }
                        else
                        {
                            _taskPoints.Add(point.GUID, point);
                        }

                        if (_taskContents.ContainsKey(point.GUID))
                        {
                            Log.Error(string.Format("任务控制者：发现相同GUID的任务内容和任务点！GUID：{0}\r\n任务内容：{1} 任务点：{2}", point.GUID, _taskContents[point.GUID].Name, point.Name));
                        }
                    }
                }
                #endregion

                #region 刷新任务状态
                foreach (var item in _taskContents)
                {
                    item.Value.ReSet();
                }
                foreach (var item in _taskPoints)
                {
                    item.Value.ReSet();
                }

                if (disableTaskIDs != null && disableTaskIDs.Count > 0)
                {
                    foreach (var item in disableTaskIDs)
                    {
                        if (_taskPoints.ContainsKey(item))
                        {
                            _taskPoints[item].IsEnableRunTime = false;
                        }
                    }
                }

                foreach (var item in _taskContents)
                {
                    TaskContentBase taskContent = item.Value;
                    for (int i = 0; i < taskContent.Depends.Count; i++)
                    {
                        TaskDepend depend = taskContent.Depends[i];
                        taskContent.Points[depend.OriginalPoint].LastPoints.Add(taskContent.Points[depend.DependPoint]);
                        taskContent.Points[depend.DependPoint].NextPoints.Add(taskContent.Points[depend.OriginalPoint]);
                    }
                }

                CurrentTaskContentIndex = 0;
                CurrentTaskContent = null;
                IsRunning = false;
                Pause = false;
                #endregion
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.TaskMaster, "任务控制者：重新编译任务失败，任务控制者丢失了任务资源 TaskContentAsset！");
            }
        }
        /// <summary>
        /// 开始任务流程
        /// </summary>
        /// <param name="isBeginFirstTask">自动开始第一个任务内容</param>
        public void Begin(bool isBeginFirstTask = true)
        {
            if (!ContentAsset || ContentAsset.Content.Count <= 0 || _taskContentsList.Count <= 0)
            {
                throw new HTFrameworkException(HTFrameworkModule.TaskMaster, "任务控制者：当前无法开始任务流程，请重新编译任务内容 RecompileTaskContent！");
            }

            CurrentTaskContentIndex = 0;
            CurrentTaskContent = null;
            IsRunning = true;
            Pause = false;

            Main.m_Event.Throw<EventTaskBegin>();

            if (isBeginFirstTask)
            {
                CurrentTaskContent = _taskContentsList[CurrentTaskContentIndex];
            }
        }
        /// <summary>
        /// 结束任务流程
        /// </summary>
        public void End()
        {
            CurrentTaskContentIndex = 0;
            CurrentTaskContent = null;
            IsRunning = false;
            Pause = false;

            Main.m_Event.Throw<EventTaskEnd>();
        }

        /// <summary>
        /// 自动完成当前任务内容，任务内容未完成的任务点会根据依赖关系依次调用自动完成
        /// </summary>
        /// <returns>是否成功</returns>
        public bool AutoCompleteCurrentTaskContent()
        {
            if (!IsRunning)
                return false;

            if (Pause)
                return false;

            if (CurrentTaskContent == null)
                return false;

            return CurrentTaskContent.AutoComplete();
        }
        /// <summary>
        /// 自动完成当前任务内容中指定的任务点
        /// </summary>
        /// <param name="id">任务点ID</param>
        /// <returns>是否成功</returns>
        public bool AutoCompleteCurrentTaskPoint(string id)
        {
            if (!IsRunning)
                return false;

            if (Pause)
                return false;

            if (CurrentTaskContent == null)
                return false;

            TaskPointBase taskPoint = CurrentTaskContent.Points.Find((p) => { return p.GUID == id; });
            if (taskPoint != null)
            {
                return taskPoint.AutoComplete();
            }
            return false;
        }
        /// <summary>
        /// 指引当前任务内容中指定的任务点
        /// </summary>
        /// <param name="id">任务点ID</param>
        public void GuideCurrentTaskPoint(string id)
        {
            if (!IsRunning)
                return;

            if (Pause)
                return;

            if (CurrentTaskContent == null)
                return;

            TaskPointBase taskPoint = CurrentTaskContent.Points.Find((p) => { return p.GUID == id; });
            if (taskPoint != null)
            {
                taskPoint.GuidePoint();
            }
        }
        /// <summary>
        /// 指引当前任务内容中指定的任务点
        /// </summary>
        /// <param name="taskPoint">任务点</param>
        public void GuideCurrentTaskPoint(TaskPointBase taskPoint)
        {
            if (!IsRunning)
                return;

            if (Pause)
                return;

            if (CurrentTaskContent == null)
                return;

            if (CurrentTaskContent.Points.Contains(taskPoint) && taskPoint != null)
            {
                taskPoint.GuidePoint();
            }
        }
    }
}