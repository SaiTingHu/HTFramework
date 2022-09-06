using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(CharacterController))]
    internal sealed class DebuggerCharacterController : DebuggerComponentBase
    {
        private CharacterController _target;

        public override void OnEnable()
        {
            _target = Target as CharacterController;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.isTrigger = BoolField("Is Trigger", _target.isTrigger);
            _target.center = Vector3Field("Center", _target.center);
            _target.radius = FloatField("Radius", _target.radius);
            _target.height = FloatField("Height", _target.height);
            BoolField("Is Grounded", _target.isGrounded);
            Vector3Field("Velocity", _target.velocity);
        }
    }
}