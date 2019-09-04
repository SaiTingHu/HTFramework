using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    public sealed class MousePositionInspector : HTFEditor<MousePosition>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsCanOnUGUI, out Target.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            FloatField(Target.XSpeed, out Target.XSpeed);
            GUILayout.Label("Y");
            FloatField(Target.YSpeed, out Target.YSpeed);
            GUILayout.Label("Z");
            FloatField(Target.ZSpeed, out Target.ZSpeed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping Time");
            FloatField(Target.DampingTime, out Target.DampingTime);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            Toggle(Target.NeedLimit, out Target.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (Target.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                FloatField(Target.XMinLimit, out Target.XMinLimit);
                GUILayout.Label("X Max");
                FloatField(Target.XMaxLimit, out Target.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                FloatField(Target.YMinLimit, out Target.YMinLimit);
                GUILayout.Label("Y Max");
                FloatField(Target.YMaxLimit, out Target.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                FloatField(Target.ZMinLimit, out Target.ZMinLimit);
                GUILayout.Label("Z Max");
                FloatField(Target.ZMaxLimit, out Target.ZMaxLimit);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}
