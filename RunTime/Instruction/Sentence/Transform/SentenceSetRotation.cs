using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SetRotation 指令
    /// </summary>
    internal sealed class SentenceSetRotation : Sentence
    {
        public Vector3 Rotation;
        public bool IsWorld;

        public override void Execute()
        {
            if (Target)
            {
                if (IsWorld) Target.transform.eulerAngles = Rotation;
                else Target.transform.localEulerAngles = Rotation;
            }
        }
    }
}