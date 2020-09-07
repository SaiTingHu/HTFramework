using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ECSManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/106619485")]
    internal sealed class ECSManagerInspector : InternalModuleInspector<ECSManager>
    {
        private IECSHelper _ECSHelper;
        private Dictionary<string, bool> _systemFoldouts;

        protected override string Intro
        {
            get
            {
                return "ECS Manager, ECS based development mode!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IECSHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _ECSHelper = _helper as IECSHelper;
            _systemFoldouts = new Dictionary<string, bool>();

            foreach (var system in _ECSHelper.Systems)
            {
                _systemFoldouts.Add(system.Value.Name, false);
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            foreach (var system in _ECSHelper.Systems)
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
                _ECSHelper.SetDirty();
            }
            GUILayout.EndHorizontal();
        }
    }
}