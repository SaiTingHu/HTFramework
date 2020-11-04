using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AspectTracker))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/85617377")]
    internal sealed class AspectTrackerInspector : InternalModuleInspector<AspectTracker>
    {
        protected override string Intro
        {
            get
            {
                return "Aspect Tracker, you can track code calls anywhere in the program, or intercept him.";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IAspectTrackHelper);
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();
            
            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableAspectTrack, out Target.IsEnableAspectTrack, "Is Enable Track");
            GUILayout.EndHorizontal();

            if (Target.IsEnableAspectTrack)
            {
                GUILayout.BeginHorizontal();
                Toggle(Target.IsEnableIntercept, out Target.IsEnableIntercept, "Is Enable Intercept");
                GUILayout.EndHorizontal();
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intercept Conditions: ");
            GUILayout.EndHorizontal();

            foreach (var condition in Target.InterceptConditions)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(condition.Key);
                GUILayout.EndHorizontal();
            }
        }
    }
}