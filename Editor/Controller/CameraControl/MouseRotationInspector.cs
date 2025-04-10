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

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotate Key", GUILayout.Width(LabelWidth));
            if (GUILayout.Button(Target.RotateKey, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Mouse Left"), Target.RotateKey == "MouseLeft", () =>
                {
                    Undo.RecordObject(target, "Change Rotate Key");
                    Target.RotateKey = "MouseLeft";
                    HasChanged();
                });
                gm.AddItem(new GUIContent("Mouse Right"), Target.RotateKey == "MouseRight", () =>
                {
                    Undo.RecordObject(target, "Change Rotate Key");
                    Target.RotateKey = "MouseRight";
                    HasChanged();
                });
                gm.AddItem(new GUIContent("Mouse Middle"), Target.RotateKey == "MouseMiddle", () =>
                {
                    Undo.RecordObject(target, "Change Rotate Key");
                    Target.RotateKey = "MouseMiddle";
                    HasChanged();
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MouseRotation.XSpeed), "X");
            PropertyField(nameof(MouseRotation.YSpeed), "Y");
            PropertyField(nameof(MouseRotation.MSpeed), "M");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angle Limit", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MouseRotation.YMinAngleLimit), "Y min");
            PropertyField(nameof(MouseRotation.YMaxAngleLimit), "Y max");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Distance", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MouseRotation.Distance), "Distance");
            PropertyField(nameof(MouseRotation.MinDistance), "Min");
            PropertyField(nameof(MouseRotation.MaxDistance), "Max");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damping", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MouseRotation.NeedDamping), "Need Damping");
            PropertyField(nameof(MouseRotation.DampingTime), "Damping Time");
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angle", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MouseRotation.X), "X");
            PropertyField(nameof(MouseRotation.Y), "Y");
            GUILayout.EndVertical();

            GUI.enabled = true;
        }
    }
}