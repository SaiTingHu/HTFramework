using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    public sealed class ResourceManagerInspector : Editor
    {
        private ResourceManager _target;

        private void OnEnable()
        {
            _target = target as ResourceManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Resource Manager, Manage all resource loading and unloading!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.Mode = (ResourceMode)EditorGUILayout.EnumPopup("Load Mode", _target.Mode);
            GUILayout.EndHorizontal();

            if (_target.Mode == ResourceMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                _target.IsCacheAssetBundle = EditorGUILayout.Toggle("Cache AssetBundle", _target.IsCacheAssetBundle);
                GUILayout.EndHorizontal();
            }

            if (GUI.enabled)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
