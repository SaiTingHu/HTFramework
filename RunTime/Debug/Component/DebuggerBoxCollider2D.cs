using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(BoxCollider2D))]
    internal sealed class DebuggerBoxCollider2D : DebuggerComponentBase
    {
        private BoxCollider2D _target;

        public override void OnEnable()
        {
            _target = Target as BoxCollider2D;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.isTrigger = BoolField("Is Trigger", _target.isTrigger);
            ObjectFieldReadOnly("Material", _target.sharedMaterial);
            _target.usedByEffector = BoolField("Used By Effector", _target.usedByEffector);
            _target.usedByComposite = BoolField("Used By Composite", _target.usedByComposite);
            _target.autoTiling = BoolField("Auto Tiling", _target.autoTiling);
            _target.offset = Vector2Field("Offset", _target.offset);
            _target.size = Vector2Field("Size", _target.size);
            _target.edgeRadius = FloatField("Edge Radius", _target.edgeRadius);
        }
    }
}