namespace HT.Framework
{
    [CustomDebugger(typeof(EntityManager))]
    internal sealed class DebuggerEntityManager : DebuggerComponentBase
    {
        private EntityManager _target;

        public override void OnEnable()
        {
            _target = Target as EntityManager;
        }
        public override void OnDebuggerGUI()
        {
            _target.IsHideAll = BoolField("Is Hide All", _target.IsHideAll);
        }
    }
}