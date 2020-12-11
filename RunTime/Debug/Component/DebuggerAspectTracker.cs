using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(AspectTracker))]
    internal sealed class DebuggerAspectTracker : DebuggerComponentBase
    {
        private AspectTracker _target;

        public override void OnEnable()
        {
            _target = Target as AspectTracker;
        }

        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            _target.IsEnableAspectTrack = GUILayout.Toggle(_target.IsEnableAspectTrack, "Is Enable Track");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsEnableIntercept = GUILayout.Toggle(_target.IsEnableIntercept, "Is Enable Intercept");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intercept Conditions: ");
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