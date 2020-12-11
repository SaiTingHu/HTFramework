using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Slider))]
    internal sealed class DebuggerSlider : DebuggerComponentBase
    {
        private Slider _target;

        public override void OnEnable()
        {
            _target = Target as Slider;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.interactable = GUILayout.Toggle(_target.interactable, "Interactable");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value: ");
            _target.value = GUILayout.HorizontalSlider(_target.value, _target.minValue, _target.maxValue);
            GUILayout.EndHorizontal();
        }
    }
}