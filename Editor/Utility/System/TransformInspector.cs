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
        private bool _onlyShowLocal = false;
        private bool _showProperty = true;
        private bool _showHierarchy = false;
        private bool _showCopy = false;
        private Transform _parent;
        private GUIContent _copy;
        private GUIContent _paste;

        protected override bool IsEnableRuntimeData => false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _onlyShowLocal = EditorPrefs.GetBool(EditorPrefsTable.Transform_OnlyShowLocal, false);
            _showProperty = EditorPrefs.GetBool(EditorPrefsTable.Transform_Property, true);
            _showHierarchy = EditorPrefs.GetBool(EditorPrefsTable.Transform_Hierarchy, false);
            _showCopy = EditorPrefs.GetBool(EditorPrefsTable.Transform_Copy, false);
            _copy = new GUIContent();
            _copy.text = "C";
            _copy.tooltip = "Copy value";
            _paste = new GUIContent();
            _paste.text = "P";
            _paste.tooltip = "Paste value";
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

                if (!_onlyShowLocal)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("P", GUILayout.Width(20));
                    EditorGUI.BeginChangeCheck();
                    Vector3 pos = EditorGUILayout.Vector3Field("", Target.position);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(Target, "Move " + Target.name);
                        Target.position = pos;
                        HasChanged();
                    }
                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                    {
                        GUIUtility.systemCopyBuffer = Target.position.ToCopyString("F4");
                        Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                    }
                    if (GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                    {
                        if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                        {
                            Undo.RecordObject(Target, "Paste position value");
                            Target.position = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.zero);
                            HasChanged();
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("R", GUILayout.Width(20));
                    EditorGUI.BeginChangeCheck();
                    Vector3 rot = EditorGUILayout.Vector3Field("", Target.rotation.eulerAngles);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(Target, "Rotate " + Target.name);
                        Target.rotation = Quaternion.Euler(rot);
                        HasChanged();
                    }
                    GUI.backgroundColor = Color.yellow;
                    if (GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
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
                    if (GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                    {
                        if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                        {
                            if (_copyQuaternion)
                            {
                                Undo.RecordObject(Target, "Paste rotation value");
                                Target.rotation = GUIUtility.systemCopyBuffer.ToPasteQuaternion(Quaternion.identity);
                                HasChanged();
                            }
                            else
                            {
                                Undo.RecordObject(Target, "Paste rotation value");
                                Target.rotation = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.zero).ToQuaternion();
                                HasChanged();
                            }
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("S", GUILayout.Width(20));
                    GUI.enabled = false;
                    EditorGUILayout.Vector3Field("", Target.lossyScale);
                    GUI.backgroundColor = Color.yellow;
                    GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20));
                    GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20));
                    GUI.backgroundColor = Color.white;
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("LP", GUILayout.Width(20));
                EditorGUI.BeginChangeCheck();
                Vector3 localpos = EditorGUILayout.Vector3Field("", Target.localPosition);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Target, "Move " + Target.name);
                    Target.localPosition = localpos;
                    HasChanged();
                }
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                {
                    GUIUtility.systemCopyBuffer = Target.localPosition.ToCopyString("F4");
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                    {
                        Undo.RecordObject(Target, "Paste localPosition value");
                        Target.localPosition = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.zero);
                        HasChanged();
                    }
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LR", GUILayout.Width(20));
                EditorGUI.BeginChangeCheck();
                Vector3 localrot = EditorGUILayout.Vector3Field("", Target.localRotation.eulerAngles);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Target, "Rotate " + Target.name);
                    Target.localRotation = Quaternion.Euler(localrot);
                    HasChanged();
                }
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
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
                if (GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                    {
                        if (_copyQuaternion)
                        {
                            Undo.RecordObject(Target, "Paste localRotation value");
                            Target.localRotation = GUIUtility.systemCopyBuffer.ToPasteQuaternion(Quaternion.identity);
                            HasChanged();
                        }
                        else
                        {
                            Undo.RecordObject(Target, "Paste localRotation value");
                            Target.localRotation = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.zero).ToQuaternion();
                            HasChanged();
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LS", GUILayout.Width(20));
                EditorGUI.BeginChangeCheck();
                Vector3 localsca = EditorGUILayout.Vector3Field("", Target.localScale);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Target, "Scale " + Target.name);
                    Target.localScale = localsca;
                    HasChanged();
                }
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(_copy, EditorStyles.miniButtonLeft, GUILayout.Width(20)))
                {
                    GUIUtility.systemCopyBuffer = Target.localScale.ToCopyString("F4");
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                if (GUILayout.Button(_paste, EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                    {
                        Undo.RecordObject(Target, "Paste localScale value");
                        Target.localScale = GUIUtility.systemCopyBuffer.ToPasteVector3(Vector3.one);
                        HasChanged();
                    }
                }
                GUI.backgroundColor = Color.white;
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
                GUILayout.Label("Root: ", GUILayout.Width(LabelWidth));
                EditorGUILayout.ObjectField(Target.root, typeof(Transform), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Parent: ", GUILayout.Width(LabelWidth));
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
                GUILayout.Label("Child Count: ", GUILayout.Width(LabelWidth));
                GUILayout.Label(Target.childCount.ToString());
                GUILayout.FlexibleSpace();
                GUI.enabled = Target.childCount > 0;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Detach", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to detach all children?", "Yes", "No"))
                    {
                        Undo.RecordObject(Target, "Detach Children");
                        Target.DetachChildren();
                        HasChanged();
                    }
                }
                GUI.backgroundColor = Color.white;
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.yellow;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Empty Parent", EditorStyles.miniButton))
                {
                    CreateEmptyParent();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Unfold Children", EditorStyles.miniButtonLeft))
                {
                    UnfoldChildren();
                }
                if (GUILayout.Button("Fold Children", EditorStyles.miniButtonRight))
                {
                    FoldChildren();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Fold All", EditorStyles.miniButton))
                {
                    FoldAll();
                }
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;

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

                GUI.backgroundColor = Color.yellow;

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

                GUI.backgroundColor = Color.green;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy To C# Public Field", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = ToCSPublicField();
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy To C# Private Field", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = ToCSPrivateField();
                    Log.Info("已复制：" + GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;

                GUILayout.BeginHorizontal();
                _copyQuaternion = GUILayout.Toggle(_copyQuaternion, "Copy Quaternion");
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            #endregion
        }
        private void CreateEmptyParent()
        {
            GameObject parent = new GameObject("EmptyParent");
            parent.transform.SetParent(Target.parent);
            parent.transform.localPosition = Target.localPosition;
            parent.transform.localRotation = Quaternion.identity;
            parent.transform.localScale = Vector3.one;
            parent.transform.SetSiblingIndex(Target.GetSiblingIndex());
            Target.SetParent(parent.transform);
            Selection.activeGameObject = parent;
            EditorGUIUtility.PingObject(parent);
        }
        private void UnfoldChildren()
        {
            Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
            EditorWindow window = EditorWindow.GetWindow(type);
            MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
            int id = Target.gameObject.GetInstanceID();
            method.Invoke(window, new object[] { id, true });
        }
        private void FoldChildren()
        {
            Type type = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.SceneHierarchyWindow");
            EditorWindow window = EditorWindow.GetWindow(type);
            MethodInfo method = window.GetType().GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);
            int id = Target.gameObject.GetInstanceID();
            method.Invoke(window, new object[] { id, false });
        }
        private void FoldAll()
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
        private int ClampAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            else if (angle < -180) angle += 360;

            return (int)angle;
        }
        private string ToCSPublicField()
        {
            string fieldName = Target.name.Trim().Replace(" ", "");
            string field = string.Format("[Label(\"{0}\")] public GameObject {1};", Target.name, fieldName);
            return field;
        }
        private string ToCSPrivateField()
        {
            string fieldName = Target.name.Trim().Replace(" ", "");
            char[] fieldNames = fieldName.ToCharArray();
            fieldNames[0] = char.ToLower(fieldNames[0]);
            string field = string.Format("[ObjectPath(\"{0}\")] private GameObject _{1};", Target.FullName(), new string(fieldNames));
            return field;
        }
    }
}