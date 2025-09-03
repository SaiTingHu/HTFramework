using System;
using System.Net;

namespace HT.Framework
{
    /// <summary>
    /// 默认的Tcp协议网络消息
    /// 消息校验码：4字节
    /// 消息头：消息体长度4字节 + 身份ID8字节 + 主命令4字节 + 子命令4字节 + 加密方式4字节 + 返回码4字节 = 28字节
    /// 消息体：消息体内容（消息体内容的长度存储在消息头的0-4索引位置的字节里）
    /// </summary>
    public sealed class TcpNetworkInfo : INetworkMessage
    {
        /// <summary>
        /// 校验码
        /// </summary>
        public int CheckCode;
        /// <summary>
        /// 身份ID
        /// </summary>
        public long Sessionid;
        /// <summary>
        /// 主命令
        /// </summary>
        public int Command;
        /// <summary>
        /// 子命令
        /// </summary>
        public int Subcommand;
        /// <summary>
        /// 加密方式
        /// </summary>
        public int Encrypt;
        /// <summary>
        /// 返回码
        /// </summary>
        public int ReturnCode;
        /// <summary>
        /// 消息内容
        /// </summary>
        public byte[] Message;

        /// <summary>
        /// 封装消息为字节数组
        /// </summary>
        /// <returns>封装后的字节数组</returns>
        public byte[] Encapsulate()
        {
            int bodyLength = Message == null ? 0 : Message.Length;
            byte[] checkCodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CheckCode));
            byte[] bodyLengthByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bodyLength));
            byte[] sessionidByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Sessionid));
            byte[] commandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Command));
            byte[] subcommandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Subcommand));
            byte[] encryptByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Encrypt));
            byte[] returnCodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ReturnCode));

            byte[] totalByte = new byte[32 + bodyLength];
            checkCodeByte.CopyTo(totalByte, 0);
            bodyLengthByte.CopyTo(totalByte, 4);
            sessionidByte.CopyTo(totalByte, 8);
            commandByte.CopyTo(totalByte, 16);
            subcommandByte.CopyTo(totalByte, 20);
            encryptByte.CopyTo(totalByte, 24);
            returnCodeByte.CopyTo(totalByte, 28);
            if (Message != null) Message.CopyTo(totalByte, 32);

            return totalByte;
        }
        public override string ToString()
        {
            return $"校验码：{CheckCode}，身份ID：{Sessionid}，主命令：{Command}，子命令：{Subcommand}，加密方式：{Encrypt}，返回码：{ReturnCode}，消息体长度：{(Message != null ? Message.Length : 0)}字节。";
        }
        public void Reset()
        {

        }
    }
}