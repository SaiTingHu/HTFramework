using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的TCP协议通道
    /// </summary>
    public sealed class TcpChannel : ProtocolChannelBase
    {
        /// <summary>
        /// 心跳包校验码
        /// </summary>
        public const int HEARTBEAT = 0;
        /// <summary>
        /// 常规信息校验码
        /// </summary>
        public const int NORMAL = 65536;

        private byte[] _heartbeatPackage;
        private byte[] _receiveBuffer = new byte[256];
        private byte[] _receiveCodeData = new byte[4];
        private byte[] _receiveHeadData = new byte[28];

        /// <summary>
        /// 通信协议
        /// </summary>
        public override ProtocolType Protocol
        {
            get
            {
                return ProtocolType.Tcp;
            }
        }
        /// <summary>
        /// 通道类型
        /// </summary>
        public override SocketType Way
        {
            get
            {
                return SocketType.Stream;
            }
        }
        /// <summary>
        /// 是否需要保持连接
        /// </summary>
        public override bool IsNeedConnect
        {
            get
            {
                return true;
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _heartbeatPackage = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(HEARTBEAT));
        }
        /// <summary>
        /// 是否是断开连接请求
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>是否是断开连接请求</returns>
        public override bool IsDisconnectRequest(INetworkMessage message)
        {
            TcpNetworkInfo networkInfo = message as TcpNetworkInfo;
            if (networkInfo != null)
            {
                if (networkInfo.Command == -1 && networkInfo.Subcommand == -1)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>封装后的字节数组</returns>
        public override byte[] EncapsulatedMessage(INetworkMessage message)
        {
            return message.Encapsulate();
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
                //接收消息校验码（4字节）
                ReceiveFixedBytes(client, _receiveBuffer, _receiveCodeData);

                int code = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveCodeData, 0));
                //通过消息校验码，判断当前接收到的是心跳包
                if (code == HEARTBEAT)
                {
                    //响应心跳包
#if UNITY_EDITOR
                    //Log.Info($"{this}：接收到心跳包【校验码：{code}】，已响应心跳包！[{DateTime.Now}]");
#endif
                    InjectMessage(_heartbeatPackage);
                    return null;
                }
                //通过消息校验码，判断当前接收到的是常规信息
                else if (code == NORMAL)
                {
                    //接收消息头（消息体长度4字节 + 身份ID8字节 + 主命令4字节 + 子命令4字节 + 加密方式4字节 + 返回码4字节 = 28字节）
                    ReceiveFixedBytes(client, _receiveBuffer, _receiveHeadData);

                    //接收消息体（消息体的长度存储在消息头的0-4索引位置的字节里）
                    int recvBodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveHeadData, 0));
                    byte[] receiveBodyData = new byte[recvBodyLength];
                    ReceiveFixedBytes(client, _receiveBuffer, receiveBodyData);

                    //解析消息
                    TcpNetworkInfo info = Main.m_ReferencePool.Spawn<TcpNetworkInfo>();
                    info.CheckCode = code;
                    info.Sessionid = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_receiveHeadData, 4));
                    info.Command = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveHeadData, 12));
                    info.Subcommand = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveHeadData, 16));
                    info.Encrypt = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveHeadData, 20));
                    info.ReturnCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveHeadData, 24));
                    info.Message = receiveBodyData;
                    return info;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 接收固定长度的字节
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="receiveBuffer">接收缓冲区</param>
        /// <param name="destinationBytes">输出到目标字节数组</param>
        private void ReceiveFixedBytes(Socket client, byte[] receiveBuffer, byte[] destinationBytes)
        {
            int recvLength = destinationBytes.Length;
            while (recvLength > 0)
            {
                int recved = client.Receive(receiveBuffer, Mathf.Min(recvLength, receiveBuffer.Length), SocketFlags.None);
                Array.Copy(receiveBuffer, 0, destinationBytes, destinationBytes.Length - recvLength, recved);
                recvLength -= recved;
            }
        }
    }
}