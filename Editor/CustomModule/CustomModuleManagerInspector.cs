using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(CustomModuleManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/103390089")]
    internal sealed class CustomModuleManagerInspector : HTFEditor<CustomModuleManager>
    {
        private Dictionary<string, CustomModuleBase> _customModules;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _customModules = Target.GetType().GetField("_customModules", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, CustomModuleBase>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Custom Module Manager, Manager of all custom modules!", MessageType.Info);
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("CustomModules: " + _customModules.Count);
            GUILayout.EndHorizontal();

            foreach (var item in _customModules)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(item.Key + "[" + item.Value.GetType().FullName + "]");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                if (item.Value.IsRunning)
                {
                    GUILayout.Label("[Running]");
                    if (GUILayout.Button("Stop", EditorStyles.miniButton, GUILayout.Width(40)))
                    {
                        item.Value.IsRunning = false;
                    }
                }
                else
                {
                    GUILayout.Label("[Stopped]");
                    if (GUILayout.Button("Run", EditorStyles.miniButton, GUILayout.Width(40)))
                    {
                        item.Value.IsRunning = true;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}