namespace HT.Framework
{
    /// <summary>
    /// 默认的Udp协议网络消息
    /// </summary>
    public sealed class UdpNetworkInfo : INetworkInfo
    {
        public string Message;

        /// <summary>
        /// 填充消息对象
        /// </summary>
        public void Fill(string message)
        {
            Message = message;
        }
    }
}