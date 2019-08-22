using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    public sealed class ControllerManagerInspector : HTFEditor<ControllerManager>
    {
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private Vector3 _positionLimitCenter;
        private Vector3 _positionLimitSize;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _mousePosition = Target.GetType().GetField("_mousePosition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as MousePosition;
            _mouseRotation = Target.GetType().GetField("_mouseRotation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as MouseRotation;
            _positionLimitCenter = new Vector3((_mousePosition.XMaxLimit - _mousePosition.XMinLimit) / 2 + _mousePosition.XMinLimit
                , (_mousePosition.YMaxLimit - _mousePosition.YMinLimit) / 2 + _mousePosition.YMinLimit
                , (_mousePosition.ZMaxLimit - _mousePosition.ZMinLimit) / 2 + _mousePosition.ZMinLimit);
            _positionLimitSize = new Vector3(_mousePosition.XMaxLimit - _mousePosition.XMinLimit
                , _mousePosition.YMaxLimit - _mousePosition.YMinLimit
                , _mousePosition.ZMaxLimit - _mousePosition.ZMinLimit);
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Controller Manager, It includes free control, first person control, third person control, etc!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.DefaultEase, out Target.DefaultEase, "Default Ease");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EnumPopup(Target.DefaultAutoPlay, out Target.DefaultAutoPlay, "Default Auto Play");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsAutoKill, out Target.IsAutoKill, "Tweener Auto Kill");
            GUILayout.EndHorizontal();
        }

        public void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (Target.NeedLimit)
                {
                    Handles.color = Color.red;
                    Handles.DrawWireCube(_positionLimitCenter, _positionLimitSize);
                }
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Main Camera: ", GUILayout.Width(120));
            EditorGUILayout.ObjectField(Target.MainCamera, typeof(Camera), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ray Target: ", GUILayout.Width(120));
            EditorGUILayout.ObjectField(Target.RayTargetObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Control Mode: ", GUILayout.Width(120));
            Target.TheControlMode = (ControlMode)EditorGUILayout.EnumPopup(Target.TheControlMode);
            GUILayout.EndHorizontal();

            if (Target.TheControlMode == ControlMode.FreeControl)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Need Limit: ", GUILayout.Width(120));
                Target.NeedLimit = EditorGUILayout.Toggle(Target.NeedLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Position Control: ", GUILayout.Width(120));
                Target.EnablePositionControl = EditorGUILayout.Toggle(Target.EnablePositionControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation Control: ", GUILayout.Width(120));
                Target.EnableRotationControl = EditorGUILayout.Toggle(Target.EnableRotationControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Point: ", GUILayout.Width(120));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(Target.LookPoint.ToString());
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy", "Minibutton"))
                {
                    GUIUtility.systemCopyBuffer =
                           _mousePosition.Target.transform.localPosition.x.ToString("F2") + "f," +
                           _mousePosition.Target.transform.localPosition.y.ToString("F2") + "f," +
                           _mousePosition.Target.transform.localPosition.z.ToString("F2") + "f";
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Angle: ", GUILayout.Width(120));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(Target.LookAngle.ToString());
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy", "Minibutton"))
                {
                    GUIUtility.systemCopyBuffer =
                       _mouseRotation.X.ToString("F2") + "f," +
                       _mouseRotation.Y.ToString("F2") + "f," +
                       _mouseRotation.Distance.ToString("F2") + "f";
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
