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
            _target.text = StringField("Text", _target.text);
            _target.offsetZ = FloatField("Offset Z", _target.offsetZ);
            _target.characterSize = FloatField("Character Size", _target.characterSize);
            _target.lineSpacing = FloatField("Line Spacing", _target.lineSpacing);
            _target.tabSize = FloatField("Tab Size", _target.tabSize);
            _target.fontSize = IntField("Font Size", _target.fontSize);
            _target.fontStyle = (FontStyle)EnumField("Font Style", _target.fontStyle);
            _target.richText = BoolField("Rich Text", _target.richText);
            ObjectFieldReadOnly("Font", _target.font);
            _target.color = ColorField("Color", _target.color);
        }
    }
}