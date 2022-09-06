using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Animator))]
    internal sealed class DebuggerAnimator : DebuggerComponentBase
    {
        private Animator _target;

        public override void OnEnable()
        {
            _target = Target as Animator;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Controller", _target.runtimeAnimatorController);
            ObjectFieldReadOnly("Avatar", _target.avatar);
            _target.applyRootMotion = BoolField("Apply Root Motion", _target.applyRootMotion);
            _target.updateMode = (AnimatorUpdateMode)EnumField("Update Mode", _target.updateMode);
            _target.cullingMode = (AnimatorCullingMode)EnumField("Culling Mode", _target.cullingMode);
            _target.speed = FloatField("Speed", _target.speed);
        }
    }
}