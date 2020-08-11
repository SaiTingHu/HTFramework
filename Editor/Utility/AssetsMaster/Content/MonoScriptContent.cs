using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 脚本资产
    /// </summary>
    internal sealed class MonoScriptContent : AssetContent
    {
        public MonoScript Script;

        public MonoScriptContent(MonoBehaviour mono)
        {
            Script = MonoScript.FromMonoBehaviour(mono);
        }
    }
}