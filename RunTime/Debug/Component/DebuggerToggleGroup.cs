using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(ToggleGroup))]
    internal sealed class DebuggerToggleGroup : DebuggerComponentBase
    {
        private ToggleGroup _target;

        public override void OnEnable()
        {
            _target = Target as ToggleGroup;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.allowSwitchOff = BoolField("Allow Switch Off", _target.allowSwitchOff);
        }
    }
}