namespace HT.Framework
{
    /// <summary>
    /// 数据驱动器的接口
    /// </summary>
    public interface IDataDriver<T> where T : class
    {
        /// <summary>
        /// 数据
        /// </summary>
        T Data { get; set; }
    }
}