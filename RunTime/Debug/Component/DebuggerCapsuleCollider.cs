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

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.isTrigger = BoolField("Is Trigger", _target.isTrigger);
            ObjectFieldReadOnly("Material", _target.sharedMaterial);
            _target.center = Vector3Field("Center", _target.center);
            _target.radius = FloatField("Radius", _target.radius);
            _target.height = FloatField("Height", _target.height);
        }
    }
}