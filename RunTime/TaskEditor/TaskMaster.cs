using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务控制器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.TaskEditor)]
    public sealed class TaskMaster : InternalModuleBase
    {
        /// <summary>
        /// 任务资源
        /// </summary>
        public TaskContentAsset ContentAsset;
        /// <summary>
        /// 是否是自动切换状态，当完成当前任务内容时，自动根据列表顺序切换到下一个任务内容【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsAutoChange = true;

        //所有的 TaskTarget <任务目标ID、任务目标>
        private Dictionary<string, TaskTarget> _targets = new Dictionary<string, TaskTarget>();
        //所有的 任务内容 <任务ID、任务内容>
        private Dictionary<string, TaskContentBase> _taskContents = new Dictionary<string, TaskContentBase>();
        //所有的 任务点 <任务ID、任务点>
        private Dictionary<string, TaskPointBase> _taskPoints = new Dictionary<string, TaskPointBase>();
        //当前的任务内容索引
        private int _currentContentIndex = -1;
        //当前的任务内容
        private TaskContentBase _currentContent;
        //任务控制者运行中
        private bool _running = false;

        internal override void OnRefresh()
        {
            base.OnRefresh();

            if (_running)
            {
                if (_currentContent != null)
                {
                    _currentContent.OnMonitor();

                    if (_currentContent.IsComplete)
                    {
                        CompleteCurrentTask();
                    }
                }
            }
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _targets.Clear();
            _taskContents.Clear();
            _taskPoints.Clear();
        }

        /// <summary>
        /// 当前是否运行中
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _running;
            }
        }
        /// <summary>
        /// 当前是否是自动切换状态
        /// </summary>
        public bool IsAutoChangeState
        {
            get
            {
                return IsAutoChange;
            }
        }
        /// <summary>
        /// 当前所有的任务内容
        /// </summary>
        public List<TaskContentBase> AllTaskContent
        {
            get
            {
                return ContentAsset.Content;
            }
        }
        /// <summary>
        /// 当前所有未完成的任务内容数量
        /// </summary>
        public int AllUncompleteTaskCount
        {
            get
            {
                int count = 0;
                foreach (var task in _taskContents)
                {
                    if (!task.Value.IsComplete)
                    {
                        count += 1;
                    }
                }
                return count;
            }
        }
        /// <summary>
        /// 当前任务内容
        /// </summary>
        public TaskContentBase CurrentTaskContent
        {
            get
            {
                return _currentContent;
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
        /// 通过ID获取任务点
        /// </summary>
        /// <param name="id">任务点ID</param>
        /// <returns>任务点</returns>
        public TaskPointBase GetTaskPoint(string id)
        {
            if (_taskPoints.ContainsKey(id))
            {
                return _taskPoints[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 通过ID设置当前激活的任务内容
        /// </summary>
        /// <param name="id">任务内容ID</param>
        public void SetCurrentTaskContent(string id)
        {
            if (_running)
            {
                if (_taskContents.ContainsKey(id))
                {
                    _currentContentIndex = ContentAsset.Content.IndexOf(_taskContents[id]);
                    BeginCurrentTask();
                }
                else
                {
                    Log.Warning("任务控制者：设置当前激活的任务内容失败，当前并不存在ID为 " + id + " 的任务内容！");
                }
            }
        }
        /// <summary>
        /// 通过索引设置当前激活的任务内容
        /// </summary>
        /// <param name="index">任务内容索引</param>
        public void SetCurrentTaskContent(int index)
        {
            if (_running)
            {
                if (index >= 0 && index < ContentAsset.Content.Count)
                {
                    _currentContentIndex = index;
                    BeginCurrentTask();
                }
                else
                {
                    Log.Warning("任务控制者：设置当前激活的任务内容失败，当前并不存在索引为 " + index + " 的任务内容！");
                }
            }
        }
        /// <summary>
        /// 切换到下一个任务内容，根据任务内容列表顺序
        /// </summary>
        public void ChangeNextTaskContent()
        {
            if (_running)
            {
                if (_currentContentIndex < ContentAsset.Content.Count - 1)
                {
                    _currentContentIndex += 1;
                }
                else
                {
                    _currentContentIndex = 0;
                }
                BeginCurrentTask();
            }
        }
        /// <summary>
        /// 完成当前任务内容，任务内容未完成的任务点会根据依赖关系依次调用自动完成
        /// </summary>
        public void CompleteCurrentTaskContent()
        {
            if (_running)
            {
                if (_currentContent != null)
                {
                    _currentContent.OnAutoComplete();
                }
            }
        }
        /// <summary>
        /// 完成指定的任务内容，任务内容未完成的任务点会根据依赖关系依次调用自动完成
        /// </summary>
        public void CompleteTaskContent(string id)
        {
            if (_running)
            {
                TaskContentBase taskContent = GetTaskContent(id);
                if (taskContent != null)
                {
                    taskContent.OnAutoComplete();
                }
            }
        }
        /// <summary>
        /// 完成指定的任务点
        /// </summary>
        /// <param name="id">任务点ID</param>
        /// <param name="completeAction">完成后执行的操作</param>
        public void CompleteTaskPoint(string id, HTFAction completeAction = null)
        {
            if (_running)
            {
                TaskPointBase taskPoint = GetTaskPoint(id);
                if (taskPoint != null)
                {
                    taskPoint.Complete(completeAction);
                }
            }
        }

        /// <summary>
        /// 重新编译任务内容，在更改任务资源 ContentAsset 后，必须重新编译一次才可以开始任务流程
        /// </summary>
        /// <param name="disableTaskIDs">禁用的任务点ID集合（当为null时启用所有任务，禁用的任务不会触发）</param>
        public void RecompileTaskContent(HashSet<string> disableTaskIDs = null)
        {
            if (ContentAsset)
            {
                #region 搜寻任务目标
                _targets.Clear();
                //搜寻框架下所有任务目标
                List<TaskTarget> targetCaches = new List<TaskTarget>();
                Main.Current.transform.GetComponentsInChildren(true, targetCaches);
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
                //搜寻场景中所有任务目标
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
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
                            _taskPoints[item].IsEnable = false;
                        }
                    }
                }

                _currentContentIndex = 0;
                _currentContent = null;
                _running = false;
                #endregion
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.TaskEditor, "任务控制者：重新编译任务失败，任务控制者丢失了任务资源 TaskContentAsset！");
            }
        }
        /// <summary>
        /// 开始任务流程
        /// </summary>
        public void Begin()
        {
            if (!ContentAsset || ContentAsset.Content.Count <= 0 || _taskContents.Count <= 0)
            {
                throw new HTFrameworkException(HTFrameworkModule.TaskEditor, "任务控制者：当前无法开始任务流程，请重新编译任务内容 RecompileTaskContent！");
            }

            _currentContentIndex = 0;
            _currentContent = null;
            _running = true;

            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskBegin>());

            if (IsAutoChangeState)
            {
                BeginCurrentTask();
            }
        }
        /// <summary>
        /// 结束任务流程
        /// </summary>
        public void End()
        {
            _currentContentIndex = 0;
            _currentContent = null;
            _running = false;

            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskEnd>());
        }

        //当前任务开始
        private void BeginCurrentTask()
        {
            _currentContent = ContentAsset.Content[_currentContentIndex];
            _currentContent.OnStart();

            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskContentStart>().Fill(_currentContent));
        }
        //当前任务完成
        private void CompleteCurrentTask()
        {
            _currentContent.OnComplete();

            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventTaskContentComplete>().Fill(_currentContent));
            _currentContent = null;

            if (AllUncompleteTaskCount > 0)
            {
                if (IsAutoChangeState)
                {
                    if (_currentContentIndex < ContentAsset.Content.Count - 1)
                    {
                        _currentContentIndex += 1;
                    }
                    else
                    {
                        _currentContentIndex = 0;
                    }
                    BeginCurrentTask();
                }
            }
            else
            {
                End();
            }
        }
    }
}