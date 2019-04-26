using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ReferencePoolManager))]
    public sealed class ReferencePoolManagerInspector : Editor
    {
        private ReferencePoolManager _target;

        private void OnEnable()
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
            this.IntField(_target.Limit, out _target.Limit);
            GUILayout.EndHorizontal();
        }
    }
}
