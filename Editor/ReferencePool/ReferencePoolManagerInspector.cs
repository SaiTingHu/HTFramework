using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(ReferencePoolManager))]
    public sealed class ReferencePoolManagerInspector : HTFEditor<ReferencePoolManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Reference pool manager, it manages all reference pools and can register new reference pools!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Limit:");
            IntField(Target.Limit, out Target.Limit);
            GUILayout.EndHorizontal();
        }
    }
}
