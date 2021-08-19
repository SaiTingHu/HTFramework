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
            EnumPopup(Target.GuideHighlighting, out Target.GuideHighlighting, "Guide Highlighting");
            GUILayout.EndHorizontal();

            switch (Target.GuideHighlighting)
            {
                case MouseRay.HighlightingType.Normal:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.NormalColor, out Target.NormalColor, "Normal Color");
                    GUILayout.EndHorizontal();
                    break;
                case MouseRay.HighlightingType.Flash:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.FlashColor1, out Target.FlashColor1, "Flash Color 1");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    ColorField(Target.FlashColor2, out Target.FlashColor2, "Flash Color 2");
                    GUILayout.EndHorizontal();
                    break;
                case MouseRay.HighlightingType.Outline:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.NormalColor, out Target.NormalColor, "Outline Color");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    FloatField(Target.OutlineIntensity, out Target.OutlineIntensity, "Outline Intensity");
                    GUILayout.EndHorizontal();
                    break;
            }

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