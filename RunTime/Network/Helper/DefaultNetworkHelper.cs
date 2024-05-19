using System;
using System.Collections;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的网络管理器助手
    /// </summary>
    internal sealed class DefaultNetworkHelper : INetworkHelper
    {
        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有的通信管道
        /// </summary>
        public Dictionary<Type, ProtocolChannelBase> ProtocolChannels { get; private set; } = new Dictionary<Type, ProtocolChannelBase>();
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
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {

        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {

        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            foreach (var channel in ProtocolChannels)
            {
                channel.Value.OnTerminate();
            }

            ProtocolChannels.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 加载通信管道
        /// </summary>
        /// <param name="channelTypes">启用的通信协议通道类型</param>
        public void LoadProtocolChannels(List<string> channelTypes)
        {
            for (int i = 0; i < channelTypes.Count; i++)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(channelTypes[i], false);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(ProtocolChannelBase)))
                    {
                        if (!ProtocolChannels.ContainsKey(type))
                            ProtocolChannels.Add(type, Activator.CreateInstance(type) as ProtocolChannelBase);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Network, $"加载通信协议通道失败：通信协议通道类 {channelTypes[i]} 必须继承至基类：ProtocolChannelBase！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Network, $"加载通信协议通道失败：丢失通信协议通道类 {channelTypes[i]}！");
                }
            }

            foreach (var channel in ProtocolChannels)
            {
                channel.Value.OnInit();
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
        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <returns>是否已连接</returns>
        public bool IsConnect(Type channelType)
        {
            if (ProtocolChannels.ContainsKey(channelType))
            {
                return ProtocolChannels[channelType].IsConnect;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        public void ConnectServer(Type channelType)
        {
            if (ProtocolChannels.ContainsKey(channelType))
            {
                if (ProtocolChannels[channelType].IsNeedConnect)
                {
                    if (ProtocolChannels[channelType].IsConnect)
                        return;

                    BeginConnectServerEvent?.Invoke(ProtocolChannels[channelType]);

                    Main.Current.StartCoroutine(ConnectServerCoroutine(ProtocolChannels[channelType]));
                }
                else
                {
                    Log.Warning($"连接服务器出错：{ProtocolChannels[channelType]} 不需要与服务器保持连接！");
                }
            }
            else
            {
                Log.Warning($"连接服务器出错：{channelType.FullName} 未启用或并不是有效的通信协议！");
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
                Log.Error($"连接服务器出错：{e}");
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
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">断开连接请求</param>
        public void DisconnectServer(Type channelType, INetworkMessage message)
        {
            if (ProtocolChannels.ContainsKey(channelType))
            {
                if (ProtocolChannels[channelType].IsNeedConnect)
                {
                    if (!ProtocolChannels[channelType].IsConnect)
                        return;

                    if (ProtocolChannels[channelType].IsDisconnectRequest(message))
                    {
                        SendMessage(channelType, message);
                    }
                    else
                    {
                        Log.Warning("与服务器断开连接出错：发送的消息并不是断开连接的请求！");
                    }
                }
                else
                {
                    Log.Warning($"与服务器断开连接出错：{ProtocolChannels[channelType]} 不需要与服务器保持连接！");
                }
            }
            else
            {
                Log.Warning($"与服务器断开连接出错：{channelType.FullName} 未启用或并不是有效的通信协议！");
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">消息对象</param>
        /// <returns>是否发送成功</returns>
        public bool SendMessage(Type channelType, INetworkMessage message)
        {
            if (ProtocolChannels.ContainsKey(channelType))
            {
                if (ProtocolChannels[channelType].IsNeedConnect)
                {
                    if (ProtocolChannels[channelType].IsConnect)
                    {
                        ProtocolChannels[channelType].InjectMessage(ProtocolChannels[channelType].EncapsulatedMessage(message));
                        return true;
                    }
                    else
                    {
                        Log.Error("发送消息出错：客户端已断开连接！");
                        return false;
                    }
                }
                else
                {
                    ProtocolChannels[channelType].InjectMessage(ProtocolChannels[channelType].EncapsulatedMessage(message));
                    return true;
                }
            }
            else
            {
                Log.Warning($"发送消息出错：{channelType.FullName} 未启用或并不是有效的通信协议！");
                return true;
            }
        }
    }
}