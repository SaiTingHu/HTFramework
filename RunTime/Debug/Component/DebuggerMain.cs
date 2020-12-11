using UnityEngine;

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
            GUILayout.BeginHorizontal();
            _target.Pause = GUILayout.Toggle(_target.Pause, "Pause");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsEnabledLogInfo = GUILayout.Toggle(_target.IsEnabledLogInfo, "Enabled LogInfo");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsEnabledLogWarning = GUILayout.Toggle(_target.IsEnabledLogWarning, "Enabled LogWarning");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsEnabledLogError = GUILayout.Toggle(_target.IsEnabledLogError, "Enabled LogError");
            GUILayout.EndHorizontal();
        }
    }
}