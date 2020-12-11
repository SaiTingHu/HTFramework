using UnityEngine;

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
            GUILayout.BeginHorizontal();
            _target.IsOfflineState = GUILayout.Toggle(_target.IsOfflineState, "Is Offline State");
            GUILayout.EndHorizontal();
        }
    }
}