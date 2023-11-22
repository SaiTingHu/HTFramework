using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(ContentSizeFitter))]
    internal sealed class DebuggerContentSizeFitter : DebuggerComponentBase
    {
        private ContentSizeFitter _target;

        public override void OnEnable()
        {
            _target = Target as ContentSizeFitter;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.horizontalFit = (ContentSizeFitter.FitMode)EnumField("Horizontal Fit", _target.horizontalFit);
            _target.verticalFit = (ContentSizeFitter.FitMode)EnumField("Vertical Fit", _target.verticalFit);
        }
    }
}