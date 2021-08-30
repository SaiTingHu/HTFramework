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
        protected override string Intro => "Task Master, the tasks controller!";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(TaskMaster.ContentAsset), "Asset");
            PropertyField(nameof(TaskMaster.GuideHighlighting), "Guide Highlighting");
            
            switch (Target.GuideHighlighting)
            {
                case MouseRay.HighlightingType.Normal:
                    PropertyField(nameof(TaskMaster.NormalColor), "Normal Color");
                    break;
                case MouseRay.HighlightingType.Flash:
                    PropertyField(nameof(TaskMaster.FlashColor1), "Flash Color 1");
                    PropertyField(nameof(TaskMaster.FlashColor2), "Flash Color 2");
                    break;
                case MouseRay.HighlightingType.Outline:
                    PropertyField(nameof(TaskMaster.NormalColor), "Outline Color");
                    PropertyField(nameof(TaskMaster.OutlineIntensity), "Outline Intensity");
                    break;
            }

            PropertyField(nameof(TaskMaster.IsAutoChange), "Auto Change");
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
                if (GUILayout.Button("Auto Complete", EditorStyles.miniButton))
                {
                    Target.AutoCompleteCurrentTaskContent();
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