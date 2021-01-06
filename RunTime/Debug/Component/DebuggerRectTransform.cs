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
            GUILayout.BeginHorizontal();
            GUILayout.Label("ChildCount: " + _target.childCount);
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
            GUILayout.Label("AnchoredPosition:", GUILayout.Width(60));
            _target.anchoredPosition3D = Vector3Field(_target.anchoredPosition3D);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("SizeDelta:", GUILayout.Width(60));
            _target.sizeDelta = Vector2Field(_target.sizeDelta);
            GUILayout.EndHorizontal();
        }
    }
}