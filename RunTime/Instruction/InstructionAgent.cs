using System;

namespace HT.Framework
{
    /// <summary>
    /// 可执行指令代理者
    /// </summary>
    [Serializable]
    public sealed class InstructionAgent : IDisposable
    {
        /// <summary>
        /// 指令代码
        /// </summary>
        public string Code;
        
        private string _lastCode;
        private Instruction _instruction;

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            if (Main.m_Instruction == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.Instruction, "执行指令失败：未发现指令编译器！");
            }

            if (_instruction == null)
            {
                _lastCode = Code;
                _instruction = Main.m_Instruction.Compile(Code);
            }
            else
            {
                if (_lastCode != Code)
                {
                    _lastCode = Code;
                    Main.m_Instruction.Compile(Code, _instruction);
                }
            }

            if (_instruction != null) _instruction.Execute();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_instruction != null)
            {
                _instruction.Dispose();
                _instruction = null;
            }
        }
    }
}