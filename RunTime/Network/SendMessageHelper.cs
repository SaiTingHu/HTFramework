using System;
using System.Net;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 默认的发送消息助手
    /// </summary>
    public sealed class SendMessageHelper : ISendMessageHelper
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="info">消息对象</param>
        /// <returns>消息字节数组</returns>
        public byte[] SendMessage(INetworkInfo info)
        {
            NetworkInfo networkInfo = info as NetworkInfo;
            byte[] checkCodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.CheckCode));
            byte[] bodyLengthByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.BodyLength));
            byte[] sessionidByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.Sessionid));
            byte[] commandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.Command));
            byte[] subcommandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.Subcommand));
            byte[] encryptByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.Encrypt));
            byte[] returnCodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(networkInfo.ReturnCode));

            byte[] messageBodyByte = new byte[networkInfo.BodyLength];
            int copyIndex = 0;
            for (int i = 0; i < networkInfo.Messages.Count; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(networkInfo.Messages[i]);
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length)).CopyTo(messageBodyByte, copyIndex);
                copyIndex += 4;
                bytes.CopyTo(messageBodyByte, copyIndex);
                copyIndex += bytes.Length;
            }

            byte[] totalByte = new byte[32 + networkInfo.BodyLength];
            checkCodeByte.CopyTo(totalByte, 0);
            bodyLengthByte.CopyTo(totalByte, 4);
            sessionidByte.CopyTo(totalByte, 8);
            commandByte.CopyTo(totalByte, 16);
            subcommandByte.CopyTo(totalByte, 20);
            encryptByte.CopyTo(totalByte, 24);
            returnCodeByte.CopyTo(totalByte, 28);
            messageBodyByte.CopyTo(totalByte, 32);

            return totalByte;
        }
    }
}