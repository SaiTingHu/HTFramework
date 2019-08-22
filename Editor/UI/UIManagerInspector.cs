using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    public sealed class UIManagerInspector : HTFEditor<UIManager>
    {
        private UILogicTemporary _currentOverlayTemporaryUI;
        private Dictionary<Type, UILogic> _overlayUIs;
        private UILogicTemporary _currentCameraTemporaryUI;
        private Dictionary<Type, UILogic> _cameraUIs;
        
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _currentOverlayTemporaryUI = Target.GetType().GetField("_currentOverlayTemporaryUI", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as UILogicTemporary;
            _overlayUIs = Target.GetType().GetField("_overlayUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, UILogic>;
            _currentCameraTemporaryUI = Target.GetType().GetField("_currentCameraTemporaryUI", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as UILogicTemporary;
            _cameraUIs = Target.GetType().GetField("_cameraUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, UILogic>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("UI Manager, Control all UILogic Entity!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableOverlayUI, out Target.IsEnableOverlayUI, "Is Enable Overlay UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableCameraUI, out Target.IsEnableCameraUI, "Is Enable Camera UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableWorldUI, out Target.IsEnableWorldUI, "Is Enable World UI");
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Overlay UIs: " + (Target.IsEnableOverlayUI ? "[Enable]" : "[Prohibit]"));
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
                            if (ui.Key.IsSubclassOf(typeof(UILogicResident)))
                            {
                                Main.m_UI.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.IsSubclassOf(typeof(UILogicTemporary)))
                            {
                                Main.m_UI.OpenTemporaryUI(ui.Key);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera UIs: " + (Target.IsEnableCameraUI ? "[Enable]" : "[Prohibit]"));
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
                            if (ui.Key.IsSubclassOf(typeof(UILogicResident)))
                            {
                                Main.m_UI.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.IsSubclassOf(typeof(UILogicTemporary)))
                            {
                                Main.m_UI.OpenTemporaryUI(ui.Key);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
