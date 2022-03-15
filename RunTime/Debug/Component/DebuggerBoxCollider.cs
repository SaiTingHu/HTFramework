using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(BoxCollider))]
    internal sealed class DebuggerBoxCollider : DebuggerComponentBase
    {
        private BoxCollider _target;
        
        public override void OnEnable()
        {
            _target = Target as BoxCollider;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.isTrigger = BoolField("Is Trigger", _target.isTrigger);
            ObjectFieldReadOnly("Material", _target.sharedMaterial);
            _target.center = Vector3Field("Center", _target.center);
            _target.size = Vector3Field("Size", _target.size);
        }
    }
}