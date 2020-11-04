using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/88125982")]
    internal sealed class UIManagerInspector : InternalModuleInspector<UIManager>
    {
        private IUIHelper _UIHelper;
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

            _UIHelper = _helper as IUIHelper;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

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

            GUI.enabled = true;
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
                foreach (var ui in _UIHelper.OverlayUIs)
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
                                Target.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.IsSubclassOf(typeof(UILogicTemporary)))
                            {
                                Target.OpenTemporaryUI(ui.Key);
                            }
                        }
                        GUI.enabled = ui.Value.IsOpened;
                        if (GUILayout.Button("Close", EditorStyles.miniButtonRight, GUILayout.Width(45)))
                        {
                            Target.CloseUI(ui.Key);
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
                foreach (var ui in _UIHelper.CameraUIs)
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
                                Target.OpenResidentUI(ui.Key);
                            }
                            else if (ui.Key.IsSubclassOf(typeof(UILogicTemporary)))
                            {
                                Target.OpenTemporaryUI(ui.Key);
                            }
                        }
                        GUI.enabled = ui.Value.IsOpened;
                        if (GUILayout.Button("Close", EditorStyles.miniButtonRight, GUILayout.Width(45)))
                        {
                            Target.CloseUI(ui.Key);
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
