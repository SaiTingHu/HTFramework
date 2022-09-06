using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤的目标
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StepTarget : HTBehaviour
    {
        /// <summary>
        /// 步骤目标ID
        /// </summary>
        public string GUID = "<None>";
        /// <summary>
        /// 步骤目标状态
        /// </summary>
        public StepTargetState State = StepTargetState.Normal;
    }

    /// <summary>
    /// 步骤触发类型
    /// </summary>
    public enum StepTrigger
    {
        /// <summary>
        /// 鼠标点击目标触发
        /// </summary>
        MouseClick,
        /// <summary>
        /// 鼠标点击UGUI按钮触发
        /// </summary>
        ButtonClick,
        /// <summary>
        /// 目标状态变为Done时触发
        /// </summary>
        StateChange,
        /// <summary>
        /// 自动触发
        /// </summary>
        AutoExecute
    }

    /// <summary>
    /// 步骤目标的状态
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