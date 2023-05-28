using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 可执行指令
    /// </summary>
    public sealed class Instruction : IDisposable
    {
        /// <summary>
        /// 所有的可执行语句
        /// </summary>
        private List<Sentence> _sentences = new List<Sentence>();

        /// <summary>
        /// 添加可执行语句
        /// </summary>
        /// <param name="sentence">可执行语句</param>
        internal void AddSentence(Sentence sentence)
        {
            if (sentence != null)
            {
                _sentences.Add(sentence);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            for (int i = 0; i < _sentences.Count; i++)
            {
                _sentences[i].Execute();
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < _sentences.Count; i++)
            {
                Main.m_ReferencePool.Despawn(_sentences[i]);
            }
            _sentences.Clear();
        }
    }
}