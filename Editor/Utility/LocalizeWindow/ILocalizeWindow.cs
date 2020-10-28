namespace HT.Framework
{
    /// <summary>
    /// 拥有本地化模式的窗口
    /// </summary>
    internal interface ILocalizeWindow
    {
        
    }

    /// <summary>
    /// 语言
    /// </summary>
    public enum Language
    {
        Chinese,
        English,
        Korean,
        Japanese
    }

    /// <summary>
    /// 词汇
    /// </summary>
    public struct Word
    {
        public string Chinese;
        public string English;
        public string Korean;
        public string Japanese;
    }
}