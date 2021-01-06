using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 脚本资产
    /// </summary>
    internal sealed class MonoScriptContent : AssetContent
    {
        public MonoScript Script { get; private set; }
        public string Assembly { get; private set; }
        public bool IsMono { get; private set; }

        public MonoScriptContent(MonoBehaviour mono)
        {
            Script = MonoScript.FromMonoBehaviour(mono);
            Assembly = mono.GetType().Assembly.GetName().Name;
            IsMono = true;
        }
        public MonoScriptContent(ScriptableObject scriptObj)
        {
            Script = MonoScript.FromScriptableObject(scriptObj);
            Assembly = scriptObj.GetType().Assembly.GetName().Name;
            IsMono = false;
        }
    }
}