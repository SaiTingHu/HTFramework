using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(CapsuleCollider))]
    internal sealed class DebuggerCapsuleCollider : DebuggerComponentBase
    {
        private CapsuleCollider _target;

        public override void OnEnable()
        {
            _target = Target as CapsuleCollider;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.isTrigger = GUILayout.Toggle(_target.isTrigger, "Is Trigger");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Center: ", GUILayout.Width(60));
            _target.center = Vector3Field(_target.center);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Direction: ", GUILayout.Width(60));
            _target.direction = IntField(_target.direction);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height: ", GUILayout.Width(60));
            _target.height = FloatField(_target.height);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Radius: ", GUILayout.Width(60));
            _target.radius = FloatField(_target.radius);
            GUILayout.EndHorizontal();
        }
    }
}