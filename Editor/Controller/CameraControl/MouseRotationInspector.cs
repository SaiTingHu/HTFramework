using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRotation))]
    public sealed class MouseRotationInspector : HTFEditor<MouseRotation>
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
            Toggle(Target.AllowOverstepDistance, out Target.AllowOverstepDistance, "Allow Overstep Distance");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsLookAtTarget, out Target.IsLookAtTarget, "LookAt Target");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.XSpeed, out Target.XSpeed, "X");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.YSpeed, out Target.YSpeed, "Y");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.MSpeed, out Target.MSpeed, "M");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.YMinAngleLimit, out Target.YMinAngleLimit, "Y min");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.YMaxAngleLimit, out Target.YMaxAngleLimit, "Y max");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.Distance, out Target.Distance, "Distance");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.MinDistance, out Target.MinDistance, "Min");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.MaxDistance, out Target.MaxDistance, "Max");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.NeedDamping, out Target.NeedDamping, "Need Damping");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Position Limit", EditorStyles.boldLabel);
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
            
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.X, out Target.X, "X");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.Y, out Target.Y, "Y");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}