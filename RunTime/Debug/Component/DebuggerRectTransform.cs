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
            IntField("Child Count", _target.childCount);
            _target.position = Vector3Field("Position", _target.position);
            _target.rotation = Quaternion.Euler(Vector3Field("Rotation", _target.eulerAngles));
            _target.localPosition = Vector3Field("Local Position", _target.localPosition);
            _target.localRotation = Quaternion.Euler(Vector3Field("Local Rotation", _target.localEulerAngles));
            _target.localScale = Vector3Field("Scale", _target.localScale);
            _target.anchoredPosition3D = Vector3Field("AnchoredPosition", _target.anchoredPosition3D);
            _target.sizeDelta = Vector2Field("SizeDelta", _target.sizeDelta);
            _target.pivot = Vector2Field("Pivot", _target.pivot);
        }
    }
}