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
    }
}