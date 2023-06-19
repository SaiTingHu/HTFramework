namespace HT.Framework
{
    /// <summary>
    /// 自定义指令
    /// </summary>
    public abstract class SentenceCustom : Sentence
    {
        /// <summary>
        /// 参数
        /// </summary>
        public string Args { get; internal set; }
    }
}