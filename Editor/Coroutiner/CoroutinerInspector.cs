using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Coroutiner))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/91492838")]
    internal sealed class CoroutinerInspector : HTFEditor<Coroutiner>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Coroutiner, Execution and destruction of unified scheduling Coroutine!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Coroutiner Tracker", EditorGlobalTools.Styles.LargeButton))
            {
                CoroutinerTrackerWindow tracker = EditorWindow.GetWindow<CoroutinerTrackerWindow>();
                tracker.titleContent.text = "Coroutiner Tracker";
                tracker.Init(Target);
                tracker.minSize = new Vector2(400, 400);
                tracker.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
                tracker.Show();
            }
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