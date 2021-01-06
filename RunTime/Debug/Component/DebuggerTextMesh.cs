using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(TextMesh))]
    internal sealed class DebuggerTextMesh : DebuggerComponentBase
    {
        private TextMesh _target;

        public override void OnEnable()
        {
            _target = Target as TextMesh;
        }
        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Text: ");
            _target.text = GUILayout.TextArea(_target.text);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset Z: ");
            _target.offsetZ = FloatField(_target.offsetZ);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character Size: ");
            _target.characterSize = FloatField(_target.characterSize);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Line Spacing: ");
            _target.lineSpacing = FloatField(_target.lineSpacing);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tab Size: ");
            _target.tabSize = FloatField(_target.tabSize);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font Size: ");
            _target.fontSize = IntField(_target.fontSize);
            GUILayout.EndHorizontal();
        }
    }
}