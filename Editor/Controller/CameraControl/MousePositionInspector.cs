using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    internal sealed class MousePositionInspector : HTFEditor<MousePosition>
    {
        protected override bool IsEnableRuntimeData => false;

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(MousePosition.CanControl), "Can Control");

            GUI.enabled = Target.CanControl;

            PropertyField(nameof(MousePosition.IsCanOnUGUI), "Can Control On UGUI");
            PropertyField(nameof(MousePosition.IsCanByKey), "Can Control By Key");

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Translate Key", GUILayout.Width(LabelWidth));
            if (GUILayout.Button(Target.TranslateKey, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("Mouse Left"), Target.TranslateKey == "MouseLeft", () =>
                {
                    Undo.RecordObject(target, "Change Translate Key");
                    Target.TranslateKey = "MouseLeft";
                    HasChanged();
                });
                gm.AddItem(new GUIContent("Mouse Right"), Target.TranslateKey == "MouseRight", () =>
                {
                    Undo.RecordObject(target, "Change Translate Key");
                    Target.TranslateKey = "MouseRight";
                    HasChanged();
                });
                gm.AddItem(new GUIContent("Mouse Middle"), Target.TranslateKey == "MouseMiddle", () =>
                {
                    Undo.RecordObject(target, "Change Translate Key");
                    Target.TranslateKey = "MouseMiddle";
                    HasChanged();
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            PropertyField(nameof(MousePosition.XSpeed), "X");
            PropertyField(nameof(MousePosition.YSpeed), "Y");
            PropertyField(nameof(MousePosition.ZSpeed), "Z");
            GUILayout.EndVertical();

            GUI.enabled = true;
        }
    }
}