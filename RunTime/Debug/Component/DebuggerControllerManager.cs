namespace HT.Framework
{
    [CustomDebugger(typeof(ControllerManager))]
    internal sealed class DebuggerControllerManager : DebuggerComponentBase
    {
        private ControllerManager _target;

        public override void OnEnable()
        {
            _target = Target as ControllerManager;
        }
        public override void OnDebuggerGUI()
        {
            _target.IsEnableBounds = BoolField("Is Enable Bounds", _target.IsEnableBounds);
            _target.EnablePositionControl = BoolField("Enable Position Control", _target.EnablePositionControl);
            _target.EnableRotationControl = BoolField("Enable Rotation Control", _target.EnableRotationControl);
            _target.IsCanControlOnUGUI = BoolField("Is Can Control On UGUI", _target.IsCanControlOnUGUI);
            _target.EnableHighlightingEffect = BoolField("Enable Highlighting Effect", _target.EnableHighlightingEffect);
            _target.EnableMouseRay = BoolField("Enable MouseRay", _target.EnableMouseRay);
            _target.Mode = (ControlMode)EnumField("Control Mode", _target.Mode);
        }
    }
}