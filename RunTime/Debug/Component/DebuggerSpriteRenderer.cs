using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(SpriteRenderer))]
    internal sealed class DebuggerSpriteRenderer : DebuggerComponentBase
    {
        private SpriteRenderer _target;

        public override void OnEnable()
        {
            _target = Target as SpriteRenderer;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            
            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Sprite", _target.sprite);
            _target.color = ColorField("Color", _target.color);
            _target.flipX = BoolField("Flip X", _target.flipX);
            _target.flipY = BoolField("Flip Y", _target.flipY);
            _target.drawMode = (SpriteDrawMode)EnumField("Draw Mode", _target.drawMode);
            _target.spriteSortPoint = (SpriteSortPoint)EnumField("Sprite Sort Point", _target.spriteSortPoint);
            ObjectFieldReadOnly("Material", _target.material);
            StringFieldReadOnly("Sorting Layer", _target.sortingLayerName);
            _target.sortingOrder = IntField("Order in Layer", _target.sortingOrder);
        }
    }
}