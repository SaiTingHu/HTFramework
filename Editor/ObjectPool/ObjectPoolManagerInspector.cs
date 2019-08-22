using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ObjectPoolManager))]
    public sealed class ObjectPoolManagerInspector : HTFEditor<ObjectPoolManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Object pool manager, it manages all object pools and can register new object pools!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Limit:");
            IntField(Target.Limit, out Target.Limit);
            GUILayout.EndHorizontal();
        }
    }
}
