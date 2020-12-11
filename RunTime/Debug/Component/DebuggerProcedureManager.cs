using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(ProcedureManager))]
    internal sealed class DebuggerProcedureManager : DebuggerComponentBase
    {
        private ProcedureManager _target;

        public override void OnEnable()
        {
            _target = Target as ProcedureManager;
        }

        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current Procedure: " + _target.CurrentProcedure);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Last Procedure"))
            {
                _target.SwitchLastProcedure();
            }
            if (GUILayout.Button("Next Procedure"))
            {
                _target.SwitchNextProcedure();
            }
            GUILayout.EndHorizontal();
        }
    }
}