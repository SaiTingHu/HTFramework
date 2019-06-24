using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepMaster))]
    public sealed class StepMasterInspector : ModuleEditor
    {
        private StepMaster _target;

        private Dictionary<string, StepContent> _stepContentIDs;
        private Dictionary<string, bool> _stepContentEnables;
        private Dictionary<string, string> _customOrder;

        protected override void OnEnable()
        {
            _target = target as StepMaster;
            
            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _stepContentIDs = _target.GetType().GetField("_stepContentIDs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<string, StepContent>;
            _stepContentEnables = _target.GetType().GetField("_stepContentEnables", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<string, bool>;
            _customOrder = _target.GetType().GetField("_customOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<string, string>;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Step Master, the stepflow controller!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ObjectField(_target.ContentAsset, out _target.ContentAsset, false, "Asset");
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();

            if (_target.ContentAsset != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Step Count: " + _target.StepCount);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step: " + _target.CurrentStepIndex);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Skip", "Minibutton"))
                {
                    _target.SkipCurrentStep();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Current Step Name: " + (_target.CurrentStepContent != null ? _target.CurrentStepContent.Name : "<None>"));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Disabled Step: ");
                GUILayout.EndHorizontal();

                foreach (KeyValuePair<string, bool> step in _stepContentEnables)
                {
                    if (!step.Value)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(_stepContentIDs[step.Key].Name);
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Custom Order: " + _customOrder.Count);
                GUILayout.EndHorizontal();

                foreach (KeyValuePair<string, string> order in _customOrder)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(string.Format("{0} -> {1}", _stepContentIDs[order.Key].Name, _stepContentIDs[order.Value].Name));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("StepMaster Asset is null!");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
