using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ObjectPoolManager))]
    public sealed class ObjectPoolManagerInspector : Editor
    {
        private ObjectPoolManager _target;

        private void OnEnable()
        {
            _target = target as ObjectPoolManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Object pool manager, it manages all object pools and can register new object pools!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Limit:");
            this.IntField(_target.Limit, out _target.Limit);
            GUILayout.EndHorizontal();
        }
    }
}
