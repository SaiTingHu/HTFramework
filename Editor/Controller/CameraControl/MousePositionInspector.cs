using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    public sealed class MousePositionInspector : HTFEditor<MousePosition>
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

            GUILayout.BeginVertical("Box");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", "BoldLabel");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            FloatField(Target.XSpeed, out Target.XSpeed, "X");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.YSpeed, out Target.YSpeed, "Y");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.ZSpeed, out Target.ZSpeed, "Z");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.DampingTime, out Target.DampingTime, "Damping Time");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Position Limit", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.NeedLimit, out Target.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();
            
            if (Target.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                FloatField(Target.XMinLimit, out Target.XMinLimit, "X Min");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                FloatField(Target.YMinLimit, out Target.YMinLimit, "Y Min");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                FloatField(Target.ZMinLimit, out Target.ZMinLimit, "Z Min");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                FloatField(Target.XMaxLimit, out Target.XMaxLimit, "X Max");
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                FloatField(Target.YMaxLimit, out Target.YMaxLimit, "Y Max");
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                FloatField(Target.ZMaxLimit, out Target.ZMaxLimit, "Z Max");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}