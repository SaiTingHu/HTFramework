using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(MousePosition))]
    public sealed class MousePositionInspector : ModuleEditor
    {
        private MousePosition _mousePosition;

        private void OnEnable()
        {
            _mousePosition = target as MousePosition;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            Toggle(_mousePosition.IsCanOnUGUI, out _mousePosition.IsCanOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("X");
            FloatField(_mousePosition.XSpeed, out _mousePosition.XSpeed);
            GUILayout.Label("Y");
            FloatField(_mousePosition.YSpeed, out _mousePosition.YSpeed);
            GUILayout.Label("M");
            FloatField(_mousePosition.MSpeed, out _mousePosition.MSpeed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Move Damping");
            FloatField(_mousePosition.MoveDamping, out _mousePosition.MoveDamping);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            GUILayout.BeginVertical("HelpBox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Target Position Limit:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            Toggle(_mousePosition.NeedLimit, out _mousePosition.NeedLimit, "Need Limit");
            GUILayout.EndHorizontal();

            if (_mousePosition.NeedLimit)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("X Min");
                FloatField(_mousePosition.XMinLimit, out _mousePosition.XMinLimit);
                GUILayout.Label("X Max");
                FloatField(_mousePosition.XMaxLimit, out _mousePosition.XMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Y Min");
                FloatField(_mousePosition.YMinLimit, out _mousePosition.YMinLimit);
                GUILayout.Label("Y Max");
                FloatField(_mousePosition.YMaxLimit, out _mousePosition.YMaxLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Z Min");
                FloatField(_mousePosition.ZMinLimit, out _mousePosition.ZMinLimit);
                GUILayout.Label("Z Max");
                FloatField(_mousePosition.ZMaxLimit, out _mousePosition.ZMaxLimit);
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
