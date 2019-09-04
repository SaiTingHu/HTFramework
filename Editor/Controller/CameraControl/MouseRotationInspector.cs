using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRotation))]
    public sealed class MouseRotationInspector : HTFEditor<MouseRotation>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsCanOnUGUI, out Target.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.AllowOverstepDistance, out Target.AllowOverstepDistance, "Allow Overstep Distance");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(Target.OffsetX, out Target.OffsetX);
            GUILayout.Label("Y");
            FloatField(Target.OffsetY, out Target.OffsetY);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(Target.XSpeed, out Target.XSpeed);
            GUILayout.Label("Y");
            FloatField(Target.YSpeed, out Target.YSpeed);
            GUILayout.Label("M");
            FloatField(Target.MSpeed, out Target.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("Y min");
            FloatField(Target.YMinAngleLimit, out Target.YMinAngleLimit);
            GUILayout.Label("Y max");
            FloatField(Target.YMaxAngleLimit, out Target.YMaxAngleLimit);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("D");
            FloatField(Target.Distance, out Target.Distance);
            Button(DistanceAdd, "", "OL Plus", GUILayout.Width(15));
            Button(DistanceSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.Label("Min");
            FloatField(Target.MinDistance, out Target.MinDistance);
            GUILayout.Label("Max");
            FloatField(Target.MaxDistance, out Target.MaxDistance);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            Toggle(Target.NeedDamping, out Target.NeedDamping, "Need Damping");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Position Limit:", "BoldLabel");
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


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            FloatField(Target.X, out Target.X);
            Button(XAdd, "", "OL Plus", GUILayout.Width(15));
            Button(XSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.Label("Y");
            FloatField(Target.Y, out Target.Y);
            Button(YAdd, "", "OL Plus", GUILayout.Width(15));
            Button(YSubtract, "", "OL Minus", GUILayout.Width(15));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DistanceAdd()
        {
            Target.Distance += 0.1f;
        }
        private void DistanceSubtract()
        {
            Target.Distance -= 0.1f;
        }
        private void XAdd()
        {
            Target.X += 2;
        }
        private void XSubtract()
        {
            Target.X -= 2;
        }
        private void YAdd()
        {
            Target.Y += 2;
        }
        private void YSubtract()
        {
            Target.Y -= 2;
        }
    }
}
