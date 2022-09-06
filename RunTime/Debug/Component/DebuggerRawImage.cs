using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(RawImage))]
    internal sealed class DebuggerRawImage : DebuggerComponentBase
    {
        private RawImage _target;

        public override void OnEnable()
        {
            _target = Target as RawImage;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            ObjectFieldReadOnly("Texture", _target.texture);
            ObjectFieldReadOnly("Material", _target.material);
            _target.raycastTarget = BoolField("Raycast Target", _target.raycastTarget);
            _target.maskable = BoolField("Maskable", _target.maskable);
        }
    }
}