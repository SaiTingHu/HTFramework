using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 指令管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Instruction)]
    public sealed class InstructionManager : InternalModuleBase<IInstructionHelper>
    {
        private List<Token> _tokenCache = new List<Token>();
        private Dictionary<string, string> _defines = new Dictionary<string, string>();
        private Dictionary<string, Type> _sentenceCustoms = new Dictionary<string, Type>();

        public override void OnInit()
        {
            base.OnInit();

            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(SentenceCustom)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                CustomInstructionAttribute attribute = types[i].GetCustomAttribute<CustomInstructionAttribute>();
                if (attribute != null)
                {
                    if (!_sentenceCustoms.ContainsKey(attribute.Keyword))
                    {
                        _sentenceCustoms.Add(attribute.Keyword, types[i]);
                    }
                    else
                    {
                        Log.Error($"【指令系统】发现了重复的自定义指令关键字 {attribute.Keyword}！");
                    }
                }
            }
        }

        /// <summary>
        /// 编译指令代码
        /// </summary>
        /// <param name="code">指令代码</param>
        /// <param name="instruction">可执行指令</param>
        public void Compile(string code, Instruction instruction)
        {
            if (string.IsNullOrEmpty(code) || instruction == null)
                return;

            string[] codes = code.Split('\n');
            instruction.Dispose();
            _defines.Clear();
            for (int i = 0; i < codes.Length; i++)
            {
                Main.m_ReferencePool.Despawns(_tokenCache);
                InstructionLexer.Analyze(codes[i], _tokenCache);
                Sentence sentence = InstructionParser.Analyze(_tokenCache, _defines, _sentenceCustoms, i + 1);
                instruction.AddSentence(sentence);
            }
        }
        /// <summary>
        /// 编译指令代码
        /// </summary>
        /// <param name="code">指令代码</param>
        /// <returns>可执行指令</returns>
        public Instruction Compile(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            Instruction instruction = new Instruction();
            Compile(code, instruction);
            return instruction;
        }
    }
}