using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(CanvasGroup))]
    internal sealed class DebuggerCanvasGroup : DebuggerComponentBase
    {
        private CanvasGroup _target;

        public override void OnEnable()
        {
            _target = Target as CanvasGroup;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.alpha = FloatField("Alpha", _target.alpha);
            _target.interactable = BoolField("Interactable", _target.interactable);
            _target.blocksRaycasts = BoolField("Blocks Raycasts", _target.blocksRaycasts);
            _target.ignoreParentGroups = BoolField("Ignore Parent Groups", _target.ignoreParentGroups);
        }
    }
}