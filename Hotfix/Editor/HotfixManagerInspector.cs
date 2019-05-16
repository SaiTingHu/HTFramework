using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(HotfixManager))]
    public sealed class HotfixManagerInspector : Editor
    {
        private HotfixManager _target;

        private void OnEnable()
        {
            _target = target as HotfixManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Hotfix manager, the hot update in this game!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.Toggle(_target.IsEnableHotfix, out _target.IsEnableHotfix, "Is Enable Hotfix");
            GUILayout.EndHorizontal();
        }
    }
}
