using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ReferencePoolManager))]
    public sealed class ReferencePoolManagerInspector : ModuleEditor
    {
        private ReferencePoolManager _target;

        protected override void OnEnable()
        {
            _target = target as ReferencePoolManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Reference pool manager, it manages all reference pools and can register new reference pools!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Limit:");
            IntField(_target.Limit, out _target.Limit);
            GUILayout.EndHorizontal();
        }
    }
}
