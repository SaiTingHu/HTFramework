namespace HT.Framework
{
    /// <summary>
    /// 拥有本地化模式的窗口
    /// </summary>
    public interface ILocalizeWindow
    {
        
    }

    /// <summary>
    /// 本地化语言
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        Chinese = 0,
        /// <summary>
        /// 英语
        /// </summary>
        English = 1,
        /// <summary>
        /// 韩语
        /// </summary>
        Korean = 2,
        /// <summary>
        /// 日语
        /// </summary>
        Japanese = 3
    }

    /// <summary>
    /// 本地化词汇
    /// </summary>
    public struct Word
    {
        public string Chinese;
        public string English;
        public string Korean;
        public string Japanese;

        public Word(string chinese, string english, string korean, string japanese)
        {
            Chinese = chinese;
            English = english;
            Korean = korean;
            Japanese = japanese;
        }
        public Word(string chinese, string english)
        {
            Chinese = chinese;
            English = english;
            Korean = null;
            Japanese = null;
        }
    }
}