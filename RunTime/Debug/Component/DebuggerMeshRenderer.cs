using UnityEngine;
using UnityEngine.Rendering;

namespace HT.Framework
{
    [CustomDebugger(typeof(MeshRenderer))]
    internal sealed class DebuggerMeshRenderer : DebuggerComponentBase
    {
        private MeshRenderer _target;

        public override void OnEnable()
        {
            _target = Target as MeshRenderer;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.receiveShadows = BoolField("Receive Shadows", _target.receiveShadows);
            _target.shadowCastingMode = (ShadowCastingMode)EnumField("Cast Shadows", _target.shadowCastingMode);
            MaterialsFieldReadOnly("Materials", _target.sharedMaterials);
        }
    }
}