using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ObjectPoolManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86610600")]
    internal sealed class ObjectPoolManagerInspector : InternalModuleInspector<ObjectPoolManager, IObjectPoolHelper>
    {
        protected override string Intro => "Object pool manager, it manages all object pools and can register new object pools!";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(ObjectPoolManager.Limit), "Limit");

            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper == null || _helper.SpawnPools.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
                return;
            }

            foreach (var pool in _helper.SpawnPools)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label($"{pool.Key}: {pool.Value.Count}");
                GUILayout.FlexibleSpace();
                GUI.enabled = pool.Value.Count > 0;
                if (GUILayout.Button("Clear"))
                {
                    pool.Value.Clear();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}