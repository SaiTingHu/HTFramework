using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/88125982")]
    internal sealed class UIManagerInspector : InternalModuleInspector<UIManager, IUIHelper>
    {
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private ReorderableList _uiList;
        private bool _overlayUIFoldout = true;
        private bool _cameraUIFoldout = true;

        protected override string Intro => "UI Manager, this is the master controller for all UIs!";

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a new define";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select define";

            _uiList = new ReorderableList(Target.DefineUINames, typeof(string), true, true, false, false);
            _uiList.elementHeight = 45;
            _uiList.footerHeight = 0;
            _uiList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x, rect.y, 200, rect.height);
                GUI.Label(sub, "Define UI:");

                if (!EditorApplication.isPlaying)
                {
                    sub.Set(rect.x + rect.width - 40, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
                    {
                        Target.DefineUINames.Add("<None>");
                        Target.DefineUIEntitys.Add(null);
                        HasChanged();
                    }

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    GUI.enabled = _uiList.index >= 0 && _uiList.index < Target.DefineUINames.Count;
                    if (GUI.Button(sub, _removeGC, "InvisibleButton"))
                    {
                        Target.DefineUINames.RemoveAt(_uiList.index);
                        Target.DefineUIEntitys.RemoveAt(_uiList.index);
                        HasChanged();
                    }
                    GUI.enabled = true;
                }
            };
            _uiList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.DefineUINames.Count)
                {
                    Rect subrect = rect;

                    subrect.Set(rect.x, rect.y + 2, 50, 16);
                    GUI.Label(subrect, "Type");
                    if (isActive)
                    {
                        subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                        if (GUI.Button(subrect, Target.DefineUINames[index], EditorGlobalTools.Styles.MiniPopup))
                        {
                            GenericMenu gm = new GenericMenu();
                            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                            {
                                return type.IsSubclassOf(typeof(UILogicBase)) && !type.IsAbstract;
                            });
                            for (int m = 0; m < types.Count; m++)
                            {
                                int j = index;
                                int n = m;
                                if (Target.DefineUINames.Contains(types[n].FullName))
                                {
                                    gm.AddDisabledItem(new GUIContent(types[n].FullName), Target.DefineUINames[j] == types[n].FullName);
                                }
                                else
                                {
                                    gm.AddItem(new GUIContent(types[n].FullName), Target.DefineUINames[j] == types[n].FullName, () =>
                                    {
                                        Target.DefineUINames[j] = types[n].FullName;
                                        HasChanged();
                                    });
                                }
                            }
                            gm.ShowAsContext();
                        }
                    }
                    else
                    {
                        subrect.Set(rect.x + 50, rect.y + 2, rect.width - 50, 16);
                        GUI.Label(subrect, Target.DefineUINames[index]);
                    }

                    subrect.Set(rect.x, rect.y + 22, 50, 16);
                    GUI.Label(subrect, "Entity");
                    subrect.Set(rect.x + 50, rect.y + 22, rect.width - 50, 16);
                    GameObject entity = EditorGUI.ObjectField(subrect, Target.DefineUIEntitys[index], typeof(GameObject), false) as GameObject;
                    if (entity != Target.DefineUIEntitys[index])
                    {
                        Target.DefineUIEntitys[index] = entity;
                        HasChanged();
                    }
                }
            };
            _uiList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                    gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                    rect.x += 2;
                    rect.width -= 6;
                    gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                }
            };
            _uiList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) =>
            {
                GameObject entity = Target.DefineUIEntitys[oldIndex];
                Target.DefineUIEntitys.RemoveAt(oldIndex);
                Target.DefineUIEntitys.Insert(newIndex, entity);
                HasChanged();
            };
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(UIManager.IsEnableOverlayUI), "Enable Overlay UI");
            PropertyField(nameof(UIManager.IsEnableCameraUI), "Enable Camera UI");
            PropertyField(nameof(UIManager.IsEnableWorldUI), "Enable World UI");
            
            _uiList.DoLayoutList();

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            Target.IsHideAll = EditorGUILayout.Toggle("Hide All", Target.IsHideAll);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Target.IsDisplayMask = EditorGUILayout.Toggle("Display Mask", Target.IsDisplayMask);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Target.IsLockTemporaryUI = EditorGUILayout.Toggle("Lock Temporary", Target.IsLockTemporaryUI);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            _overlayUIFoldout = EditorGUILayout.Foldout(_overlayUIFoldout, "Overlay UIs", true);
            GUILayout.EndHorizontal();

            if (_overlayUIFoldout)
            {
                foreach (var ui in _helper.OverlayUIs)
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
                            Target.OpenUI(ui.Key);
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
                foreach (var ui in _helper.CameraUIs)
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
                            Target.OpenUI(ui.Key);
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