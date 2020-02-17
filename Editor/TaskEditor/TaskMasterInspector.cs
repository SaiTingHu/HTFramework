using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(TaskMaster))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/104317219")]
    internal sealed class TaskMasterInspector : HTFEditor<TaskMaster>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Task Master, the tasks controller!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ObjectField(Target.ContentAsset, out Target.ContentAsset, false, "Asset");
            GUILayout.EndHorizontal();
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