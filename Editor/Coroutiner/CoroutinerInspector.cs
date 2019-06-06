using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Coroutiner))]
    public sealed class CoroutinerInspector : ModuleEditor
    {
        private Coroutiner _target;

        private void OnEnable()
        {
            _target = target as Coroutiner;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Coroutiner, Execution and destruction of unified scheduling Coroutine!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Coroutiner Tracker", "LargeButton"))
            {
                CoroutinerTrackerWindow tracker = EditorWindow.GetWindow<CoroutinerTrackerWindow>();
                tracker.titleContent.text = "Coroutiner Tracker";
                tracker.Init(_target);
                tracker.position = new Rect(200, 200, 1020, 800);
                tracker.Show();
            }
            GUILayout.EndHorizontal();
        }
    }
}
