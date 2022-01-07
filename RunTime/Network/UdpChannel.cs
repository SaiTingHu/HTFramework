using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 默认的UDP协议通道
    /// </summary>
    public sealed class UdpChannel : ProtocolChannelBase
    {
        private EndPoint _serverEndPoint;

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
        /// 通道类型
        /// </summary>
        public override SocketType Way
        {
            get
            {
                return SocketType.Dgram;
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
        
        /// <summary>
        /// 初始化通道
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();

            Client.Bind(Main.m_Network.ClientEndPoint);
        }
        /// <summary>
        /// 是否是断开连接请求
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>是否是断开连接请求</returns>
        public override bool IsDisconnectRequest(INetworkMessage message)
        {
            return false;
        }
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>封装后的字节数组</returns>
        public override byte[] EncapsulatedMessage(INetworkMessage message)
        {
            UdpNetworkInfo networkInfo = message as UdpNetworkInfo;
            byte[] bytes = Encoding.UTF8.GetBytes(networkInfo.Message);
            return bytes;
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>接收到的消息对象</returns>
        protected override INetworkMessage ReceiveMessage(Socket client)
        {
            try
            {
                if (_serverEndPoint == null) _serverEndPoint = new IPEndPoint(IPAddress.Parse(Main.m_Network.ServerIP), Main.m_Network.ServerPort);

                byte[] buffer = new byte[1024];
                int length = client.ReceiveFrom(buffer, ref _serverEndPoint);

                UdpNetworkInfo info = new UdpNetworkInfo();
                info.Message = Encoding.UTF8.GetString(buffer, 0, length);
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}