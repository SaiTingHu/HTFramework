using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRotation))]
    public sealed class MouseRotationInspector : Editor
    {
        private MouseRotation _mouseRotation;

        private void OnEnable()
        {
            _mouseRotation = target as MouseRotation;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            this.Toggle(_mouseRotation.IsCanOnUGUI, out _mouseRotation.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            this.FloatField(_mouseRotation.OffsetX, out _mouseRotation.OffsetX);
            GUILayout.Label("Y");
            this.FloatField(_mouseRotation.OffsetY, out _mouseRotation.OffsetY);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            this.FloatField(_mouseRotation.XSpeed, out _mouseRotation.XSpeed);
            GUILayout.Label("Y");
            this.FloatField(_mouseRotation.YSpeed, out _mouseRotation.YSpeed);
            GUILayout.Label("M");
            this.FloatField(_mouseRotation.MSpeed, out _mouseRotation.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            this.FloatField(_mouseRotation.XMinLimit, out _mouseRotation.XMinLimit);
            GUILayout.Label("Y");
            this.FloatField(_mouseRotation.XMaxLimit, out _mouseRotation.XMaxLimit);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("D");
            this.FloatField(_mouseRotation.Distance, out _mouseRotation.Distance);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Distance");
                _mouseRotation.Distance += 0.1f;
                this.HasChanged();
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Distance");
                _mouseRotation.Distance -= 0.1f;
                this.HasChanged();
            }
            GUILayout.Label("Min");
            this.FloatField(_mouseRotation.MinDistance, out _mouseRotation.MinDistance);
            GUILayout.Label("Max");
            this.FloatField(_mouseRotation.MaxDistance, out _mouseRotation.MaxDistance);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            this.Toggle(_mouseRotation.NeedDamping, out _mouseRotation.NeedDamping, "Need Damping");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            this.Toggle(_mouseRotation.NeedLimit, out _mouseRotation.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mouseRotation.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                this.FloatField(_mouseRotation.XMinLimit, out _mouseRotation.XMinLimit);
                GUILayout.Label("Y Max");
                this.FloatField(_mouseRotation.XMaxLimit, out _mouseRotation.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                this.FloatField(_mouseRotation.YMinLimit, out _mouseRotation.YMinLimit);
                GUILayout.Label("Y Max");
                this.FloatField(_mouseRotation.YMaxLimit, out _mouseRotation.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                this.FloatField(_mouseRotation.ZMinLimit, out _mouseRotation.ZMinLimit);
                GUILayout.Label("Z Max");
                this.FloatField(_mouseRotation.ZMaxLimit, out _mouseRotation.ZMaxLimit);
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
            this.FloatField(_mouseRotation.X, out _mouseRotation.X);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Rotation X");
                _mouseRotation.X += 2;
                this.HasChanged();
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Rotation X");
                _mouseRotation.X -= 2;
                this.HasChanged();
            }
            GUILayout.Label("Y");
            this.FloatField(_mouseRotation.Y, out _mouseRotation.Y);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Rotation Y");
                _mouseRotation.Y += 2;
                this.HasChanged();
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                Undo.RecordObject(target, "Set Rotation Y");
                _mouseRotation.Y -= 2;
                this.HasChanged();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Copy Angle"))
            {
                GUIUtility.systemCopyBuffer =
                        _mouseRotation.X.ToString("F2") + "f," +
                        _mouseRotation.Y.ToString("F2") + "f," +
                        _mouseRotation.Distance.ToString("F2") + "f";
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }
    }
}
