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

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.interactable = BoolField("Interactable", _target.interactable);
            _target.readOnly = BoolField("Read Only", _target.readOnly);
            _target.text = StringField("Text", _target.text);
            _target.characterLimit = IntField("Character Limit", _target.characterLimit);
            _target.contentType = (InputField.ContentType)EnumField("Content Type", _target.contentType);
            _target.lineType = (InputField.LineType)EnumField("Line Type", _target.lineType);
        }
    }
}