using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    [CustomDebugger(typeof(Toggle))]
    public sealed class DebuggerToggle : DebuggerComponentBase
    {
        private Toggle _target;
        private int _onValueChangedEventCount;
        private List<string> _onValueChangedEvents = new List<string>();

        public override void OnEnable()
        {
            _target = Target as Toggle;
            _onValueChangedEventCount = _target.onValueChanged.GetPersistentEventCount();
            _onValueChangedEvents.Clear();
            for (int i = 0; i < _onValueChangedEventCount; i++)
            {
                _onValueChangedEvents.Add(string.Format("{0}: {1} / {2}()", (i + 1).ToString(), _target.onValueChanged.GetPersistentTarget(i).name, _target.onValueChanged.GetPersistentMethodName(i)));
            }
        }

        public override void OnDebuggerGUI()
        {
            GUI.contentColor = _target.enabled ? Color.white : Color.gray;
            _target.enabled = GUILayout.Toggle(_target.enabled, "Enabled");
            _target.interactable = GUILayout.Toggle(_target.interactable, "Interactable");
            
            GUILayout.BeginHorizontal();
            _target.isOn = GUILayout.Toggle(_target.isOn, "Is On");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("OnValueChanged Event Count: " + _onValueChangedEventCount);
            GUI.enabled = _onValueChangedEventCount > 0;
            if (GUILayout.Button("OnValueChanged"))
            {
                _target.onValueChanged.Invoke(_target.isOn);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            for (int i = 0; i < _onValueChangedEvents.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_onValueChangedEvents[i]);
                GUILayout.EndHorizontal();
            }
        }
    }
}