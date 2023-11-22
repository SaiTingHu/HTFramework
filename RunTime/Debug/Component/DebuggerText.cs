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

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.text = StringField("Text", _target.text);
            ObjectFieldReadOnly("Font", _target.font);
            _target.fontStyle = (FontStyle)EnumField("Font Style", _target.fontStyle);
            _target.fontSize = IntField("Font Size", _target.fontSize);
            _target.lineSpacing = FloatField("Line Spacing", _target.lineSpacing);
            _target.supportRichText = BoolField("Rich Text", _target.supportRichText);
            _target.color = ColorField("Color", _target.color);
            ObjectFieldReadOnly("Material", _target.material);
            _target.raycastTarget = BoolField("Raycast Target", _target.raycastTarget);
            _target.maskable = BoolField("Maskable", _target.maskable);
        }
    }
}