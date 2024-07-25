using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(CustomModuleManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/103390089")]
    internal sealed class CustomModuleManagerInspector : InternalModuleInspector<CustomModuleManager, ICustomModuleHelper>
    {
        protected override string Intro => "CustomModule Manager, manager of all custom modules, such as starting or stopping a custom module!";

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label($"CustomModules: {_helper.GetAllCustomModule().Count}");
            GUILayout.EndHorizontal();

            foreach (var item in _helper.GetAllCustomModule())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label($"{item.Name}[{item.GetType().FullName}]");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                if (item.IsRunning)
                {
                    GUILayout.Label("[Running]");
                    if (GUILayout.Button("Stop", GUILayout.Width(40)))
                    {
                        item.IsRunning = false;
                    }
                }
                else
                {
                    GUILayout.Label("[Stopped]");
                    if (GUILayout.Button("Run", GUILayout.Width(40)))
                    {
                        item.IsRunning = true;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}