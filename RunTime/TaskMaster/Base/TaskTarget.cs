using UnityEngine;

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