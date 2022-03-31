namespace HT.Framework
{
    /// <summary>
    /// 性能及安全性检查目标
    /// </summary>
    public interface ISafetyCheckTarget
    {
        /// <summary>
        /// 性能及安全性检查
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>检查是否通过</returns>
        bool OnSafetyCheck(params object[] args);
    }
}