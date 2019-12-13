using System.Net.Sockets;

namespace HT.Framework
{
    /// <summary>
    /// 接收消息助手接口
    /// </summary>
    public interface IReceiveMessageHelper
    {
        /// <summary>
        /// 接收消息
        /// </summary>
        INetworkInfo ReceiveMessage(Socket client);
    }
}