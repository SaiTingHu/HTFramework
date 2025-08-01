using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 默认的Udp协议网络消息
    /// </summary>
    public sealed class UdpNetworkInfo : INetworkMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message;

        /// <summary>
        /// 封装消息为字节数组
        /// </summary>
        /// <returns>封装后的字节数组</returns>
        public byte[] Encapsulate()
        {
            byte[] bytes = string.IsNullOrEmpty(Message) ? null : Encoding.UTF8.GetBytes(Message);
            return bytes;
        }
        public void Reset()
        {

        }
    }
}