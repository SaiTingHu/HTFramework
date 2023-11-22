using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(MouseRayUITarget))]
    internal sealed class DebuggerMouseRayUITarget : DebuggerComponentBase
    {
        private MouseRayUITarget _target;

        public override void OnEnable()
        {
            _target = Target as MouseRayUITarget;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.Name = StringField("Name", _target.Name);
            _target.IsStepTarget = BoolField("Is Step Target", _target.IsStepTarget);
            _target.IsOpenPrompt = BoolField("Is Open Prompt", _target.IsOpenPrompt);
            _target.IsOpenHighlight = BoolField("Is Open Highlight", _target.IsOpenHighlight);
            UnityEventField("On Mouse Click ()", _target.OnMouseClick);
        }
    }
}