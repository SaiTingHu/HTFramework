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
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.raycastTarget = GUILayout.Toggle(_target.raycastTarget, "Raycast Target");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fill Amount: ");
            _target.fillAmount = FloatField(_target.fillAmount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.fillCenter = GUILayout.Toggle(_target.fillCenter, "Fill Center");
            GUILayout.EndHorizontal();
        }
    }
}