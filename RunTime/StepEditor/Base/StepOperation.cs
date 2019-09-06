using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤操作
    /// </summary>
    [Serializable]
    public sealed partial class StepOperation
    {
        /// <summary>
        /// 操作ID
        /// </summary>
        public string GUID = "";
        public Vector2 Anchor = Vector2.zero;
        /// <summary>
        /// 操作执行到进入下一个操作的时间
        /// </summary>
        public float ElapseTime = 1;
        /// <summary>
        /// 立即执行模式
        /// </summary>
        public bool Instant = false;
        /// <summary>
        /// 操作目标
        /// </summary>
        public GameObject Target = null;
        public string TargetGUID = "<None>";
        public string TargetPath = "<None>";
        /// <summary>
        /// 操作简述名称
        /// </summary>
        public string Name = "New Step Operation";
        /// <summary>
        /// 操作类型
        /// </summary>
        public StepOperationType OperationType = StepOperationType.Move;
    }
}