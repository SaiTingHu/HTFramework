﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace HT.Framework
{
    /// <summary>
    /// 通信协议通道的基类
    /// </summary>
    /// </summary>
    public abstract class ProtocolChannelBase
    {
        private bool _isEnableThread = false;
        private Thread _sendThread;
        private Thread _receiveThread;
        private List<byte[]> _sendDataBuffer = new List<byte[]>();
        private bool _isCanSend = false;

        /// <summary>
        /// 通信协议
        /// </summary>
        public virtual ProtocolType Protocol
        {
            get
            {
                return ProtocolType.Tcp;
            }
        }
        /// <summary>
        /// 通道类型
        /// </summary>
        public virtual SocketType Way
        {
            get
            {
                return SocketType.Stream;
            }
        }
        /// <summary>
        /// 是否需要保持连接
        /// </summary>
        public virtual bool IsNeedConnect
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 是否已连接至服务器
        /// </summary>
        public bool IsConnect
        {
            get
            {
                try
                {
                    return Client != null && Client.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 客户端
        /// </summary>
        public Socket Client { get; private set; }
        /// <summary>
        /// 发送消息成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> SendMessageEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase, INetworkMessage> ReceiveMessageEvent;
        /// <summary>
        /// 与服务器断开连接事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> DisconnectServerEvent;
        
        /// <summary>
        /// 初始化通道
        /// </summary>
        public virtual void OnInit()
        {
            _isEnableThread = true;

            if (IsNeedConnect)
            {
                _sendThread = new Thread(SendMessageNeedConnect);
                _sendThread.Start();

                _receiveThread = new Thread(ReceiveMessageNeedConnect);
                _receiveThread.Start();
            }
            else
            {
                Client = new Socket(AddressFamily.InterNetwork, Way, Protocol);

                _sendThread = new Thread(SendMessageNoConnect);
                _sendThread.Start();

                _receiveThread = new Thread(ReceiveMessageNoConnect);
                _receiveThread.Start();
            }
        }
        /// <summary>
        /// 终结通道
        /// </summary>
        public virtual void OnTerminate()
        {
            _isEnableThread = false;

            if (_sendThread != null && _sendThread.IsAlive)
            {
                _sendThread.Abort();
                _sendThread = null;
            }
            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Abort();
                _receiveThread = null;
            }
            
            _sendDataBuffer.Clear();
            _isCanSend = false;

            DisconnectServer();
        }
        /// <summary>
        /// 向通道中注入消息
        /// </summary>
        /// <param name="info">封装后的消息字节数组</param>
        public void InjectMessage(byte[] info)
        {
            _isCanSend = false;
            _sendDataBuffer.Add(info);
            _isCanSend = true;
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>协议名字</returns>
        public override string ToString()
        {
            return Protocol.ToString() + "协议通道";
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer()
        {
            if (Client == null)
            {
                Client = new Socket(AddressFamily.InterNetwork, Way, Protocol);
                Client.Connect(Main.m_Network.ServerEndPoint);
            }
        }
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        public void DisconnectServer()
        {
            if (Client != null)
            {
                if (IsNeedConnect && IsConnect)
                {
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Disconnect(false);

                    DisconnectServerEvent?.Invoke(this);
                }
                Client.Close();
                Client.Dispose();
                Client = null;
            }
        }

        /// <summary>
        /// 是否是断开连接请求
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>是否是断开连接请求</returns>
        public abstract bool IsDisconnectRequest(INetworkMessage message);
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="message">消息对象</param>
        /// <returns>封装后的字节数组</returns>
        public abstract byte[] EncapsulatedMessage(INetworkMessage message);
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>接收到的消息对象</returns>
        protected abstract INetworkMessage ReceiveMessage(Socket client);
        
        /// <summary>
        /// 发送消息（需要保持连接的协议）
        /// </summary>
        private void SendMessageNeedConnect()
        {
            while (_isEnableThread)
            {
                if (IsConnect && _isCanSend && _sendDataBuffer.Count > 0)
                {
                    int sendCount = Client.Send(_sendDataBuffer[0], _sendDataBuffer[0].Length, 0);
                    if (sendCount > 0)
                    {
                        _sendDataBuffer.RemoveAt(0);

                        Main.Current.QueueOnMainThread(() =>
                        {
                            SendMessageEvent?.Invoke(this);
                        });
                    }
                }
            }
        }
        /// <summary>
        /// 接收消息（需要保持连接的协议）
        /// </summary>
        private void ReceiveMessageNeedConnect()
        {
            while (_isEnableThread)
            {
                if (IsConnect)
                {
                    INetworkMessage message = ReceiveMessage(Client);

                    if (message != null)
                    {
                        Main.Current.QueueOnMainThread(() =>
                        {
                            ReceiveMessageEvent?.Invoke(this, message);

                            if (IsDisconnectRequest(message))
                            {
                                DisconnectServer();
                            }
                        });
                    }
                }
            }
        }
        /// <summary>
        /// 发送消息（不需要保持连接的协议）
        /// </summary>
        private void SendMessageNoConnect()
        {
            while (_isEnableThread)
            {
                if (_isCanSend && _sendDataBuffer.Count > 0)
                {
                    int sendCount = Client.SendTo(_sendDataBuffer[0], Main.m_Network.ServerEndPoint);
                    if (sendCount > 0)
                    {
                        _sendDataBuffer.RemoveAt(0);

                        Main.Current.QueueOnMainThread(() =>
                        {
                            SendMessageEvent?.Invoke(this);
                        });
                    }
                }
            }
        }
        /// <summary>
        /// 接收消息（不需要保持连接的协议）
        /// </summary>
        private void ReceiveMessageNoConnect()
        {
            while (_isEnableThread)
            {
                INetworkMessage message = ReceiveMessage(Client);

                if (message != null)
                {
                    Main.Current.QueueOnMainThread(() =>
                    {
                        ReceiveMessageEvent?.Invoke(this, message);
                    });
                }
            }
        }
    }
}