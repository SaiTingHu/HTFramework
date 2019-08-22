using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AspectTracker))]
    public sealed class AspectTrackerInspector : HTFEditor<AspectTracker>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Aspect Tracker, you can track code calls anywhere in the program!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableAspectTrack, out Target.IsEnableAspectTrack, "Is Track");
            GUILayout.EndHorizontal();
        }
    }
}
