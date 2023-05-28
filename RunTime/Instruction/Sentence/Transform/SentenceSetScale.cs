using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetScale 指令
    /// </summary>
    internal sealed class SentenceSetScale : Sentence
    {
        public Vector3 Scale;

        public override void Execute()
        {
            if (Target)
            {
                Target.transform.localScale = Scale;
            }
        }
    }
}