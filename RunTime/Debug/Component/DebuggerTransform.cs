using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Transform))]
    internal sealed class DebuggerTransform : DebuggerComponentBase
    {
        private Transform _target;

        public override void OnEnable()
        {
            _target = Target as Transform;
        }
        public override void OnDebuggerGUI()
        {
            IntFieldReadOnly("Child Count", _target.childCount);
            _target.position = Vector3Field("Position", _target.position);
            _target.rotation = Quaternion.Euler(Vector3Field("Rotation", _target.eulerAngles));
            _target.localPosition = Vector3Field("Local Position", _target.localPosition);
            _target.localRotation = Quaternion.Euler(Vector3Field("Local Rotation", _target.localEulerAngles));
            _target.localScale = Vector3Field("Scale", _target.localScale);

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