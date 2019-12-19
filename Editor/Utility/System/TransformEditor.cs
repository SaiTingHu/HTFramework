using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Transform))]
    public sealed class TransformEditor : HTFEditor<Transform>
    {
        private static bool _copyQuaternion = false;
        private bool _showProperty = true;
        private bool _showHierarchy = false;
        private bool _showCopy = false;
        private Transform _parent;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _showProperty = EditorPrefs.GetBool(EditorPrefsTable.Transform_Property, true);
            _showHierarchy = EditorPrefs.GetBool(EditorPrefsTable.Transform_Hierarchy, false);
            _showCopy = EditorPrefs.GetBool(EditorPrefsTable.Transform_Copy, false);
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            #region Property
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            bool showProperty = EditorGUILayout.Foldout(_showProperty, "Property", true);
            if (showProperty != _showProperty)
            {
                _showProperty = showProperty;
                EditorPrefs.SetBool(EditorPrefsTable.Transform_Property, _showProperty);
            }
            GUILayout.EndHorizontal();

            if (_showProperty)
            {
                GUILayout.BeginVertical("Box");
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Position", GUILayout.Width(80));
                Vector3 pos = EditorGUILayout.Vector3Field("", Target.position);
                if (pos != Target.position)
                {
                    Undo.RecordObject(Target, "Move " + Target.name);
                    Target.position = pos;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rotation", GUILayout.Width(80));
                Vector3 rot = EditorGUILayout.Vector3Field("", Target.rotation.eulerAngles);
                if (rot != Target.rotation.eulerAngles)
                {
                    Undo.RecordObject(Target, "Rotate " + Target.name);
                    Target.rotation = Quaternion.Euler(rot);
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scale", GUILayout.Width(80));
                GUI.enabled = false;
                EditorGUILayout.Vector3Field("", Target.lossyScale);
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalPosition", GUILayout.Width(80));
                Vector3 localpos = EditorGUILayout.Vector3Field("", Target.localPosition);
                if (localpos != Target.localPosition)
                {
                    Undo.RecordObject(Target, "Move " + Target.name);
                    Target.localPosition = localpos;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalRotation", GUILayout.Width(80));
                Vector3 localrot = EditorGUILayout.Vector3Field("", Target.localRotation.eulerAngles);
                if (localrot != Target.localRotation.eulerAngles)
                {
                    Undo.RecordObject(Target, "Rotate " + Target.name);
                    Target.localRotation = Quaternion.Euler(localrot);
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LocalScale", GUILayout.Width(80));
                Vector3 localsca = EditorGUILayout.Vector3Field("", Target.localScale);
                if (localsca != Target.localScale)
                {
                    Undo.RecordObject(Target, "Scale " + Target.name);
                    Target.localScale = localsca;
                    HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion

            #region Hierarchy
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            bool showHierarchy = EditorGUILayout.Foldout(_showHierarchy, "Hierarchy", true);
            if (showHierarchy != _showHierarchy)
            {
                _showHierarchy = showHierarchy;
                EditorPrefs.SetBool(EditorPrefsTable.Transform_Hierarchy, _showHierarchy);
            }
            GUILayout.EndHorizontal();

            if (_showHierarchy)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Root: ", GUILayout.Width(80));
                EditorGUILayout.ObjectField(Target.root, typeof(Transform), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Parent: ", GUILayout.Width(80));
                GUI.color = Target.parent ? Color.white : Color.gray;
                _parent = EditorGUILayout.ObjectField(Target.parent, typeof(Transform), true) as Transform;
                if (_parent != Target.parent)
                {
                    Undo.RecordObject(Target, "Change Parent " + Target.name);
                    Target.SetParent(_parent);
                    HasChanged();
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Child Count: ", GUILayout.Width(80));
                GUILayout.Label(Target.childCount.ToString());
                GUILayout.FlexibleSpace();
                GUI.enabled = Target.childCount > 0;
                if (GUILayout.Button("Detach", "Minibutton"))
                {
                    Undo.RecordObject(Target, "Detach Children");
                    Target.DetachChildren();
                    HasChanged();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Expand Children", "MinibuttonLeft"))
                {
                    Type type = EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
                    EditorWindow window = EditorWindow.GetWindow(type);
                    MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
                    int id = Target.gameObject.GetInstanceID();
                    method.Invoke(window, new object[] { id, true });
                }
                if (GUILayout.Button("Retract Children", "MinibuttonRight"))
                {
                    Type type = EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
                    EditorWindow window = EditorWindow.GetWindow(type);
                    MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
                    int id = Target.gameObject.GetInstanceID();
                    method.Invoke(window, new object[] { id, false });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Retract All", "Minibutton"))
                {
                    Type type = EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
                    EditorWindow window = EditorWindow.GetWindow(type);
                    object hierarchy = window.GetType().GetProperty("sceneHierarchy", BindingFlags.Public | BindingFlags.Instance).GetValue(window);
                    int[] expandedIDs = hierarchy.GetType().GetMethod("GetExpandedIDs", BindingFlags.Public | BindingFlags.Instance).Invoke(hierarchy, null) as int[];
                    MethodInfo method = hierarchy.GetType().GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);
                    object[] args = new object[2];
                    args[1] = false;
                    for (int i = 0; i < expandedIDs.Length; i++)
                    {
                        args[0] = expandedIDs[i];
                        method.Invoke(hierarchy, args);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion

            #region Copy
            GUILayout.BeginHorizontal("MeTransitionHead");
            GUILayout.Space(12);
            bool showCopy = EditorGUILayout.Foldout(_showCopy, "Copy", true);
            if (showCopy != _showCopy)
            {
                _showCopy = showCopy;
                EditorPrefs.SetBool(EditorPrefsTable.Transform_Copy, _showCopy);
            }
            GUILayout.EndHorizontal();

            if (_showCopy)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Position", "MiniButtonLeft"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.position.x.ToString("F4") + "f," +
                        Target.position.y.ToString("F4") + "f," +
                        Target.position.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy LocalPosition", "MiniButtonRight"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.localPosition.x.ToString("F4") + "f," +
                        Target.localPosition.y.ToString("F4") + "f," +
                        Target.localPosition.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Rotation", "MiniButtonLeft"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        Target.rotation.x.ToString("F4") + "f," +
                        Target.rotation.y.ToString("F4") + "f," +
                        Target.rotation.z.ToString("F4") + "f," +
                        Target.rotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = ClampAngle(Target.rotation.eulerAngles.x).ToString();
                        string y = ClampAngle(Target.rotation.eulerAngles.y).ToString();
                        string z = ClampAngle(Target.rotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                if (GUILayout.Button("Copy LocalRotation", "MiniButtonRight"))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer =
                        Target.localRotation.x.ToString("F4") + "f," +
                        Target.localRotation.y.ToString("F4") + "f," +
                        Target.localRotation.z.ToString("F4") + "f," +
                        Target.localRotation.w.ToString("F4") + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = ClampAngle(Target.localRotation.eulerAngles.x).ToString();
                        string y = ClampAngle(Target.localRotation.eulerAngles.y).ToString();
                        string z = ClampAngle(Target.localRotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Scale", "MiniButton"))
                {
                    GUIUtility.systemCopyBuffer =
                        Target.localScale.x.ToString("F4") + "f," +
                        Target.localScale.y.ToString("F4") + "f," +
                        Target.localScale.z.ToString("F4") + "f";
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Name", "MiniButtonLeft"))
                {
                    GUIUtility.systemCopyBuffer = Target.name;
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy FullName", "MiniButtonRight"))
                {
                    GUIUtility.systemCopyBuffer = Target.FullName();
                    GlobalTools.LogInfo("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _copyQuaternion = GUILayout.Toggle(_copyQuaternion, "Copy Quaternion");
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }

        private int ClampAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            else if (angle < -180) angle += 360;

            return (int)angle;
        }
    }
}