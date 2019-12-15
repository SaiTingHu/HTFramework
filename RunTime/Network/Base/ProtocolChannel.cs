using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace HT.Framework
{
    /// <summary>
    /// 通信协议通道的基类
    /// </summary>
    /// </summary>
    public abstract class ProtocolChannel
    {
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
                if (Client != null)
                    return Client.Connected;
                else
                    return false;
            }
        }
        /// <summary>
        /// 客户端
        /// </summary>
        public Socket Client { get; private set; }
        /// <summary>
        /// 发送消息成功事件
        /// </summary>
        public event HTFAction SendMessageEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        public event HTFAction<INetworkInfo> ReceiveMessageEvent;

        private Thread _sendThread;
        private Thread _receiveThread;
        private List<byte[]> _sendDataBuffer = new List<byte[]>();
        private bool _isCanSend = false;
        
        /// <summary>
        /// 初始化通道
        /// </summary>
        public virtual void OnInitialization()
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
            
            if (IsNeedConnect) _sendThread = new Thread(SendMessageNeedConnect);
            else _sendThread = new Thread(SendMessageNoConnect);
            _sendThread.Start();

            if (IsNeedConnect) _receiveThread = new Thread(ReceiveMessageNeedConnect);
            else _receiveThread = new Thread(ReceiveMessageNoConnect);
            _receiveThread.Start();
        }
        /// <summary>
        /// 终结通道
        /// </summary>
        public virtual void OnTermination()
        {
            _sendDataBuffer.Clear();
            _isCanSend = false;

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
            if (Client != null)
            {
                Client.Shutdown(SocketShutdown.Both);
                Client.Disconnect(false);
                Client.Close();
                Client.Dispose();
                Client = null;
            }
        }
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="info">消息对象</param>
        /// <returns>封装后的字节数组</returns>
        public abstract byte[] OnEncapsulatedMessage(INetworkInfo info);
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>接收到的消息对象</returns>
        protected abstract INetworkInfo OnReceiveMessage(Socket client);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="info">封装后的字节数组</param>
        public void OnSendMessage(byte[] info)
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
        /// 发送消息（需要保持连接的协议）
        /// </summary>
        private void SendMessageNeedConnect()
        {
            while (true)
            {
                if (IsConnect && _isCanSend && _sendDataBuffer.Count > 0)
                {
                    try
                    {
                        int sendCount = Client.Send(_sendDataBuffer[0], _sendDataBuffer[0].Length, 0);
                        if (sendCount > 0)
                        {
                            _sendDataBuffer.RemoveAt(0);

                            Main.Current.QueueOnMainThread(() =>
                            {
                                SendMessageEvent?.Invoke();
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        GlobalTools.LogError("发送消息出错：" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// 接收消息（需要保持连接的协议）
        /// </summary>
        private void ReceiveMessageNeedConnect()
        {
            while (true)
            {
                if (IsConnect)
                {
                    INetworkInfo info = OnReceiveMessage(Client);

                    if (info != null)
                    {
                        Main.Current.QueueOnMainThread(() =>
                        {
                            ReceiveMessageEvent?.Invoke(info);
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
            while (true)
            {
                if (_isCanSend && _sendDataBuffer.Count > 0)
                {
                    try
                    {
                        int sendCount = Client.SendTo(_sendDataBuffer[0], Main.m_Network.ServerEndPoint);
                        if (sendCount > 0)
                        {
                            _sendDataBuffer.RemoveAt(0);

                            Main.Current.QueueOnMainThread(() =>
                            {
                                SendMessageEvent?.Invoke();
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        GlobalTools.LogError("发送消息出错：" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// 接收消息（不需要保持连接的协议）
        /// </summary>
        private void ReceiveMessageNoConnect()
        {
            while (true)
            {
                INetworkInfo info = OnReceiveMessage(Client);

                if (info != null)
                {
                    Main.Current.QueueOnMainThread(() =>
                    {
                        ReceiveMessageEvent?.Invoke(info);
                    });
                }
            }
        }
    }
}