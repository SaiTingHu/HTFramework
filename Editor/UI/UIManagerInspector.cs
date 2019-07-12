using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    public sealed class UIManagerInspector : ModuleEditor
    {
        private UIManager _target;

        private UILogicTemporary _currentOverlayTemporaryUI;
        private Dictionary<Type, UILogic> _overlayUIs;
        private UILogicTemporary _currentCameraTemporaryUI;
        private Dictionary<Type, UILogic> _cameraUIs;

        protected override void OnEnable()
        {
            _target = target as UIManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();

            _currentOverlayTemporaryUI = _target.GetType().GetField("_currentOverlayTemporaryUI", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as UILogicTemporary;
            _overlayUIs = _target.GetType().GetField("_overlayUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<Type, UILogic>;
            _currentCameraTemporaryUI = _target.GetType().GetField("_currentCameraTemporaryUI", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as UILogicTemporary;
            _cameraUIs = _target.GetType().GetField("_cameraUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_target) as Dictionary<Type, UILogic>;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("UI Manager, Control all UILogic Entity!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsEnableOverlayUI, out _target.IsEnableOverlayUI, "Is Enable Overlay UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsEnableCameraUI, out _target.IsEnableCameraUI, "Is Enable Camera UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(_target.IsEnableWorldUI, out _target.IsEnableWorldUI, "Is Enable World UI");
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Overlay UIs: " + (_target.IsEnableOverlayUI ? "[Enable]" : "[Prohibit]"));
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<Type, UILogic> ui in _overlayUIs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(ui.Key.Name + ": " + (ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                if (ui.Value.IsCreated)
                {
                    if (ui.Value.IsOpened)
                    {
                        if (GUILayout.Button("Close", "Minibutton"))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Open", "Minibutton"))
                        {
                            if (ui.Key.BaseType == typeof(UILogicResident))
                            {
                                Main.m_UI.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.BaseType == typeof(UILogicTemporary))
                            {
                                Main.m_UI.OpenTemporaryUI(ui.Key);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera UIs: " + (_target.IsEnableCameraUI ? "[Enable]" : "[Prohibit]"));
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<Type, UILogic> ui in _cameraUIs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(ui.Key.Name + ": " + (ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                if (ui.Value.IsCreated)
                {
                    if (ui.Value.IsOpened)
                    {
                        if (GUILayout.Button("Close", "Minibutton"))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Open", "Minibutton"))
                        {
                            if (ui.Key.BaseType == typeof(UILogicResident))
                            {
                                Main.m_UI.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.BaseType == typeof(UILogicTemporary))
                            {
                                Main.m_UI.OpenTemporaryUI(ui.Key);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
