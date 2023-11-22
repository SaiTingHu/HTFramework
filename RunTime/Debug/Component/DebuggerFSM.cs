using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(FSM))]
    internal sealed class DebuggerFSM : DebuggerComponentBase
    {
        private FSM _target;

        public override void OnEnable()
        {
            _target = Target as FSM;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            BoolFieldReadOnly("Auto Register", _target.IsAutoRegister);
            StringFieldReadOnly("Name", _target.Name);
            StringFieldReadOnly("Group", _target.Group);
            StringFieldReadOnly("Data", _target.Data);
            ObjectFieldReadOnly("Args", _target.Args);
            StringFieldReadOnly("Current State", _target.CurrentState != null ? _target.CurrentState.GetType().ToString() : "<None>");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Renewal"))
            {
                _target.Renewal();
            }
            if (GUILayout.Button("Final"))
            {
                _target.Final();
            }
            GUILayout.EndHorizontal();
        }
    }
}