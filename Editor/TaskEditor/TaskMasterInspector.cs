using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(TaskMaster))]
    [GithubURL("")]
    [CSDNBlogURL("")]
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
    }
}