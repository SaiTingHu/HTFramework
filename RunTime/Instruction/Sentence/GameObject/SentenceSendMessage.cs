using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// #SendMessage 指令
    /// </summary>
    internal sealed class SentenceSendMessage : Sentence
    {
        public string MethodName;
        public ArgsType Type;
        public string StringArgs;
        public bool BoolArgs;
        public int IntArgs;
        public float FloatArgs;
        public Vector2 Vector2Args;
        public Vector3 Vector3Args;

        public override void Execute()
        {
            if (Target)
            {
                switch (Type)
                {
                    case ArgsType.String:
                        Target.SendMessage(MethodName, StringArgs);
                        break;
                    case ArgsType.Bool:
                        Target.SendMessage(MethodName, BoolArgs);
                        break;
                    case ArgsType.Int:
                        Target.SendMessage(MethodName, IntArgs);
                        break;
                    case ArgsType.Float:
                        Target.SendMessage(MethodName, FloatArgs);
                        break;
                    case ArgsType.Vector2:
                        Target.SendMessage(MethodName, Vector2Args);
                        break;
                    case ArgsType.Vector3:
                        Target.SendMessage(MethodName, Vector3Args);
                        break;
                    case ArgsType.None:
                        Target.SendMessage(MethodName);
                        break;
                }
            }
        }
    }
}