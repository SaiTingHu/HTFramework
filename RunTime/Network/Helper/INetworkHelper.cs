using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 网络管理器的助手接口
    /// </summary>
    public interface INetworkHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有的通信管道
        /// </summary>
        Dictionary<Type, ProtocolChannelBase> ProtocolChannels { get; set; }
        /// <summary>
        /// 开始连接服务器事件
        /// </summary>
        event HTFAction<ProtocolChannelBase> BeginConnectServerEvent;
        /// <summary>
        /// 连接服务器成功事件
        /// </summary>
        event HTFAction<ProtocolChannelBase> ConnectServerSuccessEvent;
        /// <summary>
        /// 连接服务器失败事件
        /// </summary>
        event HTFAction<ProtocolChannelBase> ConnectServerFailEvent;
        /// <summary>
        /// 与服务器断开连接事件
        /// </summary>
        event HTFAction<ProtocolChannelBase> DisconnectServerEvent;
        /// <summary>
        /// 发送消息成功事件
        /// </summary>
        event HTFAction<ProtocolChannelBase> SendMessageEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        event HTFAction<ProtocolChannelBase, INetworkMessage> ReceiveMessageEvent;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="channelTypes">启用的通信协议通道类型</param>
        void OnInitialization(List<string> channelTypes);
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 通道是否已连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <returns>是否已连接</returns>
        bool IsConnect(Type channelType);
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        void ConnectServer(Type channelType);
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">断开连接请求</param>
        void DisconnectServer(Type channelType, INetworkMessage message);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="channelType">通信协议通道类型</param>
        /// <param name="message">消息对象</param>
        /// <returns>是否发送成功</returns>
        bool SendMessage(Type channelType, INetworkMessage message);
    }
}