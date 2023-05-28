using System.Collections.Generic;

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
                Sentence sentence = InstructionParser.Analyze(_tokenCache, _defines, i + 1);
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