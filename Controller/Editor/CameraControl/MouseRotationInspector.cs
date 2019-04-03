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
            _mouseRotation.IsCanOnUGUI = GUILayout.Toggle(_mouseRotation.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Offset:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            _mouseRotation.OffsetX = EditorGUILayout.FloatField(_mouseRotation.OffsetX);
            GUILayout.Label("Y");
            _mouseRotation.OffsetY = EditorGUILayout.FloatField(_mouseRotation.OffsetY);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            _mouseRotation.XSpeed = EditorGUILayout.FloatField(_mouseRotation.XSpeed);
            GUILayout.Label("Y");
            _mouseRotation.YSpeed = EditorGUILayout.FloatField(_mouseRotation.YSpeed);
            GUILayout.Label("M");
            _mouseRotation.MSpeed = EditorGUILayout.FloatField(_mouseRotation.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("X");
            _mouseRotation.XMinLimit = EditorGUILayout.FloatField(_mouseRotation.XMinLimit);
            GUILayout.Label("Y");
            _mouseRotation.XMaxLimit = EditorGUILayout.FloatField(_mouseRotation.XMaxLimit);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            GUILayout.Label("D");
            _mouseRotation.Distance = EditorGUILayout.FloatField(_mouseRotation.Distance);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                _mouseRotation.Distance += 0.1f;
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                _mouseRotation.Distance -= 0.1f;
            }
            GUILayout.Label("Min");
            _mouseRotation.MinDistance = EditorGUILayout.FloatField(_mouseRotation.MinDistance);
            GUILayout.Label("Max");
            _mouseRotation.MaxDistance = EditorGUILayout.FloatField(_mouseRotation.MaxDistance);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Damping:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Box");
            _mouseRotation.NeedDamping = GUILayout.Toggle(_mouseRotation.NeedDamping, "Need Damping");
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            _mouseRotation.NeedLimit = GUILayout.Toggle(_mouseRotation.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mouseRotation.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                _mouseRotation.XMinLimit = EditorGUILayout.FloatField(_mouseRotation.XMinLimit);
                GUILayout.Label("Y Max");
                _mouseRotation.XMaxLimit = EditorGUILayout.FloatField(_mouseRotation.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                _mouseRotation.YMinLimit = EditorGUILayout.FloatField(_mouseRotation.YMinLimit);
                GUILayout.Label("Y Max");
                _mouseRotation.YMaxLimit = EditorGUILayout.FloatField(_mouseRotation.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                _mouseRotation.ZMinLimit = EditorGUILayout.FloatField(_mouseRotation.ZMinLimit);
                GUILayout.Label("Z Max");
                _mouseRotation.ZMaxLimit = EditorGUILayout.FloatField(_mouseRotation.ZMaxLimit);
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
            _mouseRotation.X = EditorGUILayout.FloatField(_mouseRotation.X);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                _mouseRotation.X += 2;
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                _mouseRotation.X -= 2;
            }
            GUILayout.Label("Y");
            _mouseRotation.Y = EditorGUILayout.FloatField(_mouseRotation.Y);
            if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
            {
                _mouseRotation.Y += 2;
            }
            if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
            {
                _mouseRotation.Y -= 2;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Copy Angle", "MiniButton"))
            {
                GUIUtility.systemCopyBuffer =
                        _mouseRotation.X.ToString("F2") + "f," +
                        _mouseRotation.Y.ToString("F2") + "f," +
                        _mouseRotation.Distance.ToString("F2") + "f";
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_mouseRotation);
            }
        }
    }
}
