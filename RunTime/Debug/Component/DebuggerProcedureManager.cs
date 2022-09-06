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
            string content = _target.CurrentProcedure != null ? _target.CurrentProcedure.GetType().FullName : "<None>";
            StringFieldReadOnly("Current Procedure", content);

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