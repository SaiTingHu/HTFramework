using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #NewObj 指令
    /// </summary>
    internal sealed class SentenceNewObj : Sentence
    {
        public string Name;

        public override void Execute()
        {
            _ = new GameObject(Name);
        }
    }
}