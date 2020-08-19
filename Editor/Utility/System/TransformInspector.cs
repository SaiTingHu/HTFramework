using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Transform))]
    internal sealed class TransformInspector : HTFEditor<Transform>
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
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
                
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
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

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
                    if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to detach all children?", "Yes", "No"))
                    {
                        Undo.RecordObject(Target, "Detach Children");
                        Target.DetachChildren();
                        HasChanged();
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Unfold Children", "MinibuttonLeft"))
                {
                    Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
                    EditorWindow window = EditorWindow.GetWindow(type);
                    MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
                    int id = Target.gameObject.GetInstanceID();
                    method.Invoke(window, new object[] { id, true });
                }
                if (GUILayout.Button("Fold Children", "MinibuttonRight"))
                {
                    Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
                    EditorWindow window = EditorWindow.GetWindow(type);
                    MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
                    int id = Target.gameObject.GetInstanceID();
                    method.Invoke(window, new object[] { id, false });
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Fold All", "Minibutton"))
                {
                    Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
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
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Position", EditorStyles.miniButtonLeft))
                {
                    GUIUtility.systemCopyBuffer = Target.position.ToCopyString("F4");
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy LocalPosition", EditorStyles.miniButtonRight))
                {
                    GUIUtility.systemCopyBuffer = Target.localPosition.ToCopyString("F4");
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Rotation", EditorStyles.miniButtonLeft))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer = Target.rotation.ToCopyString("F4");
                        Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = ClampAngle(Target.rotation.eulerAngles.x).ToString();
                        string y = ClampAngle(Target.rotation.eulerAngles.y).ToString();
                        string z = ClampAngle(Target.rotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                if (GUILayout.Button("Copy LocalRotation", EditorStyles.miniButtonRight))
                {
                    if (_copyQuaternion)
                    {
                        GUIUtility.systemCopyBuffer = Target.localRotation.ToCopyString("F4");
                        Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    else
                    {
                        string x = ClampAngle(Target.localRotation.eulerAngles.x).ToString();
                        string y = ClampAngle(Target.localRotation.eulerAngles.y).ToString();
                        string z = ClampAngle(Target.localRotation.eulerAngles.z).ToString();

                        GUIUtility.systemCopyBuffer = x + "f," + y + "f," + z + "f";
                        Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Scale", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = Target.localScale.ToCopyString("F4");
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Name", EditorStyles.miniButtonLeft))
                {
                    GUIUtility.systemCopyBuffer = Target.name;
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button("Copy FullName", EditorStyles.miniButtonRight))
                {
                    GUIUtility.systemCopyBuffer = Target.FullName();
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
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