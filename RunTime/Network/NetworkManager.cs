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
    public sealed class NetworkManager : ModuleManager
    {
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string IP;
        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int Port;
        public event HTFAction BeginConnectEvent;
        public event HTFAction ConnectSuccessEvent;
        public event HTFAction ConnectFailEvent;
        public event HTFAction<NetworkInfo> ReceiveMessageEvent;

        private Socket _client;
        private Thread _receiveThread;
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
        /// 设置服务器IP与端口号
        /// </summary>
        public void SetIPPort(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer(float delayed)
        {
            if (IsConnect)
            {
                GlobalTools.LogInfo("已经连接至服务器！");
                return;
            }

            if (BeginConnectEvent != null)
                BeginConnectEvent();

            this.DelayExecute(() =>
            {
                try
                {
                    _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _client.Connect(IPAddress.Parse(IP), Port);
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

                        if (ConnectSuccessEvent != null)
                            ConnectSuccessEvent();
                    }
                    else
                    {
                        if (ConnectFailEvent != null)
                            ConnectFailEvent();
                    }
                }
            }, delayed);
        }
        
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        public void DisconnectServer()
        {
            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Abort();
                _receiveThread = null;
            }
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendInfo(NetworkInfo info)
        {
            if (IsConnect)
            {
                byte[] sendBytes = info.ToByte();
                _sendDataBuffer.Add(sendBytes);

                if (!_isSending)
                {
                    StartCoroutine(SendInfoCoroutine(sendBytes));
                }
            }
            else
            {
                GlobalTools.LogError("发送消息失败：客户端已断开连接！");
            }
        }
        private IEnumerator SendInfoCoroutine(byte[] sendBytes)
        {
            _isSending = true;
            while (_sendDataBuffer.Count > 0)
            {
                try
                {
                    int sendNum = _client.Send(_sendDataBuffer[0], _sendDataBuffer[0].Length, 0);
                    if (sendNum > 0)
                    {
                        _sendDataBuffer.RemoveAt(0);
                    }
                }
                catch (Exception e)
                {
                    GlobalTools.LogError("发送数据出错：" + e.ToString());
                }
                yield return 0;
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
                //接收消息头（消息校验码4字节 + 消息体长度4字节 + 身份ID8字节 + 主命令4字节 + 子命令4字节 + 加密方式4字节 + 返回码4字节 = 32字节）
                int restHeadLength = 32;
                byte[] recvBytesHead = new byte[restHeadLength];
                while (restHeadLength > 0)
                {
                    byte[] recvBytes1 = new byte[32];
                    int alreadyRecvHead = 0;
                    if (restHeadLength >= recvBytes1.Length)
                    {
                        alreadyRecvHead = _client.Receive(recvBytes1, recvBytes1.Length, 0);
                    }
                    else
                    {
                        alreadyRecvHead = _client.Receive(recvBytes1, restHeadLength, 0);
                    }
                    recvBytes1.CopyTo(recvBytesHead, recvBytesHead.Length - restHeadLength);
                    restHeadLength -= alreadyRecvHead;
                }

                //接收消息体（消息体的长度存储在消息头的4至8索引位置的字节里）
                byte[] bytes = new byte[4];
                Array.Copy(recvBytesHead, 4, bytes, 0, 4);
                int restBodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
                byte[] recvBytesBody = new byte[restBodyLength];
                while (restBodyLength > 0)
                {
                    byte[] recvBytes2 = new byte[restBodyLength < 1024 ? restBodyLength : 1024];
                    int alreadyRecvBody = 0;
                    if (restBodyLength >= recvBytes2.Length)
                    {
                        alreadyRecvBody = _client.Receive(recvBytes2, recvBytes2.Length, 0);
                    }
                    else
                    {
                        alreadyRecvBody = _client.Receive(recvBytes2, restBodyLength, 0);
                    }
                    recvBytes2.CopyTo(recvBytesBody, recvBytesBody.Length - restBodyLength);
                    restBodyLength -= alreadyRecvBody;
                }

                //解析
                NetworkInfo info = new NetworkInfo();
                info.Fill(recvBytesHead, recvBytesBody);
                if (ReceiveMessageEvent != null)
                    ReceiveMessageEvent(info);
            }
        }
    }
}
