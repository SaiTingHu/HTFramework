using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Text))]
    internal sealed class DebuggerText : DebuggerComponentBase
    {
        private Text _target;

        public override void OnEnable()
        {
            _target = Target as Text;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.raycastTarget = GUILayout.Toggle(_target.raycastTarget, "Raycast Target");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Text: ");
            _target.text = GUILayout.TextArea(_target.text);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font Style: ");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.fontStyle = (FontStyle)EnumField(_target.fontStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font Size: ");
            _target.fontSize = IntField(_target.fontSize);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Line Spacing: ");
            _target.lineSpacing = FloatField(_target.lineSpacing);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.supportRichText = GUILayout.Toggle(_target.supportRichText, "Rich Text");
            GUILayout.EndHorizontal();
        }
    }
}