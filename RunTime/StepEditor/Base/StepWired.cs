using System;

namespace HT.Framework
{
    /// <summary>
    /// 步骤操作之间的连线
    /// </summary>
    [Serializable]
    public sealed class StepWired
    {
        public int Left = 0;
        public int Right = 0;
        
#if UNITY_EDITOR
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>新的对象</returns>
        internal StepWired Clone()
        {
            StepWired wired = new StepWired();
            wired.Left = Left;
            wired.Right = Right;
            return wired;
        }
#endif
    }
}