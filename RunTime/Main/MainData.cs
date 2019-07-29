namespace HT.Framework
{
    /// <summary>
    /// 全局的主要数据基类
    /// </summary>
    public abstract class MainData
    {
        /// <summary>
        /// 数据初始化
        /// </summary>
        public abstract void OnInitialization();

        /// <summary>
        /// 数据准备
        /// </summary>
        public abstract void OnPreparatory();
    }
}
