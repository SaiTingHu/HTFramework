using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepMaster))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/87712995")]
    internal sealed class StepMasterInspector : InternalModuleInspector<StepMaster, IStepMasterHelper>
    {
        private Dictionary<string, StepContent> _stepContentIDs;
        private Dictionary<string, bool> _stepContentEnables;
        private Dictionary<string, string> _customOrder;

        protected override string Intro
        {
            get
            {
                return "Step Master, the stepflow controller!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _stepContentIDs = Target.GetType().GetField("_stepContentIDs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, StepContent>;
            _stepContentEnables = Target.GetType().GetField("_stepContentEnables", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, bool>;
            _customOrder = Target.GetType().GetField("_customOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, string>;
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
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Target.ContentAsset != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Step Count: " + Target.StepCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step: " + Target.CurrentStepIndex);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Skip", EditorStyles.miniButtonLeft))
                {
                    Target.SkipCurrentStep();
                }
                if (GUILayout.Button("SkipImmediate", EditorStyles.miniButtonRight))
                {
                    Target.SkipCurrentStepImmediate();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step Name: " + (Target.CurrentStepContent != null ? Target.CurrentStepContent.Name : "<None>"));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step Target: ");
                EditorGUILayout.ObjectField(Target.CurrentStepContent != null ? Target.CurrentStepContent.Target : null, typeof(GameObject), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step Helper: " + (Target.CurrentStepHelper != null ? Target.CurrentStepHelper.GetType().Name : "<None>"));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Disabled Steps: ");
                GUILayout.EndHorizontal();

                foreach (var step in _stepContentEnables)
                {
                    if (!step.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(_stepContentIDs[step.Key].Name);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Custom Order: " + _customOrder.Count);
                GUILayout.EndHorizontal();

                foreach (var order in _customOrder)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(string.Format("{0} -> {1}", _stepContentIDs[order.Key].Name, _stepContentIDs[order.Value].Name));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("StepMaster Asset is null!");
                GUILayout.EndHorizontal();
            }
        }
    }
}