using UnityEngine;
using UnityEngine.Rendering;

namespace HT.Framework
{
    [CustomDebugger(typeof(MeshRenderer))]
    public sealed class DebuggerMeshRenderer : DebuggerComponentBase
    {
        private MeshRenderer _target;

        public override void OnEnable()
        {
            _target = Target as MeshRenderer;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.receiveShadows = GUILayout.Toggle(_target.receiveShadows, "Receive Shadows");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Cast Shadows: ");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.shadowCastingMode = (ShadowCastingMode)EnumField(_target.shadowCastingMode);
            GUILayout.EndHorizontal();
        }
    }
}