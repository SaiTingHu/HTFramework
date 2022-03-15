namespace HT.Framework
{
    [CustomDebugger(typeof(Main))]
    internal sealed class DebuggerMain : DebuggerComponentBase
    {
        private Main _target;

        public override void OnEnable()
        {
            _target = Target as Main;
        }
        public override void OnDebuggerGUI()
        {
            _target.Pause = BoolField("Pause", _target.Pause);
            _target.IsEnabledLogInfo = BoolField("Enabled LogInfo", _target.IsEnabledLogInfo);
            _target.IsEnabledLogWarning = BoolField("Enabled LogWarning", _target.IsEnabledLogWarning);
            _target.IsEnabledLogError = BoolField("Enabled LogError", _target.IsEnabledLogError);
        }
    }
}