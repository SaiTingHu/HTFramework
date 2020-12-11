using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Button))]
    internal sealed class DebuggerButton : DebuggerComponentBase
    {
        private Button _target;
        private int _onClickEventCount;
        private List<string> _onClickEvents = new List<string>();

        public override void OnEnable()
        {
            _target = Target as Button;
            _onClickEventCount = _target.onClick.GetPersistentEventCount();
            _onClickEvents.Clear();
            for (int i = 0; i < _onClickEventCount; i++)
            {
                _onClickEvents.Add(string.Format("{0}: {1} / {2}()", (i + 1).ToString(), _target.onClick.GetPersistentTarget(i).name, _target.onClick.GetPersistentMethodName(i)));
            }
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.interactable = GUILayout.Toggle(_target.interactable, "Interactable");

            GUILayout.BeginHorizontal();
            GUILayout.Label("OnClick Event Count: " + _onClickEventCount);
            GUI.enabled = _onClickEventCount > 0;
            if (GUILayout.Button("OnClick"))
            {
                _target.onClick.Invoke();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            
            for (int i = 0; i < _onClickEvents.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_onClickEvents[i]);
                GUILayout.EndHorizontal();
            }
        }
    }
}