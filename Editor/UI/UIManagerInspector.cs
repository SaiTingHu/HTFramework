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
        private Dictionary<Type, UILogicBase> _overlayUIs;
        private Dictionary<Type, UILogicBase> _cameraUIs;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _overlayUIs = Target.GetType().GetField("_overlayUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, UILogicBase>;
            _cameraUIs = Target.GetType().GetField("_cameraUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, UILogicBase>;
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
            GUILayout.Label("Overlay UIs: " + (Target.IsEnableOverlayUI ? "[Enable]" : "[Disable]"));
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<Type, UILogicBase> ui in _overlayUIs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(ui.Key.Name);
                if (ui.Value.IsCreated)
                {
                    if (ui.Value.IsOpened)
                    {
                        if (GUILayout.Button("Close", "Minibutton", GUILayout.Width(60)))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Open", "Minibutton", GUILayout.Width(60)))
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

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.Label((ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera UIs: " + (Target.IsEnableCameraUI ? "[Enable]" : "[Disable]"));
            GUILayout.EndHorizontal();

            foreach (KeyValuePair<Type, UILogicBase> ui in _cameraUIs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(ui.Key.Name);
                if (ui.Value.IsCreated)
                {
                    if (ui.Value.IsOpened)
                    {
                        if (GUILayout.Button("Close", "Minibutton", GUILayout.Width(60)))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Open", "Minibutton", GUILayout.Width(60)))
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

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.Label((ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                GUILayout.EndHorizontal();
            }
        }
    }
}
