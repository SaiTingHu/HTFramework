using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Light))]
    internal sealed class DebuggerLight : DebuggerComponentBase
    {
        private Light _target;

        public override void OnEnable()
        {
            _target = Target as Light;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.type = (LightType)EnumField("Type", _target.type);
            if (_target.type != LightType.Directional)
            {
                _target.range = FloatField("Range", _target.range);
            }
            if (_target.type == LightType.Spot)
            {
                _target.spotAngle = FloatField("Spot Angle", _target.spotAngle);
            }
            _target.intensity = FloatField("Intensity", _target.intensity);
            _target.shadows = (LightShadows)EnumField("Shadow Type", _target.shadows);
        }
    }
}