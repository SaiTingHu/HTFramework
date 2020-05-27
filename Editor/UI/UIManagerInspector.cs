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
    internal sealed class UIManagerInspector : InternalModuleInspector<UIManager>
    {
        private IUIHelper _UIHelper;
        private Dictionary<Type, UILogicBase> _overlayUIs;
        private Dictionary<Type, UILogicBase> _cameraUIs;
        private bool _overlayUIFoldout = true;
        private bool _cameraUIFoldout = true;

        protected override string Intro
        {
            get
            {
                return "UI Manager, Control all UILogic Entity!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IUIHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _UIHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IUIHelper;
            _overlayUIs = _UIHelper.GetType().GetField("_overlayUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_UIHelper) as Dictionary<Type, UILogicBase>;
            _cameraUIs = _UIHelper.GetType().GetField("_cameraUIs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_UIHelper) as Dictionary<Type, UILogicBase>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableOverlayUI, out Target.IsEnableOverlayUI, "Is Enable Overlay UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableCameraUI, out Target.IsEnableCameraUI, "Is Enable Camera UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableWorldUI, out Target.IsEnableWorldUI, "Is Enable World UI");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Define UI:");
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.DefineUINames.Count; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(40));
                if (GUILayout.Button(Target.DefineUINames[i], EditorGlobalTools.Styles.MiniPopup))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                    {
                        return type.IsSubclassOf(typeof(UILogicBase)) && type != typeof(UILogicResident) && type != typeof(UILogicTemporary);
                    });
                    for (int m = 0; m < types.Count; m++)
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
                    gm.ShowAsContext();
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50)))
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
            if (GUILayout.Button("New", EditorStyles.miniButton))
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
            Target.IsLockTemporaryUI = EditorGUILayout.Toggle("Lock Temporary", Target.IsLockTemporaryUI);
            GUILayout.EndHorizontal();

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
                        if (GUILayout.Button("Open", EditorStyles.miniButtonLeft, GUILayout.Width(45)))
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
                        if (GUILayout.Button("Close", EditorStyles.miniButtonRight, GUILayout.Width(45)))
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
                        if (GUILayout.Button("Open", EditorStyles.miniButtonLeft, GUILayout.Width(45)))
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
                        if (GUILayout.Button("Close", EditorStyles.miniButtonRight, GUILayout.Width(45)))
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
