using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务依赖
    /// </summary>
    [Serializable]
    public sealed class TaskDepend
    {
        [SerializeField] internal int OriginalPoint = 0;
        [SerializeField] internal int DependPoint = 0;

        public TaskDepend(int originalPoint, int dependPoint)
        {
            OriginalPoint = originalPoint;
            DependPoint = dependPoint;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 克隆
        /// </summary>
        public TaskDepend Clone()
        {
            TaskDepend depend = new TaskDepend(OriginalPoint, DependPoint);
            return depend;
        }
#endif
    }
}