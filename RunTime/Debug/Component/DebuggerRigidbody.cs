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
            _target.mass = FloatField("Mass", _target.mass);
            _target.drag = FloatField("Drag", _target.drag);
            _target.angularDrag = FloatField("Angular Drag", _target.angularDrag);
            _target.useGravity = BoolField("Use Gravity", _target.useGravity);
            _target.isKinematic = BoolField("Is Kinematic", _target.isKinematic);
            _target.velocity = Vector3Field("Velocity", _target.velocity);
        }
    }
}