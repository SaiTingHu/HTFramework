using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    public sealed class MousePositionInspector : Editor
    {
        private MousePosition _mousePosition;

        private void OnEnable()
        {
            _mousePosition = target as MousePosition;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            _mousePosition.IsCanOnUGUI = GUILayout.Toggle(_mousePosition.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            _mousePosition.XSpeed = EditorGUILayout.FloatField(_mousePosition.XSpeed);
            GUILayout.Label("Y");
            _mousePosition.YSpeed = EditorGUILayout.FloatField(_mousePosition.YSpeed);
            GUILayout.Label("M");
            _mousePosition.MSpeed = EditorGUILayout.FloatField(_mousePosition.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Move Damping");
            _mousePosition.MoveDamping = EditorGUILayout.FloatField(_mousePosition.MoveDamping);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            _mousePosition.NeedLimit = GUILayout.Toggle(_mousePosition.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mousePosition.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                _mousePosition.XMinLimit = EditorGUILayout.FloatField(_mousePosition.XMinLimit);
                GUILayout.Label("Y Max");
                _mousePosition.XMaxLimit = EditorGUILayout.FloatField(_mousePosition.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                _mousePosition.YMinLimit = EditorGUILayout.FloatField(_mousePosition.YMinLimit);
                GUILayout.Label("Y Max");
                _mousePosition.YMaxLimit = EditorGUILayout.FloatField(_mousePosition.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                _mousePosition.ZMinLimit = EditorGUILayout.FloatField(_mousePosition.ZMinLimit);
                GUILayout.Label("Z Max");
                _mousePosition.ZMaxLimit = EditorGUILayout.FloatField(_mousePosition.ZMaxLimit);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy Position", "MiniButton"))
            {
                if (_mousePosition.Target)
                {
                    GUIUtility.systemCopyBuffer =
                            _mousePosition.Target.transform.localPosition.x.ToString("F2") + "f," +
                            _mousePosition.Target.transform.localPosition.y.ToString("F2") + "f," +
                            _mousePosition.Target.transform.localPosition.z.ToString("F2") + "f";
                }
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_mousePosition);
            }
        }
    }
}
