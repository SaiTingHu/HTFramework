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
            ResourceMode mode = (ResourceMode)EditorGUILayout.EnumPopup("Load Mode", _target.Mode);
            if (mode != _target.Mode)
            {
                Undo.RecordObject(target, "Set Load Mode");
                _target.Mode = mode;
                this.HasChanged();
            }
            GUILayout.EndHorizontal();

            if (_target.Mode == ResourceMode.Resource)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Resources Folder View", "Minibutton"))
                {
                    ResourcesFolderViewWindow window = EditorWindow.GetWindow<ResourcesFolderViewWindow>();
                    window.titleContent.text = "Resources Folder View";
                    window.position = new Rect(200, 200, 400, 400);
                    window.Init();
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
            if (_target.Mode == ResourceMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                this.Toggle(_target.IsCacheAssetBundle, out _target.IsCacheAssetBundle, "Cache AssetBundle");
                GUILayout.EndHorizontal();
            }
        }
    }
}
