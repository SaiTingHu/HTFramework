using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    public sealed class UIManagerInspector : ModuleEditor
    {
        private UIManager _target;

        protected override void OnEnable()
        {
            _target = target as UIManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("UI Manager, Control all UILogic Entity!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
