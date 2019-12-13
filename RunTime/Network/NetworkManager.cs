using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网络管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NetworkManager : ModuleManagerBase
    {
        /// <summary>
        /// 发送消息助手的类型【请勿在代码中修改】
        /// </summary>
        public string SendMessageHelperType;
        /// <summary>
        /// 接收消息助手的类型【请勿在代码中修改】
        /// </summary>
        public string ReceiveMessageHelperType;
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string ServerIP;
        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int Port;
        /// <summary>
        /// 通信协议
        /// </summary>
        public ProtocolType Protocol;
        /// <summary>
        /// 开始连接服务器事件
        /// </summary>
        public event HTFAction BeginConnectServerEvent;
        /// <summary>
        /// 连接服务器成功事件
        /// </summary>
        public event HTFAction ConnectServerSuccessEvent;
        /// <summary>
        /// 连接服务器失败事件
        /// </summary>
        public event HTFAction ConnectServerFailEvent;
        /// <summary>
        /// 与服务器断开连接事件
        /// </summary>
        public event HTFAction DisconnectServerEvent;
        /// <summary>
        /// 发送消息成功事件
        /// </summary>
        public event HTFAction SendMessageEvent;
        /// <summary>
        /// 接收消息成功事件
        /// </summary>
        public event HTFAction<INetworkInfo> ReceiveMessageEvent;

        private Socket _client;
        private Thread _receiveThread;
        private ISendMessageHelper _sendMessageHelper;
        private IReceiveMessageHelper _receiveMessageHelper;
        private List<byte[]> _sendDataBuffer = new List<byte[]>();
        private bool _isSending = false;
        
        public override void OnTermination()
        {
            base.OnTermination();

            DisconnectServer();
        }

        /// <summary>
        /// 当前是否已连接
        /// </summary>
        public bool IsConnect
        {
            get
            {
                if (_client != null)
                    return _client.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer()
        {
            if (IsConnect)
            {
                GlobalTools.LogInfo("已经连接至服务器！");
                return;
            }

            //加载发送消息助手
            if (_sendMessageHelper == null)
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(SendMessageHelperType);
                if (type != null)
                {
                    if (typeof(ISendMessageHelper).IsAssignableFrom(type))
                    {
                        _sendMessageHelper = Activator.CreateInstance(type) as ISendMessageHelper;
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Network, "加载发送消息助手失败：发送消息助手类 " + SendMessageHelperType + " 必须实现接口：ISendMessageHelper！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Network, "加载发送消息助手失败：丢失发送消息助手类 " + SendMessageHelperType + "！");
                }
            }

            //加载接收消息助手
            if (_receiveMessageHelper == null)
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(ReceiveMessageHelperType);
                if (type != null)
                {
                    if (typeof(IReceiveMessageHelper).IsAssignableFrom(type))
                    {
                        _receiveMessageHelper = Activator.CreateInstance(type) as IReceiveMessageHelper;
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Network, "加载接收消息助手失败：接收消息助手类 " + ReceiveMessageHelperType + " 必须实现接口：IReceiveMessageHelper！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Network, "加载接收消息助手失败：丢失接收消息助手类 " + ReceiveMessageHelperType + "！");
                }
            }

            BeginConnectServerEvent?.Invoke();

            Main.Current.StartCoroutine(ConnectServerCoroutine());
        }
        private IEnumerator ConnectServerCoroutine()
        {
            yield return YieldInstructioner.GetWaitForEndOfFrame();

            try
            {
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, Protocol);
                _client.Connect(IPAddress.Parse(ServerIP), Port);
            }
            catch (Exception e)
            {
                GlobalTools.LogError("连接服务器出错：" + e.ToString());
            }
            finally
            {
                if (IsConnect)
                {
                    _receiveThread = new Thread(ReceiveMessage);
                    _receiveThread.Start();

                    ConnectServerSuccessEvent?.Invoke();
                }
                else
                {
                    ConnectServerFailEvent?.Invoke();
                }
            }
        }

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        public void DisconnectServer()
        {
            _sendDataBuffer.Clear();
            _isSending = false;

            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Abort();
                _receiveThread = null;
            }
            if (_client != null)
            {
                _client.Disconnect(false);
                _client.Close();
                _client.Dispose();
                _client = null;
            }

            DisconnectServerEvent?.Invoke();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="info">消息</param>
        public void SendMessage(INetworkInfo info)
        {
            if (IsConnect)
            {
                _sendDataBuffer.Add(_sendMessageHelper.SendMessage(info));

                if (!_isSending)
                {
                    Main.Current.StartCoroutine(SendMessageCoroutine());
                }
            }
            else
            {
                GlobalTools.LogError("发送消息失败：客户端已断开连接！");
            }
        }
        private IEnumerator SendMessageCoroutine()
        {
            _isSending = true;
            while (_sendDataBuffer.Count > 0)
            {
                try
                {
                    int sendCount = _client.Send(_sendDataBuffer[0], _sendDataBuffer[0].Length, 0);
                    if (sendCount > 0)
                    {
                        _sendDataBuffer.RemoveAt(0);

                        SendMessageEvent?.Invoke();
                    }
                }
                catch (Exception e)
                {
                    GlobalTools.LogError("发送消息失败：" + e.ToString());
                }
                yield return null;
            }
            _isSending = false;
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        private void ReceiveMessage()
        {
            while (true)
            {
                INetworkInfo info = _receiveMessageHelper.ReceiveMessage(_client);

                ReceiveMessageEvent?.Invoke(info);
            }
        }
    }
}