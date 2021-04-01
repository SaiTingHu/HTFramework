using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(TaskMaster))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/104317219")]
    internal sealed class TaskMasterInspector : InternalModuleInspector<TaskMaster, ITaskMasterHelper>
    {
        protected override string Intro
        {
            get
            {
                return "Task Master, the tasks controller!";
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            ObjectField(Target.ContentAsset, out Target.ContentAsset, false, "Asset");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsAutoChange, out Target.IsAutoChange, "Auto Change");
            GUILayout.EndHorizontal();
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Target.ContentAsset != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Task Content Count: " + Target.AllTaskContent.Count);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Task: " + (Target.CurrentTaskContent != null ? Target.CurrentTaskContent.Name : "<None>"));
                GUILayout.FlexibleSpace();
                GUI.enabled = Target.CurrentTaskContent != null;
                if (GUILayout.Button("Complete", EditorStyles.miniButton))
                {
                    Target.CompleteCurrentTaskContent();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("TaskMaster Asset is null!");
                GUILayout.EndHorizontal();
            }
        }
    }
}