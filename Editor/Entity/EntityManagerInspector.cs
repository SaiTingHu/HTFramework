using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(EntityManager))]
    public sealed class EntityManagerInspector : ModuleEditor
    {
        private EntityManager _target;
        
        protected override void OnEnable()
        {
            _target = target as EntityManager;

            base.OnEnable();
        }

        protected override void OnPlayingEnable()
        {
            base.OnPlayingEnable();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Entity Manager, Control all EntityLogic!", MessageType.Info);
            GUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }

        protected override void OnPlayingInspectorGUI()
        {
            base.OnPlayingInspectorGUI();

            GUILayout.BeginVertical("Helpbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Runtime Data", "BoldLabel");
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }
    }
}
