using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    public sealed class ResourceManagerInspector : HTFEditor<ResourceManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Resource Manager, Manage all resource loading and unloading!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.Mode, out Target.Mode, "Load Mode");
            GUILayout.EndHorizontal();

            if (Target.Mode == ResourceLoadMode.Resource)
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
            else if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                Toggle(Target.IsEditorMode, out Target.IsEditorMode, "Editor Mode");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsCacheAssetBundle, out Target.IsCacheAssetBundle, "Cache AssetBundle");
                GUILayout.EndHorizontal();
            }
        }
    }
}
