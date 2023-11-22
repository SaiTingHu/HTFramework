using UnityEngine;
using UnityEngine.Rendering;

namespace HT.Framework
{
    [CustomDebugger(typeof(SkinnedMeshRenderer))]
    internal sealed class DebuggerSkinnedMeshRenderer : DebuggerComponentBase
    {
        private SkinnedMeshRenderer _target;

        public override void OnEnable()
        {
            _target = Target as SkinnedMeshRenderer;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.receiveShadows = BoolField("Receive Shadows", _target.receiveShadows);
            _target.shadowCastingMode = (ShadowCastingMode)EnumField("Cast Shadows", _target.shadowCastingMode);
            ObjectFieldReadOnly("Mesh", _target.sharedMesh);
            IntFieldReadOnly("Bones", _target.bones.Length);
            MaterialsFieldReadOnly("Materials", _target.materials);
        }
    }
}