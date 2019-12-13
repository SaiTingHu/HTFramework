namespace HT.Framework
{
    /// <summary>
    /// 发送消息助手接口
    /// </summary>
    public interface ISendMessageHelper
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        byte[] SendMessage(INetworkInfo info);
    }
}