namespace HT.Framework
{
    [CustomDebugger(typeof(UIManager))]
    internal sealed class DebuggerUIManager : DebuggerComponentBase
    {
        private UIManager _target;

        public override void OnEnable()
        {
            _target = Target as UIManager;
        }
        public override void OnDebuggerGUI()
        {
            _target.IsHideAll = BoolField("Is Hide All", _target.IsHideAll);
            _target.IsDisplayMask = BoolField("Is Display Mask", _target.IsDisplayMask);
            _target.IsLockTemporaryUI = BoolField("Is Lock Temporary", _target.IsLockTemporaryUI);
        }
    }
}