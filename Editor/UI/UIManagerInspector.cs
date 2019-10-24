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

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Define UI", "BoldLabel");
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.DefineUINames.Count; i++)
            {
                GUILayout.BeginVertical("HelpBox");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(40));
                if (GUILayout.Button(Target.DefineUINames[i], "MiniPopup"))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                    for (int m = 0; m < types.Count; m++)
                    {
                        if (types[m].IsSubclassOf(typeof(UILogicBase)))
                        {
                            int j = i;
                            int n = m;
                            if (Target.DefineUINames.Contains(types[n].FullName))
                            {
                                gm.AddDisabledItem(new GUIContent(types[n].FullName));
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(types[n].FullName), Target.DefineUINames[j] == types[n].FullName, () =>
                                {
                                    Undo.RecordObject(target, "Set Define UI Name");
                                    Target.DefineUINames[j] = types[n].FullName;
                                    HasChanged();
                                });
                            }
                        }
                    }
                    gm.ShowAsContext();
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete", "Minibutton", GUILayout.Width(50)))
                {
                    Undo.RecordObject(target, "Delete Define UI");
                    Target.DefineUINames.RemoveAt(i);
                    Target.DefineUIEntitys.RemoveAt(i);
                    HasChanged();
                    continue;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Entity", GUILayout.Width(40));
                GameObject entity = Target.DefineUIEntitys[i];
                ObjectField(Target.DefineUIEntitys[i], out entity, false, "");
                if (entity != Target.DefineUIEntitys[i])
                {
                    Target.DefineUIEntitys[i] = entity;
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New", "Minibutton"))
            {
                Undo.RecordObject(target, "New Define UI");
                Target.DefineUINames.Add("<None>");
                Target.DefineUIEntitys.Add(null);
                HasChanged();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
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
