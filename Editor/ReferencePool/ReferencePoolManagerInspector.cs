using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ReferencePoolManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/87191712")]
    public sealed class ReferencePoolManagerInspector : HTFEditor<ReferencePoolManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Reference pool manager, it manages all reference pools and can register new reference pools!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.Limit, out Target.Limit, "Limit");
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Target.SpawnPools.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var pool in Target.SpawnPools)
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
