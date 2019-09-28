using System;

namespace HT.Framework
{
    public class JsonException : ApplicationException
    {
        public JsonException () : base ()
        {
        }

        internal JsonException (ParserToken token) :
            base (string.Format ("输入字符串中包含无效的令牌 '{0}' ！", token))
        {
        }

        internal JsonException (ParserToken token,
                                Exception inner_exception) :
            base (string.Format("输入字符串中包含无效的令牌 '{0}' ！", token),
                inner_exception)
        {
        }

        internal JsonException (int c) :
            base (string.Format ("输入字符串中包含无效的字符 '{0}' ！", (char) c))
        {
        }

        internal JsonException (int c, Exception inner_exception) :
            base (string.Format ("输入字符串中包含无效的字符 '{0}' ！", (char) c),
                inner_exception)
        {
        }


        public JsonException (string message) : base (message)
        {
        }

        public JsonException (string message, Exception inner_exception) :
            base (message, inner_exception)
        {
        }
    }
}
