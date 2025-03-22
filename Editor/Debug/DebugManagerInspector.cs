using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DebugManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/102570194")]
    internal sealed class DebugManagerInspector : InternalModuleInspector<DebugManager, IDebugHelper>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(DebugManager.IsEnableDebugger), "Enable Debugger");
            
            if (Target.IsEnableDebugger)
            {
                PropertyField(nameof(DebugManager.DebuggerSkin), "Debugger Skin");
                PropertyField(nameof(DebugManager.IsChinese), "Use Chinese");
            }
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}