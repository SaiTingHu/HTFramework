using UnityEngine;

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
            GUILayout.BeginHorizontal();
            _target.IsHideAll = GUILayout.Toggle(_target.IsHideAll, "Is Hide All");
            GUILayout.EndHorizontal();
        }
    }
}