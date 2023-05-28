namespace HT.Framework
{
    /// <summary>
    /// #Active 指令
    /// </summary>
    internal sealed class SentenceActive : Sentence
    {
        public bool Active;

        public override void Execute()
        {
            if (Target)
            {
                Target.SetActive(Active);
            }
        }
    }
}