using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework
{
    [CustomDebugger(typeof(EventSystem))]
    internal sealed class DebuggerEventSystem : DebuggerComponentBase
    {
        private EventSystem _target;

        public override void OnEnable()
        {
            _target = Target as EventSystem;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Current Selected", _target.currentSelectedGameObject);
            ObjectFieldReadOnly("First Selected", _target.firstSelectedGameObject);
            _target.sendNavigationEvents = BoolField("Send Navigation Events", _target.sendNavigationEvents);
            _target.pixelDragThreshold = IntField("Drag Threshold", _target.pixelDragThreshold);
        }
    }
}