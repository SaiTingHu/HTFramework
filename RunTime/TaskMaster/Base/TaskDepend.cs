using System;

namespace HT.Framework
{
    /// <summary>
    /// 任务依赖
    /// </summary>
    [Serializable]
    public sealed class TaskDepend
    {
        public int OriginalPoint = 0;
        public int DependPoint = 0;

        public TaskDepend(int originalPoint, int dependPoint)
        {
            OriginalPoint = originalPoint;
            DependPoint = dependPoint;
        }
    }
}