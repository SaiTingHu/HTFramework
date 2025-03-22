using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(InstructionManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/130918484")]
    internal sealed class InstructionManagerInspector : InternalModuleInspector<InstructionManager, IInstructionHelper>
    {
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}