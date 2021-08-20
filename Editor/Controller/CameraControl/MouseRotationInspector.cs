using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRotation))]
    internal sealed class MouseRotationInspector : HTFEditor<MouseRotation>
    {
        protected override bool IsEnableRuntimeData => false;

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(MouseRotation.CanControl), "Can Control");

            GUI.enabled = Target.CanControl;

            PropertyField(nameof(MouseRotation.IsCanOnUGUI), "Can Control On UGUI");
            PropertyField(nameof(MouseRotation.AllowOverstepDistance), "Allow Overstep Distance");
            PropertyField(nameof(MouseRotation.IsLookAtTarget), "LookAt Target");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(MouseRotation.XSpeed), "      X");
            PropertyField(nameof(MouseRotation.YSpeed), "      Y");
            PropertyField(nameof(MouseRotation.MSpeed), "      M");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(MouseRotation.YMinAngleLimit), "      Y min");
            PropertyField(nameof(MouseRotation.YMaxAngleLimit), "      Y max");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(MouseRotation.Distance), "      Distance");
            PropertyField(nameof(MouseRotation.MinDistance), "      Min");
            PropertyField(nameof(MouseRotation.MaxDistance), "      Max");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(MouseRotation.NeedDamping), "      Need Damping");
            PropertyField(nameof(MouseRotation.DampingTime), "      Damping Time");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            PropertyField(nameof(MouseRotation.X), "      X");
            PropertyField(nameof(MouseRotation.Y), "      Y");
            
            GUI.enabled = true;
        }
    }
}