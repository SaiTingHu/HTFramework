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
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed: ");
            _target.speed = FloatField(_target.speed);
            GUILayout.EndHorizontal();
        }
    }
}