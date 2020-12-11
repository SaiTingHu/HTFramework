using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Camera))]
    internal sealed class DebuggerCamera : DebuggerComponentBase
    {
        private Camera _target;

        public override void OnEnable()
        {
            _target = Target as Camera;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");

            GUILayout.Label("Clear Flags: ");
            GUILayout.BeginHorizontal();
            _target.clearFlags = (CameraClearFlags)EnumField(_target.clearFlags);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Field Of View: ");
            _target.fieldOfView = FloatField(_target.fieldOfView);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.fieldOfView = GUILayout.HorizontalSlider(_target.fieldOfView, 1, 179);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Depth: ");
            _target.depth = FloatField(_target.depth);
            GUILayout.EndHorizontal();
        }
    }
}