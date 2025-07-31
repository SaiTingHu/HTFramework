using System.Collections.Generic;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 默认的Tcp协议网络消息
    /// 消息头：消息校验码4字节 + 消息体长度4字节 + 身份ID8字节 + 主命令4字节 + 子命令4字节 + 加密方式4字节 + 返回码4字节 = 32字节
    /// 消息体：消息1长度4字节 + 消息1 + 消息2长度4字节 + 消息2......（消息体的长度存储在消息头的4至8索引位置的字节里）
    /// </summary>
    public sealed class TcpNetworkInfo : INetworkMessage
    {
        public int CheckCode;
        public int BodyLength;
        public long Sessionid;
        public int Command;
        public int Subcommand;
        public int Encrypt;
        public int ReturnCode;
        public List<string> Messages;
        
        public TcpNetworkInfo(int checkCode, long sessionid, int command, int subcommand, int encrypt, int returnCode, List<string> messages)
        {
            CheckCode = checkCode;
            BodyLength = 0;
            Sessionid = sessionid;
            Command = command;
            Subcommand = subcommand;
            Encrypt = encrypt;
            ReturnCode = returnCode;
            Messages = messages;
            if (Messages != null && Messages.Count > 0)
            {
                for (int i = 0; i < Messages.Count; i++)
                {
                    if (string.IsNullOrEmpty(Messages[i]))
                    {
                        Messages.RemoveAt(i);
                        i -= 1;
                        continue;
                    }
                    BodyLength += Encoding.UTF8.GetBytes(Messages[i]).Length;
                }
                BodyLength += Messages.Count * 4;
            }
        }
        public TcpNetworkInfo()
        {

        }

        public override string ToString()
        {
            StringToolkit.BeginConcat();
            StringToolkit.Concat($"校验码：{CheckCode}，消息体长度：{BodyLength}，身份ID：{Sessionid}，主命令：{Command}，子命令：{Subcommand}，加密方式：{Encrypt}，返回码：{ReturnCode}，");
            if (Messages != null && Messages.Count > 0)
            {
                StringToolkit.Concat($"消息体（{Messages.Count}条）：");
                for (int i = 0; i < Messages.Count; i++)
                {
                    StringToolkit.Concat(Messages[i]);
                    StringToolkit.Concat(i == (Messages.Count - 1) ? "。" : "、");
                }
            }
            else
            {
                StringToolkit.Concat("消息体（0条）。");
            }
            return StringToolkit.EndConcat();
        }
    }
}