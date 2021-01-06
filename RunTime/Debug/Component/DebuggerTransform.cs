using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Transform))]
    internal sealed class DebuggerTransform : DebuggerComponentBase
    {
        private Transform _target;
        private int _childCount;

        public override void OnEnable()
        {
            _target = Target as Transform;
            _childCount = _target.childCount;
        }
        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ChildCount: " + _childCount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Position:", GUILayout.Width(60));
            _target.position = Vector3Field(_target.position);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotation:", GUILayout.Width(60));
            _target.rotation = Quaternion.Euler(Vector3Field(_target.rotation.eulerAngles));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale:", GUILayout.Width(60));
            _target.localScale = Vector3Field(_target.localScale);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton("x-"))
            {
                _target.Translate(-0.1f, 0, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("x+"))
            {
                _target.Translate(0.1f, 0, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("y-"))
            {
                _target.Translate(0, -0.1f, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("y+"))
            {
                _target.Translate(0, 0.1f, 0, Space.Self);
            }
            if (GUILayout.RepeatButton("z-"))
            {
                _target.Translate(0, 0, -0.1f, Space.Self);
            }
            if (GUILayout.RepeatButton("z+"))
            {
                _target.Translate(0, 0, 0.1f, Space.Self);
            }
            GUILayout.EndHorizontal();
        }
    }
}