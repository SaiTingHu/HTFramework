using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    internal sealed class MousePositionInspector : HTFEditor<MousePosition>
    {
        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsCanOnUGUI, out Target.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            FloatField(Target.XSpeed, out Target.XSpeed, "      X");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.YSpeed, out Target.YSpeed, "      Y");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.ZSpeed, out Target.ZSpeed, "      Z");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.DampingTime, out Target.DampingTime, "      Damping Time");
            GUILayout.EndHorizontal();
        }
    }
}