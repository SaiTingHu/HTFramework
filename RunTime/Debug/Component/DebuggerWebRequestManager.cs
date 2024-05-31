namespace HT.Framework
{
    [CustomDebugger(typeof(WebRequestManager))]
    internal sealed class DebuggerWebRequestManager : DebuggerComponentBase
    {
        private WebRequestManager _target;

        public override void OnEnable()
        {
            _target = Target as WebRequestManager;
        }
        public override void OnDebuggerGUI()
        {
            _target.IsOfflineState = BoolField("Is Offline State", _target.IsOfflineState);
            _target.IsLogDetail = BoolField("Is Log Detail", _target.IsLogDetail);
        }
    }
}