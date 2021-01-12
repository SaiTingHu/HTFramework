using UnityEngine;

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
            GUILayout.BeginHorizontal();
            _target.IsHideAll = GUILayout.Toggle(_target.IsHideAll, "Is Hide All");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsDisplayMask = GUILayout.Toggle(_target.IsDisplayMask, "Is Display Mask");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsLockTemporaryUI = GUILayout.Toggle(_target.IsLockTemporaryUI, "Is Lock Temporary");
            GUILayout.EndHorizontal();
        }
    }
}