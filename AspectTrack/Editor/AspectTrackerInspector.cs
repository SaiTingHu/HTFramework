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
            GUI.color = _target.IsEnableAspectTrack ? Color.white : Color.gray;
            _target.IsEnableAspectTrack = GUILayout.Toggle(_target.IsEnableAspectTrack, "Is Track");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
