namespace HT.Framework
{
    /// <summary>
    /// #Rename 指令
    /// </summary>
    internal sealed class SentenceRename : Sentence
    {
        public string Name;

        public override void Execute()
        {
            if (Target)
            {
                Target.name = Name;
            }
        }
    }
}