using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Light))]
    internal sealed class DebuggerLight : DebuggerComponentBase
    {
        private Light _target;

        public override void OnEnable()
        {
            _target = Target as Light;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            GUILayout.Label("Type: " + _target.type);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensity: ");
            _target.intensity = FloatField(_target.intensity);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Range: ");
            _target.range = FloatField(_target.range);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spot Angle: ");
            _target.spotAngle = FloatField(_target.spotAngle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shadow: ");
            _target.shadows = (LightShadows)EnumField(_target.shadows);
            GUILayout.EndHorizontal();
        }
    }
}