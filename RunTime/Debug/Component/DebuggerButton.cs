using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Button))]
    internal sealed class DebuggerButton : DebuggerComponentBase
    {
        private Button _target;

        public override void OnEnable()
        {
            _target = Target as Button;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.interactable = BoolField("Interactable", _target.interactable);
            UnityEventField("On Click ()", _target.onClick);
        }
    }
}