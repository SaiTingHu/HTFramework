using System;
using UnityEngine;

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
        [Multiline(10)] public string Code;
        
        private string _lastCode;
        private Instruction _instruction;

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
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
            _instruction.Execute();
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