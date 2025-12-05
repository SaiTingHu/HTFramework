using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HT.Framework
{
    /// <summary>
    /// 默认的UDP协议通道
    /// </summary>
    public sealed class UdpChannel : ProtocolChannelBase
    {
        private EndPoint _serverEndPoint;
        private byte[] _receiveBuffer = new byte[64 * 1024];

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

            Client.Bind(ClientEndPoint);
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
        /// 发送消息
        /// </summary>
        /// <param name="info">封装后的消息字节数组</param>
        /// <returns>已发送字节</returns>
        public override async Task<int> SendMessage(byte[] info)
        {
            if (info == null || info.Length == 0)
                return 0;

            int count = 0;
            try
            {
                count = await Client.SendToAsync(info, SocketFlags.None, ServerEndPoint);
            }
            catch (Exception e)
            {
                Log.Error($"{this} 发送消息出错：{e.Message}");
                count = 0;
            }
            return count;
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
                if (_serverEndPoint == null) _serverEndPoint = ServerEndPoint;

                int length = client.ReceiveFrom(_receiveBuffer, ref _serverEndPoint);

                UdpNetworkInfo info = Main.m_ReferencePool.Spawn<UdpNetworkInfo>();
                info.Message = Encoding.UTF8.GetString(_receiveBuffer, 0, length);
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}