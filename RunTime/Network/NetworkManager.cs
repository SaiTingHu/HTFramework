using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网络管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Network)]
    public sealed class NetworkManager : InternalModuleBase<INetworkHelper>
    {
        /// <summary>
        /// 启用的通信协议通道类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> ChannelTypes = new List<string>();
        /// <summary>
        /// 服务器IP地址【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string ServerIP;
        /// <summary>
        /// 服务器端口号【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int ServerPort;
        /// <summary>
        /// 客户端IP地址【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string ClientIP;
        /// <summary>
        /// 客户端端口号【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int ClientPort;

        /// <summary>
        /// 开始连接服务器事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> BeginConnectServerEvent;
        /// <summary>
        /// 连接服务器成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> ConnectServerSuccessEvent;
        /// <summary>
        /// 连接服务器失败事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> ConnectServerFailEvent;
        /// <summary>
        /// 与服务器断开连接事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> DisconnectServerEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase, INetworkMessage> ReceiveMessageEvent;

        private IPEndPoint _serverEndPoint;
        private IPEndPoint _clientEndPoint;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public IPEndPoint ServerEndPoint
        {
            get
            {
                if (_serverEndPoint == null)
                {
                    _serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
                }
                return _serverEndPoint;
            }
        }
        /// <summary>
        /// 客户端地址
        /// </summary>
        public IPEndPoint ClientEndPoint
        {
            get
            {
                if (_clientEndPoint == null)
                {
                    _clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIP), ClientPort);
                }
                return _clientEndPoint;
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _helper.LoadProtocolChannels(ChannelTypes);
            _helper.BeginConnectServerEvent += (cha) =>
            {
                BeginConnectServerEvent?.Invoke(cha);
            };
            _helper.ConnectServerSuccessEvent += (cha) =>
            {
                ConnectServerSuccessEvent?.Invoke(cha);
            };
            _helper.ConnectServerFailEvent += (cha) =>
            {
                ConnectServerFailEvent?.Invoke(cha);
            };
            _helper.DisconnectServerEvent += (cha) =>
            {
                DisconnectServerEvent?.Invoke(cha);
            };
            _helper.ReceiveMessageEvent += (cha, mes) =>
            {
                ReceiveMessageEvent?.Invoke(cha, mes);
            };
        }
        
        /// <summary>
        /// 设置服务器IP地址及端口号
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        public void SetServerIP(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;

            if (_serverEndPoint != null)
            {
                _serverEndPoint.Address = IPAddress.Parse(ServerIP);
                _serverEndPoint.Port = ServerPort;
            }
        }
        
        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <returns>是否已连接</returns>
        public bool IsConnect<T>() where T : ProtocolChannelBase
        {
            return _helper.IsConnect(typeof(T));
        }
        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <returns>是否已连接</returns>
        public bool IsConnect(Type channelType)
        {
            return _helper.IsConnect(channelType);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        public void ConnectServer<T>() where T : ProtocolChannelBase
        {
            _helper.ConnectServer(typeof(T));
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        public void ConnectServer(Type channelType)
        {
            _helper.ConnectServer(channelType);
        }

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <param name="message">断开连接请求</param>
        public void DisconnectServer<T>(INetworkMessage message) where T : ProtocolChannelBase
        {
            _helper.DisconnectServer(typeof(T), message);
        }
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">断开连接请求</param>
        public void DisconnectServer(Type channelType, INetworkMessage message)
        {
            _helper.DisconnectServer(channelType, message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <param name="message">消息对象</param>
        /// <returns>已发送字节</returns>
        public async Task<int> SendMessage<T>(INetworkMessage message) where T : ProtocolChannelBase
        {
            int count = await _helper.SendMessage(typeof(T), message);
            return count;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">消息对象</param>
        /// <returns>已发送字节</returns>
        public async Task<int> SendMessage(Type channelType, INetworkMessage message)
        {
            int count = await _helper.SendMessage(channelType, message);
            return count;
        }

        /// <summary>
        /// 获取指定的通信管道
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <returns>通信协议通道实例</returns>
        public T GetProtocolChannel<T>() where T : ProtocolChannelBase
        {
            return GetProtocolChannel(typeof(T)) as T;
        }
        /// <summary>
        /// 获取指定的通信管道
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <returns>通信协议通道实例</returns>
        public ProtocolChannelBase GetProtocolChannel(Type channelType)
        {
            return _helper.GetProtocolChannel(channelType);
        }
    }
}