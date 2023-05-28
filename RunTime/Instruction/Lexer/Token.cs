namespace HT.Framework
{
    /// <summary>
    /// 单词
    /// </summary>
    internal sealed class Token : IReference
    {
        /// <summary>
        /// 单词类型
        /// </summary>
        public TokenType Type;
        /// <summary>
        /// 单词内容
        /// </summary>
        public string Value;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Type = TokenType.Invalid;
            Value = null;
        }
    }

    /// <summary>
    /// 单词类型
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// 指令关键字
        /// </summary>
        Instruct,
        /// <summary>
        /// 标识符
        /// </summary>
        Identifier,
        /// <summary>
        /// 字符串值
        /// </summary>
        String,
        /// <summary>
        /// bool值
        /// </summary>
        Bool,
        /// <summary>
        /// 整型值
        /// </summary>
        Int,
        /// <summary>
        /// 浮点值
        /// </summary>
        Float,
        /// <summary>
        /// Vector2值
        /// </summary>
        Vector2,
        /// <summary>
        /// Vector3值
        /// </summary>
        Vector3,
        /// <summary>
        /// 无效的
        /// </summary>
        Invalid
    }
}