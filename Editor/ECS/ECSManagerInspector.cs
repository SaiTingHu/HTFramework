using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ECSManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/106619485")]
    internal sealed class ECSManagerInspector : InternalModuleInspector<ECSManager, IECSHelper>
    {
        private Dictionary<string, bool> _systemFoldouts;

        protected override string Intro => "ECS Manager, this is a ECS based development mode!";

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _systemFoldouts = new Dictionary<string, bool>();

            foreach (var system in _helper.Systems)
            {
                _systemFoldouts.Add(system.Value.Name, false);
            }
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            foreach (var system in _helper.Systems)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                _systemFoldouts[system.Value.Name] = EditorGUILayout.Foldout(_systemFoldouts[system.Value.Name], system.Value.Name, true);
                GUILayout.EndHorizontal();

                if (_systemFoldouts[system.Value.Name])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    system.Value.IsEnabled = EditorGUILayout.Toggle("IsEnabled", system.Value.IsEnabled);
                    GUILayout.EndHorizontal();

                    foreach (var entity in system.Value.StarEntities)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.ObjectField(entity.gameObject, typeof(GameObject), true);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60)))
                        {
                            system.Value.StarEntities.Remove(entity);
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Dirty"))
            {
                _helper.SetDirty();
            }
            GUILayout.EndHorizontal();
        }
    }
}