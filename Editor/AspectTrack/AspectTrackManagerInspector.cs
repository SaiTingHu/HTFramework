using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AspectTrackManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/85617377")]
    internal sealed class AspectTrackManagerInspector : InternalModuleInspector<AspectTrackManager, IAspectTrackHelper>
    {
        protected override string Intro => "Aspect Track Manager, you can track code calls anywhere in the program, or intercept him.";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

#if DISABLE_ASPECTTRACK
            GUI.color = Color.red;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Disabled", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
#else
            PropertyField(nameof(AspectTrackManager.IsEnableAspectTrack), "Enable Track");

            if (Target.IsEnableAspectTrack)
            {
                PropertyField(nameof(AspectTrackManager.IsEnableIntercept), "Enable Intercept");
            }
#endif
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

#if DISABLE_ASPECTTRACK
            GUI.color = Color.red;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Disabled", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
#else
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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Conditions"))
            {
                Target.InterceptConditions.Clear();
            }
            GUILayout.EndHorizontal();
#endif
        }
    }
}