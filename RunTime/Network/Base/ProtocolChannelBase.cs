using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Framework
{
    /// <summary>
    /// 通信协议通道的基类
    /// </summary>
    /// </summary>
    public abstract class ProtocolChannelBase
    {
        private bool _isEnableThread = false;
        private Thread _receiveThread;

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
        /// 服务器地址
        /// </summary>
        public virtual IPEndPoint ServerEndPoint
        {
            get
            {
                return Main.m_Network.ServerEndPoint;
            }
        }
        /// <summary>
        /// 客户端地址
        /// </summary>
        public virtual IPEndPoint ClientEndPoint
        {
            get
            {
                return Main.m_Network.ClientEndPoint;
            }
        }
        /// <summary>
        /// 客户端
        /// </summary>
        public Socket Client { get; private set; }
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
                _receiveThread = new Thread(ReceiveMessageNeedConnect);
                _receiveThread.Start();
            }
            else
            {
                Client = new Socket(AddressFamily.InterNetwork, Way, Protocol);

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

            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Abort();
                _receiveThread = null;
            }
            
            DisconnectServer();
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>协议名字</returns>
        public override string ToString()
        {
            return $"{Protocol}协议通道";
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer()
        {
            if (!IsNeedConnect)
                return;

            if (Client == null)
            {
                Client = new Socket(AddressFamily.InterNetwork, Way, Protocol);
                Client.Connect(ServerEndPoint);
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
                    try
                    {
                        Client.Shutdown(SocketShutdown.Both);
                        Client.Disconnect(false);
                    }
                    catch (Exception)
                    { }

                    DisconnectServerEvent?.Invoke(this);
                }
                Client.Close();
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
        /// 发送消息
        /// </summary>
        /// <param name="info">封装后的消息字节数组</param>
        /// <returns>已发送字节</returns>
        public abstract Task<int> SendMessage(byte[] info);
        /// <summary>
        /// 接收消息（此方法将在子线程中调用）
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>接收到的消息对象</returns>
        protected abstract INetworkMessage ReceiveMessage(Socket client);
        
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
                            Main.m_ReferencePool.Despawn(message);
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
                        Main.m_ReferencePool.Despawn(message);
                    });
                }
            }
        }
    }
}