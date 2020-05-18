using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网络管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Network)]
    public sealed class NetworkManager : InternalModuleBase
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
        /// 发送消息成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase> SendMessageEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        public event HTFAction<ProtocolChannelBase, INetworkMessage> ReceiveMessageEvent;

        private Dictionary<Type, ProtocolChannelBase> _protocolChannels = new Dictionary<Type, ProtocolChannelBase>();
        private IPEndPoint _serverEndPoint;
        private IPEndPoint _clientEndPoint;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            //加载通信协议通道
            for (int i = 0; i < ChannelTypes.Count; i++)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(ChannelTypes[i]);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(ProtocolChannelBase)))
                    {
                        if (!_protocolChannels.ContainsKey(type))
                            _protocolChannels.Add(type, Activator.CreateInstance(type) as ProtocolChannelBase);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Network, "加载通信协议通道失败：通信协议通道类 " + ChannelTypes[i] + " 必须实现接口：IProtocolChannel！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Network, "加载通信协议通道失败：丢失通信协议通道类 " + ChannelTypes[i] + "！");
                }
            }

            //初始化通道
            foreach (var channel in _protocolChannels)
            {
                channel.Value.OnInitialization();
                channel.Value.SendMessageEvent += (cha) =>
                {
                    SendMessageEvent?.Invoke(cha);
                };
                channel.Value.ReceiveMessageEvent += (cha, message) =>
                {
                    ReceiveMessageEvent?.Invoke(cha, message);
                };
                channel.Value.DisconnectServerEvent += (cha) =>
                {
                    DisconnectServerEvent?.Invoke(cha);
                };
            }
        }

        internal override void OnTermination()
        {
            base.OnTermination();

            //终结通道
            foreach (var channel in _protocolChannels)
            {
                channel.Value.OnTermination();
            }

            _protocolChannels.Clear();
        }

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

        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <returns>是否已连接</returns>
        public bool IsConnect<T>() where T : ProtocolChannelBase
        {
            return IsConnect(typeof(T));
        }
        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <returns>是否已连接</returns>
        public bool IsConnect(Type channelType)
        {
            if (_protocolChannels.ContainsKey(channelType))
            {
                return _protocolChannels[channelType].IsConnect;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        public void ConnectServer<T>() where T : ProtocolChannelBase
        {
            ConnectServer(typeof(T));
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        public void ConnectServer(Type channelType)
        {
            if (_protocolChannels.ContainsKey(channelType))
            {
                if (_protocolChannels[channelType].IsNeedConnect)
                {
                    if (_protocolChannels[channelType].IsConnect)
                    {
                        return;
                    }

                    BeginConnectServerEvent?.Invoke(_protocolChannels[channelType]);

                    Main.Current.StartCoroutine(ConnectServerCoroutine(_protocolChannels[channelType]));
                }
                else
                {
                    GlobalTools.LogWarning("连接服务器出错：" + _protocolChannels[channelType].ToString() + " 不需要与服务器保持连接！");
                }
            }
            else
            {
                GlobalTools.LogWarning("连接服务器出错：" + channelType.FullName + " 未启用或并不是有效的通信协议！");
            }
        }
        private IEnumerator ConnectServerCoroutine(ProtocolChannelBase protocolChannel)
        {
            yield return null;

            try
            {
                protocolChannel.DisconnectServer();
                protocolChannel.ConnectServer();
            }
            catch (Exception e)
            {
                GlobalTools.LogError("连接服务器出错：" + e.ToString());
            }
            finally
            {
                if (protocolChannel.IsConnect)
                {
                    ConnectServerSuccessEvent?.Invoke(protocolChannel);
                }
                else
                {
                    ConnectServerFailEvent?.Invoke(protocolChannel);
                }
            }
        }

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <param name="message">断开连接请求</param>
        public void DisconnectServer<T>(INetworkMessage message) where T : ProtocolChannelBase
        {
            DisconnectServer(typeof(T), message);
        }
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">断开连接请求</param>
        public void DisconnectServer(Type channelType, INetworkMessage message)
        {
            if (_protocolChannels.ContainsKey(channelType))
            {
                if (_protocolChannels[channelType].IsNeedConnect)
                {
                    if (!_protocolChannels[channelType].IsConnect)
                    {
                        return;
                    }

                    if (_protocolChannels[channelType].IsDisconnectRequest(message))
                    {
                        SendMessage(channelType, message);
                    }
                    else
                    {
                        GlobalTools.LogWarning("与服务器断开连接出错：发送的消息并不是断开连接的请求！");
                    }
                }
                else
                {
                    GlobalTools.LogWarning("与服务器断开连接出错：" + _protocolChannels[channelType].ToString() + " 不需要与服务器保持连接！");
                }
            }
            else
            {
                GlobalTools.LogWarning("与服务器断开连接出错：" + channelType.FullName + " 未启用或并不是有效的通信协议！");
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">通信协议通道类型</typeparam>
        /// <param name="message">消息对象</param>
        /// <returns>是否发送成功</returns>
        public bool SendMessage<T>(INetworkMessage message) where T : ProtocolChannelBase
        {
            return SendMessage(typeof(T), message);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">消息对象</param>
        /// <returns>是否发送成功</returns>
        public bool SendMessage(Type channelType, INetworkMessage message)
        {
            if (_protocolChannels.ContainsKey(channelType))
            {
                if (_protocolChannels[channelType].IsNeedConnect)
                {
                    if (_protocolChannels[channelType].IsConnect)
                    {
                        _protocolChannels[channelType].InjectMessage(_protocolChannels[channelType].EncapsulatedMessage(message));
                        return true;
                    }
                    else
                    {
                        GlobalTools.LogError("发送消息出错：客户端已断开连接！");
                        return false;
                    }
                }
                else
                {
                    _protocolChannels[channelType].InjectMessage(_protocolChannels[channelType].EncapsulatedMessage(message));
                    return true;
                }
            }
            else
            {
                GlobalTools.LogWarning("发送消息出错：" + channelType.FullName + " 未启用或并不是有效的通信协议！");
                return true;
            }
        }
    }
}