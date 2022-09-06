using UnityEngine;
using UnityEngine.Events;

namespace HT.Framework
{
    /// <summary>
    /// 任务的目标
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TaskTarget : HTBehaviour
    {
        /// <summary>
        /// 任务目标ID
        /// </summary>
        public string GUID = "<None>";
        /// <summary>
        /// 任务目标状态
        /// </summary>
        public TaskTargetState State = TaskTargetState.Normal;
        /// <summary>
        /// 以此为目标物体的任务点执行【完成】回调时所需的时间
        /// </summary>
        public float CompletingTime = 0;
        /// <summary>
        /// 当以此为目标物体的任务点【完成】时回调
        /// </summary>
        public UnityEvent OnTaskPointComplete;
        /// <summary>
        /// 当以此为目标物体的任务点【自动完成】时回调
        /// </summary>
        public UnityEvent OnTaskPointAutoComplete;
    }

    /// <summary>
    /// 任务目标的状态
    /// </summary>
    public enum TaskTargetState
    {
        /// <summary>
        /// 常态
        /// </summary>
        Normal,
        /// <summary>
        /// 已完成
        /// </summary>
        Done
    }
}