namespace HT.Framework
{
    /// <summary>
    /// 行为接口：秒更新（仅限于HTBehaviour子类实现）
    /// </summary>
    public interface IUpdateSecond
    {
        /// <summary>
        /// 秒更新（每秒呼叫一次）
        /// </summary>
        void OnUpdateSecond();
    }
}