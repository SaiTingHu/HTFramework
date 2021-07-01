using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(ControllerManager))]
    internal sealed class DebuggerControllerManager : DebuggerComponentBase
    {
        private ControllerManager _target;

        public override void OnEnable()
        {
            _target = Target as ControllerManager;
        }
        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            _target.IsEnableBounds = GUILayout.Toggle(_target.IsEnableBounds, "Is Enable Bounds");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.EnablePositionControl = GUILayout.Toggle(_target.EnablePositionControl, "Enable Position Control");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.EnableRotationControl = GUILayout.Toggle(_target.EnableRotationControl, "Enable Rotation Control");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.IsCanControlOnUGUI = GUILayout.Toggle(_target.IsCanControlOnUGUI, "Is Can Control On UGUI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.EnableHighlightingEffect = GUILayout.Toggle(_target.EnableHighlightingEffect, "Enable Highlighting Effect");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.EnableMouseRay = GUILayout.Toggle(_target.EnableMouseRay, "Enable MouseRay");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Control Mode: ", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.TheControlMode = (ControlMode)EnumField(_target.TheControlMode);
            GUILayout.EndHorizontal();
        }
    }
}