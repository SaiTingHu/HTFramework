using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ControllerManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89416110")]
    internal sealed class ControllerManagerInspector : InternalModuleInspector<ControllerManager, IControllerHelper>
    {
        private List<BoxBoundsHandle> _handles = new List<BoxBoundsHandle>();

        protected override string Intro => "Controller Manager, it encapsulation free view controller, first person controller, third person controller, etc!";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(ControllerManager.DefaultMode), "Default ControlMode");
            PropertyField(nameof(ControllerManager.DefaultEase), "Default Ease");
            PropertyField(nameof(ControllerManager.DefaultAutoPlay), "Default Auto Play");
            PropertyField(nameof(ControllerManager.IsAutoKill), "Tweener Auto Kill");
            
            GUI.enabled = true;

            PropertyField(nameof(ControllerManager.IsEnableBounds), "Enable Bounds");
            
            if (Target.IsEnableBounds)
            {
                PropertyField(nameof(ControllerManager.FreeControlBounds), "Free Control Bounds");
            }
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Main Camera: ", GUILayout.Width(LabelWidth));
            EditorGUILayout.ObjectField(Target.MainCamera, typeof(Camera), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ray Target: ", GUILayout.Width(LabelWidth));
            EditorGUILayout.ObjectField(Target.RayTargetObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Control Mode: ", GUILayout.Width(LabelWidth));
            Target.Mode = (ControlMode)EditorGUILayout.EnumPopup(Target.Mode);
            GUILayout.EndHorizontal();

            if (Target.Mode == ControlMode.FreeControl)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Position Control: ", GUILayout.Width(LabelWidth));
                Target.EnablePositionControl = EditorGUILayout.Toggle(Target.EnablePositionControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation Control: ", GUILayout.Width(LabelWidth));
                Target.EnableRotationControl = EditorGUILayout.Toggle(Target.EnableRotationControl);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Look Point: ", GUILayout.Width(LabelWidth));
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
                GUILayout.Label("Look Angle: ", GUILayout.Width(LabelWidth));
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
    }
}