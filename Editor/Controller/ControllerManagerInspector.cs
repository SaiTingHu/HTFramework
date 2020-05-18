using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89416110")]
    internal sealed class ControllerManagerInspector : InternalModuleInspector<ControllerManager>
    {
        private MousePosition _mousePosition;
        private MouseRotation _mouseRotation;
        private Vector3 _positionLimitCenter;
        private Vector3 _positionLimitSize;
        private Vector3 _limitMin;
        private Vector3 _limitMax;

        protected override string Intro
        {
            get
            {
                return "Controller Manager, It includes free control, first person control, third person control, etc!";
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _mousePosition = Target.GetType().GetField("_mousePosition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as MousePosition;
            _mouseRotation = Target.GetType().GetField("_mouseRotation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as MouseRotation;
            _positionLimitCenter.Set((_mousePosition.XMaxLimit - _mousePosition.XMinLimit) / 2 + _mousePosition.XMinLimit
                , (_mousePosition.YMaxLimit - _mousePosition.YMinLimit) / 2 + _mousePosition.YMinLimit
                , (_mousePosition.ZMaxLimit - _mousePosition.ZMinLimit) / 2 + _mousePosition.ZMinLimit);
            _positionLimitSize.Set(_mousePosition.XMaxLimit - _mousePosition.XMinLimit
                , _mousePosition.YMaxLimit - _mousePosition.YMinLimit
                , _mousePosition.ZMaxLimit - _mousePosition.ZMinLimit);
            _limitMin.Set(_mousePosition.XMinLimit, _mousePosition.YMinLimit, _mousePosition.ZMinLimit);
            _limitMax.Set(_mousePosition.XMaxLimit, _mousePosition.YMaxLimit, _mousePosition.ZMaxLimit);
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();
            
            GUILayout.BeginHorizontal();
            EnumPopup(Target.DefaultControlMode, out Target.DefaultControlMode, "Default ControlMode");
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
            GUILayout.Label("Main Camera: ", GUILayout.Width(100));
            EditorGUILayout.ObjectField(Target.MainCamera, typeof(Camera), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ray Target: ", GUILayout.Width(100));
            EditorGUILayout.ObjectField(Target.RayTargetObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Control Mode: ", GUILayout.Width(100));
            Target.TheControlMode = (ControlMode)EditorGUILayout.EnumPopup(Target.TheControlMode);
            GUILayout.EndHorizontal();

            if (Target.TheControlMode == ControlMode.FreeControl)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Need Limit: ", GUILayout.Width(100));
                Target.NeedLimit = EditorGUILayout.Toggle(Target.NeedLimit);
                GUILayout.EndHorizontal();

                if (Target.NeedLimit)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Min Limit: ", GUILayout.Width(60));
                    Vector3 vec = EditorGUILayout.Vector3Field("", _limitMin);
                    if (vec != _limitMin)
                    {
                        _limitMin = vec;
                        Target.SetMinLimit(_limitMin);
                        _positionLimitCenter.Set((_mousePosition.XMaxLimit - _mousePosition.XMinLimit) / 2 + _mousePosition.XMinLimit
                            , (_mousePosition.YMaxLimit - _mousePosition.YMinLimit) / 2 + _mousePosition.YMinLimit
                            , (_mousePosition.ZMaxLimit - _mousePosition.ZMinLimit) / 2 + _mousePosition.ZMinLimit);
                        _positionLimitSize.Set(_mousePosition.XMaxLimit - _mousePosition.XMinLimit
                            , _mousePosition.YMaxLimit - _mousePosition.YMinLimit
                            , _mousePosition.ZMaxLimit - _mousePosition.ZMinLimit);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Max Limit: ", GUILayout.Width(60));
                    vec = EditorGUILayout.Vector3Field("", _limitMax);
                    if (vec != _limitMax)
                    {
                        _limitMax = vec;
                        Target.SetMaxLimit(_limitMax);
                        _positionLimitCenter.Set((_mousePosition.XMaxLimit - _mousePosition.XMinLimit) / 2 + _mousePosition.XMinLimit
                            , (_mousePosition.YMaxLimit - _mousePosition.YMinLimit) / 2 + _mousePosition.YMinLimit
                            , (_mousePosition.ZMaxLimit - _mousePosition.ZMinLimit) / 2 + _mousePosition.ZMinLimit);
                        _positionLimitSize.Set(_mousePosition.XMaxLimit - _mousePosition.XMinLimit
                            , _mousePosition.YMaxLimit - _mousePosition.YMinLimit
                            , _mousePosition.ZMaxLimit - _mousePosition.ZMinLimit);
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Position Control: ", GUILayout.Width(100));
                Target.EnablePositionControl = EditorGUILayout.Toggle(Target.EnablePositionControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation Control: ", GUILayout.Width(100));
                Target.EnableRotationControl = EditorGUILayout.Toggle(Target.EnableRotationControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Point: ", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(Target.LookPoint.ToString());
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = Target.LookPoint.ToCopyString("F2");
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Angle: ", GUILayout.Width(100));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(Target.LookAngle.ToString());
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = Target.LookAngle.ToCopyString("F2");
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}