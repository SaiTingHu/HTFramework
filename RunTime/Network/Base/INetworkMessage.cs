namespace HT.Framework
{
    /// <summary>
    /// 网络消息接口
    /// </summary>
    public interface INetworkMessage : IReference
    {
        /// <summary>
        /// 封装消息为字节数组
        /// </summary>
        /// <returns>封装后的字节数组</returns>
        public byte[] Encapsulate();
    }
}