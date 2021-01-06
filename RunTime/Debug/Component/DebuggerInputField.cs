using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(InputField))]
    internal sealed class DebuggerInputField : DebuggerComponentBase
    {
        private InputField _target;

        public override void OnEnable()
        {
            _target = Target as InputField;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.interactable = GUILayout.Toggle(_target.interactable, "Interactable");
            _target.readOnly = GUILayout.Toggle(_target.readOnly, "Read Only");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Text: ");
            _target.text = GUILayout.TextArea(_target.text);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character Limit: ");
            _target.characterLimit = IntField(_target.characterLimit);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Content Type: ");
            GUILayout.EndHorizontal();

            _target.contentType = (InputField.ContentType)EnumField(_target.contentType);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Line Type: ");
            GUILayout.EndHorizontal();

            _target.lineType = (InputField.LineType)EnumField(_target.lineType);
        }
    }
}