using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(RectTransform))]
    internal sealed class RectTransformInspector : HTFEditor<RectTransform>
    {
        private static bool _copyQuaternion = false;
        private bool _showProperty = true;
        private bool _showHierarchy = false;
        private bool _showCopy = false;
        private Transform _parent;
        private Editor _originalEditor;
        private MethodInfo _originalOnSceneGUI;
        private MethodInfo _originalOnHeaderGUI;

        protected override bool IsEnableRuntimeData => false;

        public override void DrawPreview(Rect previewArea)
        {
            _originalEditor.DrawPreview(previewArea);
        }
        public override string GetInfoString()
        {
            return _originalEditor.GetInfoString();
        }
        public override GUIContent GetPreviewTitle()
        {
            return _originalEditor.GetPreviewTitle();
        }
        public override bool HasPreviewGUI()
        {
            return _originalEditor.HasPreviewGUI();
        }
        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            _originalEditor.OnInteractivePreviewGUI(r, background);
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            _originalEditor.OnPreviewGUI(r, background);
        }
        public override void OnPreviewSettings()
        {
            _originalEditor.OnPreviewSettings();
        }
        public override void ReloadPreviewInstances()
        {
            _originalEditor.ReloadPreviewInstances();
        }
        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            return _originalEditor.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        public override bool RequiresConstantRepaint()
        {
            return _originalEditor.RequiresConstantRepaint();
        }
        public override bool UseDefaultMargins()
        {
            return _originalEditor.UseDefaultMargins();
        }

        private void OnDisable()
        {
            if (_originalEditor != null)
            {
                DestroyImmediate(_originalEditor);
            }
        }
        private void OnSceneGUI()
        {
            if (_originalEditor != null && _originalOnSceneGUI != null)
            {
                _originalOnSceneGUI.Invoke(_originalEditor, null);
            }
        }
        protected override void OnHeaderGUI()
        {
            if (_originalEditor != null && _originalOnHeaderGUI != null)
            {
                _originalOnHeaderGUI.Invoke(_originalEditor, null);
            }
        }
        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _showProperty = EditorPrefs.GetBool(EditorPrefsTable.RectTransform_Property, true);
            _showHierarchy = EditorPrefs.GetBool(EditorPrefsTable.RectTransform_Hierarchy, false);
            _showCopy = EditorPrefs.GetBool(EditorPrefsTable.RectTransform_Copy, false);

            Type rectTransformEditor = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.RectTransformEditor");
            if (rectTransformEditor != null && targets != null && targets.Length > 0)
            {
                _originalEditor = CreateEditor(targets, rectTransformEditor);
                _originalOnSceneGUI = rectTransformEditor.GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.NonPublic);
                _originalOnHeaderGUI = rectTransformEditor.GetMethod("OnHeaderGUI", BindingFlags.Instance | BindingFlags.NonPublic);
            }
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
                EditorPrefs.SetBool(EditorPrefsTable.RectTransform_Property, _showProperty);
            }
            GUILayout.EndHorizontal();

            if (_showProperty)
            {
                GUILayout.BeginVertical();
                _originalEditor.OnInspectorGUI();
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
                EditorPrefs.SetBool(EditorPrefsTable.RectTransform_Hierarchy, _showHierarchy);
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
                if (GUILayout.Button("Detach", EditorStyles.miniButton))
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
                EditorPrefs.SetBool(EditorPrefsTable.RectTransform_Copy, _showCopy);
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
                if (GUILayout.Button("Copy anchoredPosition", EditorStyles.miniButtonRight))
                {
                    GUIUtility.systemCopyBuffer = Target.anchoredPosition.ToCopyString("F2");
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
                if (GUILayout.Button("Copy SizeDelta", EditorStyles.miniButton))
                {
                    GUIUtility.systemCopyBuffer = Target.sizeDelta.ToCopyString("F2");
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
        private void CreateEmptyParent()
        {
            GameObject parent = new GameObject("EmptyParent");
            RectTransform rectTransform = parent.AddComponent<RectTransform>();
            rectTransform.SetParent(Target.parent);
            rectTransform.localPosition = Target.localPosition;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.SetSiblingIndex(Target.GetSiblingIndex());
            Target.SetParent(rectTransform);
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
    }
}