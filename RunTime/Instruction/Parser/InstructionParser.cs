using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    internal static class InstructionParser
    {
        /// <summary>
        /// 语法分析
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        public static Sentence Analyze(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens != null && tokens.Count > 0)
            {
                Token invalid = tokens.Find((t) => { return t.Type == TokenType.Invalid; });
                if (invalid != null)
                {
                    Log.Error($"【指令系统】语法解析错误：发现了无效的字符 {invalid.Value}！[第{line}行]");
                    return null;
                }

                if (tokens[0].Type == TokenType.Instruct)
                {
                    switch (tokens[0].Value)
                    {
                        case "#Define":
                            return GenerateSentenceDefine(tokens, defines, line);
                        case "#AddCom":
                            return GenerateSentenceAddCom(tokens, defines, line);
                        case "#RemoveCom":
                            return GenerateSentenceRemoveCom(tokens, defines, line);
                        case "#SetField":
                            return GenerateSentenceSetField(tokens, defines, line);
                        case "#SetProperty":
                            return GenerateSentenceSetProperty(tokens, defines, line);
                        case "#Active":
                            return GenerateSentenceActive(tokens, defines, line);
                        case "#DeleteObj":
                            return GenerateSentenceDeleteObj(tokens, defines, line);
                        case "#NewObj":
                            return GenerateSentenceNewObj(tokens, defines, line);
                        case "#Rename":
                            return GenerateSentenceRename(tokens, defines, line);
                        case "#SendMessage":
                            return GenerateSentenceSendMessage(tokens, defines, line);
                        case "#SetParent":
                            return GenerateSentenceSetParent(tokens, defines, line);
                        case "#SetPosition":
                            return GenerateSentenceSetPosition(tokens, defines, line);
                        case "#SetRotation":
                            return GenerateSentenceSetRotation(tokens, defines, line);
                        case "#SetScale":
                            return GenerateSentenceSetScale(tokens, defines, line);
                        default:
                            Log.Error($"【指令系统】语法解析错误：无法识别指令关键字 {tokens[0].Value}！[第{line}行]");
                            break;
                    }
                }
                else
                {
                    Log.Error($"【指令系统】语法解析错误：必须以指令关键字开头！[第{line}行]");
                }
            }
            return null;
        }

        /// <summary>
        /// 生成语句（#Define）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static Sentence GenerateSentenceDefine(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#Define指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier)
            {
                Log.Error($"【指令系统】语法解析错误：#Define指令的第一个参数必须为标识符类型（[]包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#Define指令的第二个参数必须为String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            string identifier = tokens[1].Value;
            string value = tokens[2].Value.Replace("\"", "");
            if (!defines.ContainsKey(identifier))
            {
                defines.Add(identifier, value);
            }
            else
            {
                Log.Error($"【指令系统】语法解析错误：#Define指令定义了重复的标识符{identifier}！[第{line}行]");
            }
            return null;
        }
        /// <summary>
        /// 生成语句（#AddCom）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceAddCom GenerateSentenceAddCom(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#AddCom指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#AddCom指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#AddCom指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceAddCom sentence = Main.m_ReferencePool.Spawn<SentenceAddCom>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.ComType = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#RemoveCom）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceRemoveCom GenerateSentenceRemoveCom(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#RemoveCom指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#RemoveCom指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#RemoveCom指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceRemoveCom sentence = Main.m_ReferencePool.Spawn<SentenceRemoveCom>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.ComType = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetField）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetField GenerateSentenceSetField(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 5)
            {
                Log.Error($"【指令系统】语法解析错误：#SetField指令必须跟四个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetField指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetField指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[3].Type != TokenType.Identifier && tokens[3].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetField指令的第三个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (!IsValueType(tokens[4].Type))
            {
                Log.Error($"【指令系统】语法解析错误：#SetField指令的第四个参数必须为值类型，不能为{tokens[4].Type}类型！[第{line}行]");
                return null;
            }

            SentenceSetField sentence = Main.m_ReferencePool.Spawn<SentenceSetField>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.ComType = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            sentence.FieldName = ConvertIdentifier(tokens[3].Type, tokens[3].Value.Replace("\"", ""), defines, line);
            if (tokens[4].Type == TokenType.String)
            {
                sentence.Type = ArgsType.String;
                sentence.StringArgs = tokens[4].Value.Replace("\"", "");
            }
            else if (tokens[4].Type == TokenType.Bool)
            {
                sentence.Type = ArgsType.Bool;
                sentence.BoolArgs = tokens[4].Value == "true";
            }
            else if (tokens[4].Type == TokenType.Int)
            {
                sentence.Type = ArgsType.Int;
                sentence.IntArgs = int.Parse(tokens[4].Value);
            }
            else if (tokens[4].Type == TokenType.Float)
            {
                sentence.Type = ArgsType.Float;
                sentence.FloatArgs = float.Parse(tokens[4].Value);
            }
            else if (tokens[4].Type == TokenType.Vector2)
            {
                sentence.Type = ArgsType.Vector2;
                sentence.Vector2Args = tokens[4].Value.ToVector2();
            }
            else if (tokens[4].Type == TokenType.Vector3)
            {
                sentence.Type = ArgsType.Vector3;
                sentence.Vector3Args = tokens[4].Value.ToVector3();
            }
            else
            {
                sentence.Type = ArgsType.String;
                sentence.StringArgs = tokens[4].Value.Replace("\"", "");
            }
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetProperty）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetProperty GenerateSentenceSetProperty(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 5)
            {
                Log.Error($"【指令系统】语法解析错误：#SetProperty指令必须跟四个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetProperty指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetProperty指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[3].Type != TokenType.Identifier && tokens[3].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetProperty指令的第三个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (!IsValueType(tokens[4].Type))
            {
                Log.Error($"【指令系统】语法解析错误：#SetProperty指令的第四个参数必须为值类型，不能为{tokens[4].Type}类型！[第{line}行]");
                return null;
            }

            SentenceSetProperty sentence = Main.m_ReferencePool.Spawn<SentenceSetProperty>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.ComType = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            sentence.PropertyName = ConvertIdentifier(tokens[3].Type, tokens[3].Value.Replace("\"", ""), defines, line);
            if (tokens[4].Type == TokenType.String)
            {
                sentence.Type = ArgsType.String;
                sentence.StringArgs = tokens[4].Value.Replace("\"", "");
            }
            else if (tokens[4].Type == TokenType.Bool)
            {
                sentence.Type = ArgsType.Bool;
                sentence.BoolArgs = tokens[4].Value == "true";
            }
            else if (tokens[4].Type == TokenType.Int)
            {
                sentence.Type = ArgsType.Int;
                sentence.IntArgs = int.Parse(tokens[4].Value);
            }
            else if (tokens[4].Type == TokenType.Float)
            {
                sentence.Type = ArgsType.Float;
                sentence.FloatArgs = float.Parse(tokens[4].Value);
            }
            else if (tokens[4].Type == TokenType.Vector2)
            {
                sentence.Type = ArgsType.Vector2;
                sentence.Vector2Args = tokens[4].Value.ToVector2();
            }
            else if (tokens[4].Type == TokenType.Vector3)
            {
                sentence.Type = ArgsType.Vector3;
                sentence.Vector3Args = tokens[4].Value.ToVector3();
            }
            else
            {
                sentence.Type = ArgsType.String;
                sentence.StringArgs = tokens[4].Value.Replace("\"", "");
            }
            return sentence;
        }
        /// <summary>
        /// 生成语句（#Active）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceActive GenerateSentenceActive(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#Active指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#Active指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Bool)
            {
                Log.Error($"【指令系统】语法解析错误：#Active指令的第二个参数必须为Bool类型！[第{line}行]");
                return null;
            }

            SentenceActive sentence = Main.m_ReferencePool.Spawn<SentenceActive>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.Active = tokens[2].Value == "true";
            return sentence;
        }
        /// <summary>
        /// 生成语句（#DeleteObj）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceDeleteObj GenerateSentenceDeleteObj(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 2)
            {
                Log.Error($"【指令系统】语法解析错误：#DeleteObj指令必须跟一个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#DeleteObj指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceDeleteObj sentence = Main.m_ReferencePool.Spawn<SentenceDeleteObj>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#NewObj）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceNewObj GenerateSentenceNewObj(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 2)
            {
                Log.Error($"【指令系统】语法解析错误：#NewObj指令必须跟一个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#NewObj指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceNewObj sentence = Main.m_ReferencePool.Spawn<SentenceNewObj>();
            sentence.TargetPath = null;
            sentence.Name = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#Rename）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceRename GenerateSentenceRename(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#Rename指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#Rename指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#Rename指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceRename sentence = Main.m_ReferencePool.Spawn<SentenceRename>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.Name = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SendMessage）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSendMessage GenerateSentenceSendMessage(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3 && tokens.Count != 4)
            {
                Log.Error($"【指令系统】语法解析错误：#SendMessage指令必须跟两个或三个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SendMessage指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SendMessage指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceSendMessage sentence = Main.m_ReferencePool.Spawn<SentenceSendMessage>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.MethodName = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            if (tokens.Count == 4)
            {
                if (tokens[3].Type == TokenType.String)
                {
                    sentence.Type = ArgsType.String;
                    sentence.StringArgs = tokens[3].Value.Replace("\"", "");
                }
                else if (tokens[3].Type == TokenType.Bool)
                {
                    sentence.Type = ArgsType.Bool;
                    sentence.BoolArgs = tokens[3].Value == "true";
                }
                else if (tokens[3].Type == TokenType.Int)
                {
                    sentence.Type = ArgsType.Int;
                    sentence.IntArgs = int.Parse(tokens[3].Value);
                }
                else if (tokens[3].Type == TokenType.Float)
                {
                    sentence.Type = ArgsType.Float;
                    sentence.FloatArgs = float.Parse(tokens[3].Value);
                }
                else if (tokens[3].Type == TokenType.Vector2)
                {
                    sentence.Type = ArgsType.Vector2;
                    sentence.Vector2Args = tokens[3].Value.ToVector2();
                }
                else if (tokens[3].Type == TokenType.Vector3)
                {
                    sentence.Type = ArgsType.Vector3;
                    sentence.Vector3Args = tokens[3].Value.ToVector3();
                }
                else
                {
                    sentence.Type = ArgsType.None;
                }
            }
            else
            {
                sentence.Type = ArgsType.None;
            }
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetParent）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetParent GenerateSentenceSetParent(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#SetParent指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetParent指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Identifier && tokens[2].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetParent指令的第二个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }

            SentenceSetParent sentence = Main.m_ReferencePool.Spawn<SentenceSetParent>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.ParentPath = ConvertIdentifier(tokens[2].Type, tokens[2].Value.Replace("\"", ""), defines, line);
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetPosition）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetPosition GenerateSentenceSetPosition(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 4)
            {
                Log.Error($"【指令系统】语法解析错误：#SetPosition指令必须跟三个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetPosition指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Vector3)
            {
                Log.Error($"【指令系统】语法解析错误：#SetPosition指令的第二个参数必须为Vector3类型！[第{line}行]");
                return null;
            }
            if (tokens[3].Type != TokenType.Bool)
            {
                Log.Error($"【指令系统】语法解析错误：#SetPosition指令的第三个参数必须为Bool类型！[第{line}行]");
                return null;
            }

            SentenceSetPosition sentence = Main.m_ReferencePool.Spawn<SentenceSetPosition>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.Position = tokens[2].Value.ToVector3();
            sentence.IsWorld = tokens[3].Value == "true";
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetRotation）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetRotation GenerateSentenceSetRotation(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 4)
            {
                Log.Error($"【指令系统】语法解析错误：#SetRotation指令必须跟三个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetRotation指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Vector3)
            {
                Log.Error($"【指令系统】语法解析错误：#SetRotation指令的第二个参数必须为Vector3类型！[第{line}行]");
                return null;
            }
            if (tokens[3].Type != TokenType.Bool)
            {
                Log.Error($"【指令系统】语法解析错误：#SetRotation指令的第三个参数必须为Bool类型！[第{line}行]");
                return null;
            }

            SentenceSetRotation sentence = Main.m_ReferencePool.Spawn<SentenceSetRotation>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.Rotation = tokens[2].Value.ToVector3();
            sentence.IsWorld = tokens[3].Value == "true";
            return sentence;
        }
        /// <summary>
        /// 生成语句（#SetScale）
        /// </summary>
        /// <param name="tokens">单词序列</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>可执行语句</returns>
        private static SentenceSetScale GenerateSentenceSetScale(List<Token> tokens, Dictionary<string, string> defines, int line)
        {
            if (tokens.Count != 3)
            {
                Log.Error($"【指令系统】语法解析错误：#SetScale指令必须跟两个参数！[第{line}行]");
                return null;
            }
            if (tokens[1].Type != TokenType.Identifier && tokens[1].Type != TokenType.String)
            {
                Log.Error($"【指令系统】语法解析错误：#SetScale指令的第一个参数只能为标识符（[]包裹）或String类型（双引号包裹）！[第{line}行]");
                return null;
            }
            if (tokens[2].Type != TokenType.Vector3)
            {
                Log.Error($"【指令系统】语法解析错误：#SetScale指令的第二个参数必须为Vector3类型！[第{line}行]");
                return null;
            }

            SentenceSetScale sentence = Main.m_ReferencePool.Spawn<SentenceSetScale>();
            sentence.TargetPath = ConvertIdentifier(tokens[1].Type, tokens[1].Value.Replace("\"", ""), defines, line);
            sentence.Scale = tokens[2].Value.ToVector3();
            return sentence;
        }

        /// <summary>
        /// 单词类型是否为值类型
        /// </summary>
        /// <param name="type">单词类型</param>
        /// <returns>是否为值类型</returns>
        private static bool IsValueType(TokenType type)
        {
            switch (type)
            {
                case TokenType.String:
                case TokenType.Bool:
                case TokenType.Int:
                case TokenType.Float:
                case TokenType.Vector2:
                case TokenType.Vector3:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 转换标识符
        /// </summary>
        /// <param name="type">单词类型</param>
        /// <param name="identifier">标识符</param>
        /// <param name="defines">标识符定义</param>
        /// <param name="line">源码行数</param>
        /// <returns>标识符定义的值</returns>
        private static string ConvertIdentifier(TokenType type, string identifier, Dictionary<string, string> defines, int line)
        {
            if (type == TokenType.Identifier)
            {
                if (defines.ContainsKey(identifier))
                {
                    return defines[identifier];
                }
                else
                {
                    Log.Error($"【指令系统】语法解析错误：发现了未定义的标识符{identifier}！[第{line}行]");
                }
            }
            return identifier;
        }
    }
}