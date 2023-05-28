using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetPosition 指令
    /// </summary>
    internal sealed class SentenceSetPosition : Sentence
    {
        public Vector3 Position;
        public bool IsWorld;

        public override void Execute()
        {
            if (Target)
            {
                if (IsWorld) Target.transform.position = Position;
                else Target.transform.localPosition = Position;
            }
        }
    }
}