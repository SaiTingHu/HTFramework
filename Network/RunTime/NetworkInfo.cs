using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 网络消息
    /// </summary>
    public struct NetworkInfo
    {
        int CrcCode;
        int BodyLength;
        long Sessionid;
        int Command;
        int Subcommand;
        int Encrypt;
        int ReturnCode;
        List<string> Messages;

        /// <summary>
        /// 填充
        /// </summary>
        public void Fill(byte[] head, byte[] body)
        {
            CrcCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 0));
            BodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 4));
            Sessionid = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(head, 8));
            Command = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 16));
            Subcommand = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 20));
            Encrypt = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 24));
            ReturnCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(head, 28));
            
            Messages = new List<string>();
            for (int i = 0; i < body.Length;)
            {
                byte[] bytes = new byte[4];
                Array.Copy(body, i, bytes, 0, 4);
                i += 4;
                int num = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));

                bytes = new byte[num];
                Array.Copy(body, i, bytes, 0, num);
                i += num;
                Messages.Add(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            }
        }

        /// <summary>
        /// 填充
        /// </summary>
        public void Fill(int crccode, long sessionid, int command, int subcommand, int encrypt, int returnCode, string[] messageBody)
        {
            CrcCode = crccode;
            BodyLength = 0;
            Sessionid = sessionid;
            Command = command;
            Subcommand = subcommand;
            Encrypt = encrypt;
            ReturnCode = returnCode;
            Messages = new List<string>(messageBody);
            for (int i = 0; i < Messages.Count; i++)
            {
                if (Messages[i] == "" || string.IsNullOrEmpty(Messages[i]))
                    break;
                BodyLength += Encoding.UTF8.GetBytes(Messages[i]).Length;
            }
            BodyLength += Messages.Count * 4;
        }
        
        /// <summary>
        /// 转换为字节数组
        /// </summary>
        public byte[] ToByte()
        {
            byte[] crccodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CrcCode));
            byte[] bodyLengthByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BodyLength));
            byte[] sessionidByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Sessionid));
            byte[] commandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Command));
            byte[] subcommandByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Subcommand));
            byte[] encryptByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Encrypt));
            byte[] returnCodeByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ReturnCode));

            byte[] messageBodyByte = new byte[BodyLength];
            int copyIndex = 0;
            for (int i = 0; i < Messages.Count; i++)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Messages[i]);
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length)).CopyTo(messageBodyByte, copyIndex);
                copyIndex += 4;
                bytes.CopyTo(messageBodyByte, copyIndex);
                copyIndex += bytes.Length;
            }

            byte[] totalByte = new byte[32 + BodyLength];
            crccodeByte.CopyTo(totalByte, 0);
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
