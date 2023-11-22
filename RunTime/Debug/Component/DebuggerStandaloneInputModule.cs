using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    [CustomDebugger(typeof(StandaloneInputModule))]
    public class DebuggerStandaloneInputModule : DebuggerComponentBase
    {
        private StandaloneInputModule _target;

        public override void OnEnable()
        {
            _target = Target as StandaloneInputModule;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.horizontalAxis = StringField("Horizontal Axis", _target.horizontalAxis);
            _target.verticalAxis = StringField("Vertical Axis", _target.verticalAxis);
            _target.submitButton = StringField("Submit Button", _target.submitButton);
            _target.cancelButton = StringField("Cancel Button", _target.cancelButton);
            _target.inputActionsPerSecond = FloatField("Input Actions Per Second", _target.inputActionsPerSecond);
            _target.repeatDelay = FloatField("Repeat Delay", _target.repeatDelay);
        }
    }
}