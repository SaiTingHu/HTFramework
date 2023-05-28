using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #DeleteObj 指令
    /// </summary>
    internal sealed class SentenceDeleteObj : Sentence
    {
        public override void Execute()
        {
            if (Target)
            {
                Object.Destroy(Target);
            }
        }
    }
}