using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 异常信息
    /// </summary>
    public sealed class ExceptionInfo : IReference
    {
        /// <summary>
        /// 异常类型
        /// </summary>
        public LogType Type;
        /// <summary>
        /// 异常日志
        /// </summary>
        public string LogString;
        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        public string StackTrace;

        public ExceptionInfo Fill(string logString, string stackTrace, LogType type)
        {
            Type = type;
            LogString = logString;
            StackTrace = stackTrace;
            return this;
        }
        public void Reset()
        {
            Type = LogType.Error;
            LogString = "";
            StackTrace = "";
        }
    }
}