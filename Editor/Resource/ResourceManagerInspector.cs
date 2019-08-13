using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    public sealed class ResourceManagerInspector : ModuleEditor
    {
        private ResourceManager _target;

        protected override void OnEnable()
        {
            _target = target as ResourceManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Resource Manager, Manage all resource loading and unloading!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(_target.Mode, out _target.Mode, "Load Mode");
            GUILayout.EndHorizontal();

            if (_target.Mode == ResourceLoadMode.Resource)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Resources Folder View", "LargeButton"))
                {
                    ResourcesFolderViewWindow window = EditorWindow.GetWindow<ResourcesFolderViewWindow>();
                    window.titleContent.text = "Resources Folder View";
                    window.position = new Rect(200, 200, 400, 400);
                    window.Init();
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
            else if (_target.Mode == ResourceLoadMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                Toggle(_target.IsEditorMode, out _target.IsEditorMode, "Editor Mode");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(_target.IsCacheAssetBundle, out _target.IsCacheAssetBundle, "Cache AssetBundle");
                GUILayout.EndHorizontal();
            }
        }
    }
}
