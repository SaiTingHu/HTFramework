using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤操作的目标
    /// </summary>
    public sealed class StepTarget : MonoBehaviour
    {
        public string GUID = "<None>";
        public StepTargetState State = StepTargetState.Normal;
    }

    /// <summary>
    /// 步骤触发类型
    /// </summary>
    public enum StepTrigger
    {
        /// <summary>
        /// 鼠标点击目标
        /// </summary>
        MouseClick,
        /// <summary>
        /// 目标状态改变
        /// </summary>
        StateChange,
        /// <summary>
        /// 自动执行
        /// </summary>
        AutoExecute
    }

    /// <summary>
    /// 步骤操作目标的状态
    /// </summary>
    public enum StepTargetState
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
