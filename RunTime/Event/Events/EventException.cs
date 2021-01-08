using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常事件（三型事件）
    /// </summary>
    public sealed class EventException : EventHandlerBase
    {
        public string LogString;
        public string StackTrace;
        public LogType TheType;

        /// <summary>
        /// 填充数据，所有属性、字段的初始化工作可以在这里完成
        /// </summary>
        public EventException Fill(string logString, string stackTrace, LogType type)
        {
            LogString = logString;
            StackTrace = stackTrace;
            TheType = type;
            return this;
        }
        /// <summary>
        /// 重置引用，当被引用池回收时调用
        /// </summary>
        public override void Reset()
        {
            LogString = "";
            StackTrace = "";
            TheType = LogType.Error;
        }
    }
}
