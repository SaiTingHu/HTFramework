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
        /// <summary>
        /// 节点的锚点
        /// </summary>
        [SerializeField] internal Vector2 Anchor = Vector2.zero;
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
        [SerializeField] internal string TargetGUID = "<None>";
        [SerializeField] internal string TargetPath = "<None>";
        /// <summary>
        /// 操作简述名称
        /// </summary>
        public string Name = "New Step Operation";
        /// <summary>
        /// 操作类型
        /// </summary>
        public StepOperationType OperationType = StepOperationType.Move;

#if UNITY_EDITOR
        internal static float Width = 150;
        internal static float Height = 40;
        
        internal Rect Position
        {
            get
            {
                return new Rect(Anchor.x - Width / 2, Anchor.y - Height / 2, Width, Height);
            }
        }
        internal Rect LeftPosition
        {
            get
            {
                return new Rect(Anchor.x - Width / 2 - 10, Anchor.y - 10, 20, 20);
            }
        }
        internal Rect RightPosition
        {
            get
            {
                return new Rect(Anchor.x + Width / 2 - 10, Anchor.y - 10, 20, 20);
            }
        }
#endif
    }
}