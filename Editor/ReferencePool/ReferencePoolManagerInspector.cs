﻿using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ReferencePoolManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/87191712")]
    internal sealed class ReferencePoolManagerInspector : InternalModuleInspector<ReferencePoolManager, IReferencePoolHelper>
    {
        protected override string Intro => "Reference pool manager, it manages all reference pools and can register new reference pools!";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(ReferencePoolManager.Limit), "Limit");

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper.SpawnPools.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var pool in _helper.SpawnPools)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(pool.Key.Name + ": " + pool.Value.Count);
                GUILayout.FlexibleSpace();
                GUI.enabled = pool.Value.Count > 0;
                if (GUILayout.Button("Clear", EditorStyles.miniButton))
                {
                    pool.Value.Clear();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}