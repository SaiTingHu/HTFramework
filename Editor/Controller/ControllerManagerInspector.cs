using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89416110")]
    internal sealed class ControllerManagerInspector : InternalModuleInspector<ControllerManager>
    {
        private List<BoxBoundsHandle> _handles = new List<BoxBoundsHandle>();
        
        protected override string Intro
        {
            get
            {
                return "Controller Manager, It includes free control, first person control, third person control, etc!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IControllerHelper);
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;
            
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

            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableBounds, out Target.IsEnableBounds, "Enable Bounds");
            GUILayout.EndHorizontal();

            if (Target.IsEnableBounds)
            {
                PropertyField("FreeControlBounds");
            }
        }

        private void OnSceneGUI()
        {
            if (Target.IsEnableBounds)
            {
                if (_handles.Count != Target.FreeControlBounds.Count)
                {
                    while (_handles.Count < Target.FreeControlBounds.Count)
                    {
                        _handles.Add(new BoxBoundsHandle());
                    }
                    while (_handles.Count > Target.FreeControlBounds.Count)
                    {
                        _handles.RemoveAt(0);
                    }
                }

                if (_handles.Count > 0)
                {
                    using (new Handles.DrawingScope(Color.cyan))
                    {
                        EditorGUI.BeginChangeCheck();
                        for (int i = 0; i < _handles.Count; i++)
                        {
                            _handles[i].center = Target.FreeControlBounds[i].center;
                            _handles[i].size = Target.FreeControlBounds[i].size;
                            _handles[i].DrawHandle();
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(Target, "Change FreeControl Bounds");
                            for (int i = 0; i < _handles.Count; i++)
                            {
                                Bounds bounds = Target.FreeControlBounds[i];
                                bounds.center = _handles[i].center;
                                bounds.size = _handles[i].size;
                                Target.FreeControlBounds[i] = bounds;
                            }
                            HasChanged();
                        }
                    }
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