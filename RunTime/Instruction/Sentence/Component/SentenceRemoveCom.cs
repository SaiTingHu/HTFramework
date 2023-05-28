using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #RemoveCom 指令
    /// </summary>
    internal sealed class SentenceRemoveCom : Sentence
    {
        public string ComType;

        public override void Execute()
        {
            if (Target)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(ComType);
                if (type != null)
                {
                    Component component = Target.GetComponent(type);
                    if (component) UnityEngine.Object.Destroy(component);
                }
            }
        }
    }
}