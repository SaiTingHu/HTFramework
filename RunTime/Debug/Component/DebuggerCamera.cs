using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(Camera))]
    internal sealed class DebuggerCamera : DebuggerComponentBase
    {
        private Camera _target;

        public override void OnEnable()
        {
            _target = Target as Camera;
        }
        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;

            _target.enabled = BoolField("Enabled", _target.enabled);
            _target.clearFlags = (CameraClearFlags)EnumField("Clear Flags", _target.clearFlags);
            _target.fieldOfView = FloatField("Field Of View", _target.fieldOfView);
            _target.nearClipPlane = FloatField("Clipping Planes Near", _target.nearClipPlane);
            _target.farClipPlane = FloatField("Clipping Planes Far", _target.farClipPlane);
            _target.depth = FloatField("Depth", _target.depth);
            ObjectFieldReadOnly("Target Texture", _target.targetTexture);
            _target.useOcclusionCulling = BoolField("Occlusion Culling", _target.useOcclusionCulling);
        }
    }
}