using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(BoxCollider))]
    internal sealed class DebuggerBoxCollider : DebuggerComponentBase
    {
        private BoxCollider _target;
        
        public override void OnEnable()
        {
            _target = Target as BoxCollider;
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.isTrigger = GUILayout.Toggle(_target.isTrigger, "Is Trigger");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Center: ", GUILayout.Width(60));
            _target.center = Vector3Field(_target.center);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Size: ", GUILayout.Width(60));
            _target.size = Vector3Field(_target.size);
            GUILayout.EndHorizontal();
        }
    }
}