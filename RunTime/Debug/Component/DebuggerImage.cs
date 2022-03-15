using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Image))]
    internal sealed class DebuggerImage : DebuggerComponentBase
    {
        private Image _target;

        public override void OnEnable()
        {
            _target = Target as Image;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Source Image", _target.sprite);
            ObjectFieldReadOnly("Material", _target.material);
            _target.raycastTarget = BoolField("Raycast Target", _target.raycastTarget);
            _target.maskable = BoolField("Maskable", _target.maskable);
            _target.type = (Image.Type)EnumField("Image Type", _target.type);
            _target.fillCenter = BoolField("Fill Center", _target.fillCenter);
            _target.preserveAspect = BoolField("Preserve Aspect", _target.preserveAspect);
            _target.fillAmount = FloatField("Fill Amount", _target.fillAmount);
        }
    }
}