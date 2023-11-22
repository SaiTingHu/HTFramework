using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(RectTransform))]
    internal sealed class DebuggerRectTransform : DebuggerComponentBase
    {
        private RectTransform _target;

        public override void OnEnable()
        {
            _target = Target as RectTransform;
        }
        public override void OnDebuggerGUI()
        {
            IntFieldReadOnly("Child Count", _target.childCount);
            _target.position = Vector3Field("Position", _target.position);
            _target.rotation = Quaternion.Euler(Vector3Field("Rotation", _target.eulerAngles));
            _target.localPosition = Vector3Field("Local Position", _target.localPosition);
            _target.localRotation = Quaternion.Euler(Vector3Field("Local Rotation", _target.localEulerAngles));
            _target.localScale = Vector3Field("Scale", _target.localScale);
            _target.anchoredPosition = Vector2Field("AnchoredPosition", _target.anchoredPosition);
            _target.sizeDelta = Vector2Field("SizeDelta", _target.sizeDelta);
            _target.pivot = Vector2Field("Pivot", _target.pivot);

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton("x-"))
            {
                _target.anchoredPosition -= new Vector2(1, 0);
            }
            if (GUILayout.RepeatButton("x+"))
            {
                _target.anchoredPosition += new Vector2(1, 0);
            }
            if (GUILayout.RepeatButton("y-"))
            {
                _target.anchoredPosition -= new Vector2(0, 1);
            }
            if (GUILayout.RepeatButton("y+"))
            {
                _target.anchoredPosition += new Vector2(0, 1);
            }
            GUILayout.EndHorizontal();
        }
    }
}