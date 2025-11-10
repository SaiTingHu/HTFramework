using System.Collections.Generic;
using System.Text;

namespace HT.Framework
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    internal static class InstructionLexer
    {
        private static StringBuilder Builder = new StringBuilder();
        private static char BOM = '\ufeff';

        /// <summary>
        /// 词法分析
        /// </summary>
        /// <param name="code">指令代码（一条指令）</param>
        /// <param name="tokens">输出的单词序列</param>
        public static void Analyze(string code, List<Token> tokens)
        {
            if (string.IsNullOrEmpty(code))
                return;

            bool isStr = false;
            Builder.Clear();
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '/' && (i + 1) < code.Length && code[i + 1] == '/')
                    break;

                if (code[i] == '\t' || code[i] == '\r' || code[i] == BOM)
                    continue;

                if (isStr)
                {
                    if (code[i] == '\"')
                    {
                        Builder.Append(code[i]);
                        tokens.Add(GenerateToken(Builder.ToString()));
                        Builder.Clear();
                        isStr = false;
                    }
                    else
                    {
                        Builder.Append(code[i]);
                    }
                }
                else
                {
                    if (char.IsWhiteSpace(code[i]) || code[i] == '\0')
                    {
                        if (Builder.Length > 0)
                        {
                            tokens.Add(GenerateToken(Builder.ToString()));
                            Builder.Clear();
                        }
                    }
                    else if (code[i] == '\"')
                    {
                        if (Builder.Length > 0)
                        {
                            tokens.Add(GenerateToken(Builder.ToString()));
                            Builder.Clear();
                        }
                        Builder.Append(code[i]);
                        isStr = true;
                    }
                    else
                    {
                        Builder.Append(code[i]);
                    }
                }
            }
            if (Builder.Length > 0)
            {
                tokens.Add(GenerateToken(Builder.ToString()));
                Builder.Clear();
            }
        }

        /// <summary>
        /// 生成单词
        /// </summary>
        /// <param name="value">单词内容</param>
        private static Token GenerateToken(string value)
        {
            Token token = Main.m_ReferencePool.Spawn<Token>();
            token.Type = GetTokenType(value);
            token.Value = value;
            return token;
        }
        /// <summary>
        /// 获取单词类型
        /// </summary>
        /// <param name="value">单词内容</param>
        private static TokenType GetTokenType(string value)
        {
            if (value.StartsWith('#')) return TokenType.Instruct;
            else if (value.StartsWith('[') && value.EndsWith(']') && value.Length >= 3) return TokenType.Identifier;
            else if (value.StartsWith('\"') && value.EndsWith('\"') && value.Length >= 2) return TokenType.String;
            else if (value == "true" || value == "false") return TokenType.Bool;
            else if (value.IsInt()) return TokenType.Int;
            else if (value.IsFloat()) return TokenType.Float;
            else if (value.StartsWith("Vector2(") && value.EndsWith(")")) return TokenType.Vector2;
            else if (value.StartsWith("Vector3(") && value.EndsWith(")")) return TokenType.Vector3;
            else return TokenType.Invalid;
        }
    }
}