using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Coroutiner))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/91492838")]
    internal sealed class CoroutinerInspector : InternalModuleInspector<Coroutiner>
    {
        protected override string Intro
        {
            get
            {
                return "Coroutiner, a unified scheduler for the coroutines, including execution and destruction, as well as viewing the status of all coroutines!";
            }
        }
        protected override Type HelperInterface
        {
            get
            {
                return typeof(ICoroutinerHelper);
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();
            
            GUILayout.BeginHorizontal();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Coroutiner Tracker", EditorGlobalTools.Styles.LargeButton))
            {
                CoroutinerTrackerWindow tracker = EditorWindow.GetWindow<CoroutinerTrackerWindow>();
                tracker.titleContent.image = EditorGUIUtility.IconContent("NavMeshAgent Icon").image;
                tracker.titleContent.text = "Coroutiner Tracker";
                tracker.Init(Target);
                tracker.minSize = new Vector2(400, 400);
                tracker.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
                tracker.Show();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}