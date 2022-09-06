using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(AspectTrackManager))]
    internal sealed class DebuggerAspectTracker : DebuggerComponentBase
    {
        private AspectTrackManager _target;

        public override void OnEnable()
        {
            _target = Target as AspectTrackManager;
        }
        public override void OnDebuggerGUI()
        {
            _target.IsEnableAspectTrack = BoolField("Is Enable Track", _target.IsEnableAspectTrack);
            _target.IsEnableIntercept = BoolField("Is Enable Intercept", _target.IsEnableIntercept);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intercept Conditions");
            GUILayout.EndHorizontal();

            foreach (var condition in _target.InterceptConditions)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(condition.Key);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Conditions"))
            {
                _target.InterceptConditions.Clear();
            }
            GUILayout.EndHorizontal();
        }
    }
}