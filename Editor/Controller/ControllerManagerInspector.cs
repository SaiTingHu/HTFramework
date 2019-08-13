using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    public sealed class ControllerManagerInspector : ModuleEditor
    {
        private ControllerManager _target;

        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private Vector3 _positionLimitCenter;
        private Vector3 _positionLimitSize;

        protected override void OnEnable()
        {
            _target = target as ControllerManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _mousePosition = _target.GetType().GetField("_mousePosition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as MousePosition;
            _mouseRotation = _target.GetType().GetField("_mouseRotation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as MouseRotation;
            _positionLimitCenter = new Vector3((_mousePosition.XMaxLimit - _mousePosition.XMinLimit) / 2 + _mousePosition.XMinLimit
                , (_mousePosition.YMaxLimit - _mousePosition.YMinLimit) / 2 + _mousePosition.YMinLimit
                , (_mousePosition.ZMaxLimit - _mousePosition.ZMinLimit) / 2 + _mousePosition.ZMinLimit);
            _positionLimitSize = new Vector3(_mousePosition.XMaxLimit - _mousePosition.XMinLimit
                , _mousePosition.YMaxLimit - _mousePosition.YMinLimit
                , _mousePosition.ZMaxLimit - _mousePosition.ZMinLimit);
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Controller Manager, It includes free control, first person control, third person control, etc!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(_target.DefaultEase, out _target.DefaultEase, "Default Ease");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EnumPopup(_target.DefaultAutoPlay, out _target.DefaultAutoPlay, "Default Auto Play");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsAutoKill, out _target.IsAutoKill, "Tweener Auto Kill");
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (_target.NeedLimit)
                {
                    Handles.color = Color.red;
                    Handles.DrawWireCube(_positionLimitCenter, _positionLimitSize);
                }
            }
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Main Camera: ", GUILayout.Width(120));
            EditorGUILayout.ObjectField(_target.MainCamera, typeof(Camera), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ray Target: ", GUILayout.Width(120));
            EditorGUILayout.ObjectField(_target.RayTargetObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Control Mode: ", GUILayout.Width(120));
            _target.TheControlMode = (ControlMode)EditorGUILayout.EnumPopup(_target.TheControlMode);
            GUILayout.EndHorizontal();

            if (_target.TheControlMode == ControlMode.FreeControl)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Need Limit: ", GUILayout.Width(120));
                _target.NeedLimit = EditorGUILayout.Toggle(_target.NeedLimit);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Position Control: ", GUILayout.Width(120));
                _target.EnablePositionControl = EditorGUILayout.Toggle(_target.EnablePositionControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation Control: ", GUILayout.Width(120));
                _target.EnableRotationControl = EditorGUILayout.Toggle(_target.EnableRotationControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Point: ", GUILayout.Width(120));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(_target.LookPoint.ToString());
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
                GUILayout.Label(_target.LookAngle.ToString());
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

            GUILayout.EndVertical();
        }
    }
}
