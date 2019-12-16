namespace HT.Framework
{
    /// <summary>
    /// 默认的Udp协议网络消息
    /// </summary>
    public sealed class UdpNetworkInfo : INetworkMessage
    {
        public string Message;
        
        public UdpNetworkInfo(string message)
        {
            Message = message;
        }

        public UdpNetworkInfo()
        {
        }
    }
}