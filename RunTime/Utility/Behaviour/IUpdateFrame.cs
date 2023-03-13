namespace HT.Framework
{
    /// <summary>
    /// 行为接口：帧更新（仅限于HTBehaviour子类实现）
    /// </summary>
    public interface IUpdateFrame
    {
        /// <summary>
        /// 帧更新（每帧呼叫一次）
        /// </summary>
        void OnUpdateFrame();
    }
}