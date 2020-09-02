using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ObjectPoolManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/86610600")]
    internal sealed class ObjectPoolManagerInspector : InternalModuleInspector<ObjectPoolManager>
    {
        private IObjectPoolHelper _objectPoolHelper;

        protected override string Intro
        {
            get
            {
                return "Object pool manager, it manages all object pools and can register new object pools!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IObjectPoolHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _objectPoolHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IObjectPoolHelper;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();
            IntField(Target.Limit, out Target.Limit, "Limit");
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_objectPoolHelper.SpawnPools.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var pool in _objectPoolHelper.SpawnPools)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(pool.Key + ": " + pool.Value.Count);
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