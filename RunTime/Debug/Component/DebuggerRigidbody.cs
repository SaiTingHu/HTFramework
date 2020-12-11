using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Rigidbody))]
    internal sealed class DebuggerRigidbody : DebuggerComponentBase
    {
        private Rigidbody _target;

        public override void OnEnable()
        {
            _target = Target as Rigidbody;
        }

        public override void OnDebuggerGUI()
        {
            _target.useGravity = GUILayout.Toggle(_target.useGravity, "Use Gravity");
            _target.isKinematic = GUILayout.Toggle(_target.isKinematic, "Is Kinematic");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Velocity:", GUILayout.Width(60));
            _target.velocity = Vector3Field(_target.velocity);
            GUILayout.EndHorizontal();
        }
    }
}