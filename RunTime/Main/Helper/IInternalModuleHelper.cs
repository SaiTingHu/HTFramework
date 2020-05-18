namespace HT.Framework
{
    /// <summary>
    /// 内置模块的助手接口
    /// </summary>
    public interface IInternalModuleHelper
    {
        /// <summary>
        /// 所属的内置模块
        /// </summary>
        InternalModuleBase Module { get; set; }
    }
}