using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Canvas))]
    internal sealed class DebuggerCanvas : DebuggerComponentBase
    {
        private Canvas _target;

        public override void OnEnable()
        {
            _target = Target as Canvas;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            StringFieldReadOnly("Render Mode", _target.renderMode.ToString());
            _target.pixelPerfect = BoolField("Pixel Perfect", _target.pixelPerfect);
            _target.sortingOrder = IntField("Sort Order", _target.sortingOrder);
            StringFieldReadOnly("Target Display", $"Display {_target.targetDisplay}");
        }
    }
}