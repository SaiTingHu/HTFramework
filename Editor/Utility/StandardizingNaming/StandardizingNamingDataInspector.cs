using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StandardizingNamingData))]
    internal sealed class StandardizingNamingDataInspector : HTFEditor<StandardizingNamingData>
    {
        private ShowType _showType = ShowType.Hierarchy;
        private GUIContent _addGC;
        private GUIContent _removeGC;
        private SerializedProperty _hierarchyProperty;
        private ReorderableList _hierarchyList;
        private SerializedProperty _projectProperty;
        private ReorderableList _projectList;
        private HashSet<NamingSign> _selects = new HashSet<NamingSign>();

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add a naming sign";
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            _removeGC.tooltip = "Remove select naming sign";

            _hierarchyProperty = GetProperty("HierarchyNamingSigns");
            _hierarchyList = new ReorderableList(serializedObject, _hierarchyProperty, true, true, false, false);
            _hierarchyList.footerHeight = 0;
            _hierarchyList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x + 35, rect.y, LabelWidth, rect.height);
                GUI.Label(sub, "Component Type");

                sub.Set(rect.x + 35 + LabelWidth, rect.y, rect.width - 35 - LabelWidth, rect.height);
                GUI.Label(sub, "Naming");

                sub.Set(rect.x + rect.width - 40, rect.y, 20, rect.height);
                if (GUI.Button(sub, _addGC, EditorGlobalTools.Styles.IconButton))
                {
                    Undo.RecordObject(target, "Add a naming sign");
                    AddHierarchyNamingSign();
                    HasChanged();
                }

                sub.Set(rect.x + rect.width - 20, rect.y, 20, rect.height);
                GUI.enabled = _selects.Count > 0;
                if (GUI.Button(sub, _removeGC, EditorGlobalTools.Styles.IconButton))
                {
                    Undo.RecordObject(target, "Remove select naming sign");
                    RemoveHierarchyNamingSign(_selects);
                    HasChanged();
                }
                GUI.enabled = true;
            };
            _hierarchyList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.HierarchyNamingSigns.Count)
                {
                    OnNamingSignGUI(rect, Target.HierarchyNamingSigns[index], ShowType.Hierarchy);
                }
            };
            _hierarchyList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
            _projectProperty = GetProperty("ProjectNamingSigns");
            _projectList = new ReorderableList(serializedObject, _projectProperty, true, true, false, false);
            _projectList.footerHeight = 0;
            _projectList.drawHeaderCallback = (Rect rect) =>
            {
                Rect sub = rect;
                sub.Set(rect.x + 35, rect.y, LabelWidth, rect.height);
                GUI.Label(sub, "Object Type");

                sub.Set(rect.x + 35 + LabelWidth, rect.y, rect.width - 35 - LabelWidth, rect.height);
                GUI.Label(sub, "Naming");

                sub.Set(rect.x + rect.width - 40, rect.y, 20, rect.height);
                if (GUI.Button(sub, _addGC, EditorGlobalTools.Styles.IconButton))
                {
                    Undo.RecordObject(target, "Add a naming sign");
                    AddProjectNamingSign();
                    HasChanged();
                }

                sub.Set(rect.x + rect.width - 20, rect.y, 20, rect.height);
                GUI.enabled = _selects.Count > 0;
                if (GUI.Button(sub, _removeGC, EditorGlobalTools.Styles.IconButton))
                {
                    Undo.RecordObject(target, "Remove select naming sign");
                    RemoveProjectNamingSign(_selects);
                    HasChanged();
                }
                GUI.enabled = true;
            };
            _projectList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= 0 && index < Target.ProjectNamingSigns.Count)
                {
                    OnNamingSignGUI(rect, Target.ProjectNamingSigns[index], ShowType.Project);
                }
            };
            _projectList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            if (targets.Length == 1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                ShowType lastType = _showType;
                if (GUILayout.Toggle(_showType == ShowType.Hierarchy, "Hierarchy", "LargeButtonLeft", GUILayout.Width(100)))
                {
                    _showType = ShowType.Hierarchy;
                }
                if (GUILayout.Toggle(_showType == ShowType.Project, "Project", "LargeButtonRight", GUILayout.Width(100)))
                {
                    _showType = ShowType.Project;
                }
                if (lastType != _showType)
                {
                    _selects.Clear();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                if (_showType == ShowType.Hierarchy)
                {
                    _hierarchyList.DoLayoutList();
                }
                else
                {
                    _projectList.DoLayoutList();
                }
            }
        }
        private void OnNamingSignGUI(Rect rect, NamingSign namingSign, ShowType showType)
        {
            List<NamingSign> namingSigns = showType == ShowType.Hierarchy ? Target.HierarchyNamingSigns : Target.ProjectNamingSigns;

            if (string.IsNullOrEmpty(namingSign.FullName))
            {
                namingSign.Target = null;
                GUI.contentColor = Color.gray;
            }
            else
            {
                if (namingSign.Target == null)
                {
                    namingSign.Target = ReflectionToolkit.GetTypeInRunTimeAssemblies(namingSign.FullName);
                }
                else
                {
                    if (namingSign.Target.FullName != namingSign.FullName)
                    {
                        namingSign.Target = ReflectionToolkit.GetTypeInRunTimeAssemblies(namingSign.FullName);
                    }
                }
                GUI.contentColor = Color.white;
            }

            Rect sub = rect;
            sub.Set(rect.x, rect.y, 20, rect.height);
            bool contain = _selects.Contains(namingSign);
            bool newContain = GUI.Toggle(sub, contain, "");
            if (newContain != contain)
            {
                if (newContain) _selects.Add(namingSign);
                else _selects.Remove(namingSign);
            }

            GUIContent gc = namingSign.Target == null ? new GUIContent() : EditorGUIUtility.ObjectContent(null, namingSign.Target);
            gc.text = namingSign.Target == null ? "<None>" : namingSign.Name;
            gc.tooltip = namingSign.FullName;
            sub.Set(rect.x + 20, rect.y + 2, LabelWidth, rect.height);
            if (GUI.Button(sub, gc, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = null;
                if (showType == ShowType.Hierarchy)
                {
                    types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                    {
                        return type.IsSubclassOf(typeof(Component)) && !type.IsAbstract;
                    });
                }
                else
                {
                    types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                    {
                        return type.IsSubclassOf(typeof(UnityEngine.Object)) && !type.IsSubclassOf(typeof(Component)) && !type.IsAbstract;
                    });
                }
                for (int i = 0; i < types.Count; i++)
                {
                    Type type = types[i];
                    gm.AddItem(new GUIContent(type.FullName.Replace(".", "/")), false, () =>
                    {
                        bool exist = namingSigns.Exists((s) => { return s.FullName == type.FullName; });
                        if (!exist)
                        {
                            Undo.RecordObject(target, "Change naming sign");
                            namingSign.Target = type;
                            namingSign.Name = type.Name;
                            namingSign.FullName = type.FullName;
                            HasChanged();
                        }
                    });
                }
                gm.ShowAsContext();
            }

            EditorGUI.BeginChangeCheck();
            sub.Set(rect.x + 20 + LabelWidth, rect.y + 2, rect.width - 20 - LabelWidth, 18);
            string signValue = EditorGUI.TextField(sub, namingSign.Sign);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change naming sign");
                namingSign.Sign = signValue;
                HasChanged();
            }

            GUI.contentColor = Color.white;
        }

        /// <summary>
        /// 新增Hierarchy命名标记
        /// </summary>
        private void AddHierarchyNamingSign()
        {
            Target.HierarchyNamingSigns.Add(new NamingSign(null, "Prefix_" + Target.NameMatch));
        }
        /// <summary>
        /// 移除选中的Hierarchy命名标记
        /// </summary>
        private void RemoveHierarchyNamingSign(HashSet<NamingSign> selects)
        {
            foreach (var sign in selects)
            {
                Target.HierarchyNamingSigns.Remove(sign);
            }
            selects.Clear();
        }
        /// <summary>
        /// 新增Project命名标记
        /// </summary>
        private void AddProjectNamingSign()
        {
            Target.ProjectNamingSigns.Add(new NamingSign(null, "Prefix_" + Target.NameMatch));
        }
        /// <summary>
        /// 移除选中的Project命名标记
        /// </summary>
        private void RemoveProjectNamingSign(HashSet<NamingSign> selects)
        {
            foreach (var sign in selects)
            {
                Target.ProjectNamingSigns.Remove(sign);
            }
            selects.Clear();
        }

        private enum ShowType
        {
            Hierarchy,
            Project
        }
    }
}