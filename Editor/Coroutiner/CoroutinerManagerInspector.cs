using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(CoroutinerManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/91492838")]
    internal sealed class CoroutinerManagerInspector : InternalModuleInspector<CoroutinerManager, ICoroutinerHelper>
    {
        protected override string Intro => "Coroutiner Manager, a unified scheduler for the coroutines, including execution and destruction, as well as viewing the status of all coroutines!";

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();
            
            GUILayout.BeginHorizontal();
            GUI.enabled = EditorApplication.isPlaying;
            GUI.backgroundColor = Color.green;
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
            GUI.backgroundColor = Color.white;
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