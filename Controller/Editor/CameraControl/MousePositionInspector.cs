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
            this.Toggle(_mousePosition.IsCanOnUGUI, out _mousePosition.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            this.FloatField(_mousePosition.XSpeed, out _mousePosition.XSpeed);
            GUILayout.Label("Y");
            this.FloatField(_mousePosition.YSpeed, out _mousePosition.YSpeed);
            GUILayout.Label("M");
            this.FloatField(_mousePosition.MSpeed, out _mousePosition.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Move Damping");
            this.FloatField(_mousePosition.MoveDamping, out _mousePosition.MoveDamping);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            this.Toggle(_mousePosition.NeedLimit, out _mousePosition.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mousePosition.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                this.FloatField(_mousePosition.XMinLimit, out _mousePosition.XMinLimit);
                GUILayout.Label("X Max");
                this.FloatField(_mousePosition.XMaxLimit, out _mousePosition.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                this.FloatField(_mousePosition.YMinLimit, out _mousePosition.YMinLimit);
                GUILayout.Label("Y Max");
                this.FloatField(_mousePosition.YMaxLimit, out _mousePosition.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                this.FloatField(_mousePosition.ZMinLimit, out _mousePosition.ZMinLimit);
                GUILayout.Label("Z Max");
                this.FloatField(_mousePosition.ZMaxLimit, out _mousePosition.ZMaxLimit);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Copy Position"))
            {
                if (_mousePosition.Target)
                {
                    GUIUtility.systemCopyBuffer =
                            _mousePosition.Target.transform.localPosition.x.ToString("F2") + "f," +
                            _mousePosition.Target.transform.localPosition.y.ToString("F2") + "f," +
                            _mousePosition.Target.transform.localPosition.z.ToString("F2") + "f";
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }
    }
}
