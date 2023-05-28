using System;

namespace HT.Framework
{
    /// <summary>
    /// #AddCom 指令
    /// </summary>
    internal sealed class SentenceAddCom : Sentence
    {
        public string ComType;

        public override void Execute()
        {
            if (Target)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(ComType);
                if (type != null) Target.AddComponent(type);
            }
        }
    }
}