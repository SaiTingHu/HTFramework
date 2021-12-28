using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BoxCollider))]
    internal sealed class BoxColliderInspector : HTFEditor<BoxCollider>
    {
        private Editor _originalEditor;
        private MethodInfo _originalOnEnable;
        private bool _isShowBounds;

        private void OnSceneGUI()
        {
            if (_isShowBounds)
            {
                using (new Handles.DrawingScope(Color.red))
                {
                    for (int i = 0; i < Targets.Length; i++)
                    {
                        Handles.DrawWireCube(Targets[i].bounds.center, Targets[i].bounds.size);
                    }
                }
            }
        }
        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            Type boxColliderEditor = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.BoxColliderEditor");
            if (boxColliderEditor != null && targets != null && targets.Length > 0)
            {
                _originalEditor = CreateEditor(targets, boxColliderEditor);
                _originalOnEnable = boxColliderEditor.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public);
                _originalOnEnable.Invoke(_originalEditor, null);
            }
            _isShowBounds = EditorPrefs.GetBool(EditorPrefsTable.Collider_ShowBounds, false);
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            if (_originalEditor != null)
            {
                _originalEditor.OnInspectorGUI();
            }

            GUILayout.BeginHorizontal();
            bool value = EditorGUILayout.Toggle("Show Bounds", _isShowBounds);
            if (_isShowBounds != value)
            {
                _isShowBounds = value;
                EditorPrefs.SetBool(EditorPrefsTable.Collider_ShowBounds, _isShowBounds);
                HasChanged();
            }
            GUILayout.EndHorizontal();
        }
    }
}