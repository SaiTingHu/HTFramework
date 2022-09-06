using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Toggle))]
    internal sealed class DebuggerToggle : DebuggerComponentBase
    {
        private Toggle _target;

        public override void OnEnable()
        {
            _target = Target as Toggle;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.interactable = BoolField("Interactable", _target.interactable);
            _target.isOn = BoolField("Is On", _target.isOn);
        }
    }
}