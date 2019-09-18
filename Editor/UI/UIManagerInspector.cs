using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/88125982")]
    public sealed class UIManagerInspector : HTFEditor<UIManager>
    {
        private Dictionary<Type, UILogicBase> _overlayUIs;
        private Dictionary<Type, UILogicBase> _cameraUIs;
        private bool _overlayUIFoldout = true;
        private bool _cameraUIFoldout = true;

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
            GUILayout.Space(10);
            _overlayUIFoldout = EditorGUILayout.Foldout(_overlayUIFoldout, "Overlay UIs", true);
            GUILayout.EndHorizontal();

            if (_overlayUIFoldout)
            {
                foreach (KeyValuePair<Type, UILogicBase> ui in _overlayUIs)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(ui.Key.Name);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label((ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.ObjectField(ui.Value.UIEntity, typeof(GameObject), true);
                    if (ui.Value.IsCreated)
                    {
                        GUILayout.FlexibleSpace();
                        GUI.enabled = !ui.Value.IsOpened;
                        if (GUILayout.Button("Open", "minibuttonleft", GUILayout.Width(45)))
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
                        GUI.enabled = ui.Value.IsOpened;
                        if (GUILayout.Button("Close", "minibuttonright", GUILayout.Width(45)))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _cameraUIFoldout = EditorGUILayout.Foldout(_cameraUIFoldout, "Camera UIs", true);
            GUILayout.EndHorizontal();

            if (_cameraUIFoldout)
            {
                foreach (KeyValuePair<Type, UILogicBase> ui in _cameraUIs)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(ui.Key.Name);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label((ui.Value.IsCreated ? "[Created]" : "[Uncreated]") + " " + (ui.Value.IsOpened ? "[Opened]" : "[Unopened]"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    EditorGUILayout.ObjectField(ui.Value.UIEntity, typeof(GameObject), true);
                    if (ui.Value.IsCreated)
                    {
                        GUILayout.FlexibleSpace();
                        GUI.enabled = !ui.Value.IsOpened;
                        if (GUILayout.Button("Open", "minibuttonleft", GUILayout.Width(45)))
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
                        GUI.enabled = ui.Value.IsOpened;
                        if (GUILayout.Button("Close", "minibuttonright", GUILayout.Width(45)))
                        {
                            Main.m_UI.CloseUI(ui.Key);
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
