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
            _target.Limit = EditorGUILayout.IntField(_target.Limit);
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
