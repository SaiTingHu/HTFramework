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

        private PagePainter _pagePainter;
        private Editor _originalEditor;
        private MethodInfo _originalOnSceneGUI;
        private MethodInfo _originalOnHeaderGUI;

        protected override bool IsEnableRuntimeData => false;
        protected override bool IsWideMode => false;

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
        
        private void OnSceneGUI()
        {
            if (_originalEditor != null && _originalOnSceneGUI != null)
            {
                _originalOnSceneGUI.Invoke(_originalEditor, null);
            }
        }
        private void OnDestroy()
        {
            _originalOnSceneGUI = null;
            _originalOnHeaderGUI = null;
            if (_originalEditor != null)
            {
                DestroyImmediate(_originalEditor);
                _originalEditor = null;
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

            _pagePainter = new PagePainter(this);
            _pagePainter.AddPage("Property", EditorGUIUtility.IconContent("ToolHandleLocal").image, PropertyGUI);
            _pagePainter.AddPage("Hierarchy", EditorGUIUtility.IconContent("ToolHandlePivot").image, HierarchyGUI);
            _pagePainter.AddPage("Copy", EditorGUIUtility.IconContent("ToolHandleCenter").image, CopyGUI);
            
            Type rectTransformEditor = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.RectTransformEditor");
            if (rectTransformEditor != null && targets != null && targets.Length > 0)
            {
                _originalEditor = CreateEditor(targets, rectTransformEditor);
                _originalOnSceneGUI = rectTransformEditor.GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.NonPublic);
                _originalOnHeaderGUI = rectTransformEditor.GetMethod("OnHeaderGUI", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }
        protected override void OnDefaultDisable()
        {
            base.OnDefaultDisable();

            if (_originalEditor != null)
            {
                DestroyImmediate(_originalEditor);
                _originalEditor = null;
            }
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.Space(5);

            _pagePainter.Painting();
        }
        private void PropertyGUI()
        {
            _originalEditor.OnInspectorGUI();
        }
        private void HierarchyGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root", GUILayout.Width(LabelWidth));
            EditorGUILayout.ObjectField(Target.root, typeof(Transform), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parent", GUILayout.Width(LabelWidth));
            GUI.color = Target.parent ? Color.white : Color.gray;
            Transform parent = EditorGUILayout.ObjectField(Target.parent, typeof(Transform), true) as Transform;
            if (parent != Target.parent)
            {
                Undo.RecordObject(Target, "Change Parent " + Target.name);
                Target.SetParent(parent);
                HasChanged();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Child Count", GUILayout.Width(LabelWidth));
            EditorGUILayout.LabelField(Target.childCount.ToString());
            GUILayout.FlexibleSpace();
            GUI.enabled = Target.childCount > 0;
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Detach"))
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
            if (GUILayout.Button("Create Empty Parent"))
            {
                CreateEmptyParent();
            }
            GUILayout.EndHorizontal();
            
            GUI.backgroundColor = Color.white;
        }
        private void CopyGUI()
        {
            GUI.backgroundColor = Color.yellow;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy Position", EditorGlobalTools.Styles.ButtonLeft))
            {
                GUIUtility.systemCopyBuffer = Target.position.ToCopyString("F4");
            }
            if (GUILayout.Button("Copy anchoredPosition", EditorGlobalTools.Styles.ButtonRight))
            {
                GUIUtility.systemCopyBuffer = Target.anchoredPosition.ToCopyString("F2");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy Rotation", EditorGlobalTools.Styles.ButtonLeft))
            {
                if (_copyQuaternion)
                {
                    GUIUtility.systemCopyBuffer = Target.rotation.ToCopyString("F4");
                }
                else
                {
                    float x = ClampAngle(Target.rotation.eulerAngles.x);
                    float y = ClampAngle(Target.rotation.eulerAngles.y);
                    float z = ClampAngle(Target.rotation.eulerAngles.z);
                    Vector3 angle = new Vector3(x, y, z);
                    GUIUtility.systemCopyBuffer = angle.ToCopyString("F1");
                }
            }
            if (GUILayout.Button("Copy LocalRotation", EditorGlobalTools.Styles.ButtonRight))
            {
                if (_copyQuaternion)
                {
                    GUIUtility.systemCopyBuffer = Target.localRotation.ToCopyString("F4");
                }
                else
                {
                    float x = ClampAngle(Target.localRotation.eulerAngles.x);
                    float y = ClampAngle(Target.localRotation.eulerAngles.y);
                    float z = ClampAngle(Target.localRotation.eulerAngles.z);
                    Vector3 angle = new Vector3(x, y, z);
                    GUIUtility.systemCopyBuffer = angle.ToCopyString("F1");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy Scale"))
            {
                GUIUtility.systemCopyBuffer = Target.localScale.ToCopyString("F4");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy SizeDelta"))
            {
                GUIUtility.systemCopyBuffer = Target.sizeDelta.ToCopyString("F2");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy Name", EditorGlobalTools.Styles.ButtonLeft))
            {
                GUIUtility.systemCopyBuffer = Target.name;
            }
            if (GUILayout.Button("Copy FullName", EditorGlobalTools.Styles.ButtonRight))
            {
                GUIUtility.systemCopyBuffer = Target.FullName();
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.green;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy To C# Public Field"))
            {
                GUIUtility.systemCopyBuffer = ToCSPublicField();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy To C# Private Field"))
            {
                GUIUtility.systemCopyBuffer = ToCSPrivateField();
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;

            GUILayout.BeginHorizontal();
            _copyQuaternion = GUILayout.Toggle(_copyQuaternion, "Copy Quaternion");
            GUILayout.EndHorizontal();
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
        private float ClampAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            else if (angle < -180) angle += 360;

            return angle;
        }
        private string ToCSPublicField()
        {
            string fieldName = Target.name.Trim().Replace(" ", "");
            string field = $"[Label(\"{Target.name}\")] public GameObject {fieldName};";
            return field;
        }
        private string ToCSPrivateField()
        {
            string fieldName = Target.name.Trim().Replace(" ", "");
            char[] fieldNames = fieldName.ToCharArray();
            fieldNames[0] = char.ToLower(fieldNames[0]);
            bool copyInjectPath = EditorPrefs.GetBool(EditorPrefsTable.Transform_CopyInjectPath, false);
            string field = null;
            if (copyInjectPath) field = $"[InjectPath(\"{Target.FullName()}\")] private GameObject _{new string(fieldNames)};";
            else field = $"private GameObject _{new string(fieldNames)};";
            return field;
        }
    }
}