using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常事件
    /// </summary>
    public sealed class EventException : EventHandlerBase
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        public string LogString { get; private set; }
        /// <summary>
        /// 堆栈跟踪信息
        /// </summary>
        public string StackTrace { get; private set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType TheType { get; private set; }

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
