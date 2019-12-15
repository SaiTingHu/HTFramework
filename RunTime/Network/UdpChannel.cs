using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 默认的UDP协议通道
    /// </summary>
    public sealed class UdpChannel : ProtocolChannel
    {
        /// <summary>
        /// 通信协议
        /// </summary>
        public override ProtocolType Protocol
        {
            get
            {
                return ProtocolType.Udp;
            }
        }
        /// <summary>
        /// 是否需要保持连接
        /// </summary>
        public override bool IsNeedConnect
        {
            get
            {
                return false;
            }
        }

        private EndPoint _serverEndPoint;

        /// <summary>
        /// 初始化通道
        /// </summary>
        public override void OnInitialization()
        {
            base.OnInitialization();

            Client.Bind(Main.m_Network.ClientEndPoint);
            _serverEndPoint = Main.m_Network.ServerEndPoint;
        }
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="info">消息对象</param>
        /// <returns>封装后的字节数组</returns>
        public override byte[] OnEncapsulatedMessage(INetworkInfo info)
        {
            UdpNetworkInfo networkInfo = info as UdpNetworkInfo;
            byte[] bytes = Encoding.UTF8.GetBytes(networkInfo.Message);
            return bytes;
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>接收到的消息对象</returns>
        protected override INetworkInfo OnReceiveMessage(Socket client)
        {
            byte[] buffer = new byte[1024];
            int length = client.ReceiveFrom(buffer, ref _serverEndPoint);

            UdpNetworkInfo info = new UdpNetworkInfo();
            info.Message = Encoding.UTF8.GetString(buffer, 0, length);
            return info;
        }
    }
}