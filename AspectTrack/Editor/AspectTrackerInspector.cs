using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(AspectTracker))]
    public sealed class AspectTrackerInspector : Editor
    {
        private AspectTracker _target;

        private void OnEnable()
        {
            _target = target as AspectTracker;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Aspect Tracker, you can track code calls anywhere in the program!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.Toggle(_target.IsEnableAspectTrack, out _target.IsEnableAspectTrack, "Is Track");
            GUILayout.EndHorizontal();
        }
    }
}
