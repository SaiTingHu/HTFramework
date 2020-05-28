namespace HT.Framework
{
    /// <summary>
    /// 调试器日志
    /// </summary>
    internal sealed class DebuggerConsoleLog : IReference
    {
        public string Name;
        public string Time;
        public string Type;
        public string Message;
        public string StackTrace;

        public void Reset()
        {
            
        }
    }
}