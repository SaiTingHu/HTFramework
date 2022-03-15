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

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.interactable = BoolField("Interactable", _target.interactable);
            _target.direction = (Slider.Direction)EnumField("Direction", _target.direction);
            _target.minValue = FloatField("Min Value", _target.minValue);
            _target.maxValue = FloatField("Max Value", _target.maxValue);
            _target.wholeNumbers = BoolField("Whole Numbers", _target.wholeNumbers);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value", GUILayout.Width(120));
            _target.value = GUILayout.HorizontalSlider(_target.value, _target.minValue, _target.maxValue);
            GUILayout.EndHorizontal();
        }
    }
}