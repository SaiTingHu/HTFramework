using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ECS_Entity))]
    internal sealed class ECS_EntityInspector : HTFEditor<ECS_Entity>
    {
        private HTFAction _generateID;

        protected override bool IsEnableRuntimeData => false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _generateID = GenerateID;
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Open In Inspector", EditorGlobalTools.Styles.ButtonLeft))
            {
                ECS_Inspector inspector = EditorWindow.GetWindow<ECS_Inspector>();
                inspector.titleContent.text = "ECS Inspector";
                inspector.Entity = Target;
                inspector.Show();
            }
            Button(_generateID, "Generate ID", EditorGlobalTools.Styles.ButtonRight);
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Show Component", EditorGlobalTools.Styles.ButtonLeft))
            {
                ECS_Component[] components = Target.GetComponents<ECS_Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].hideFlags = HideFlags.None;
                }
                HasChanged();
            }
            if (GUILayout.Button("Hide Component", EditorGlobalTools.Styles.ButtonRight))
            {
                ECS_Component[] components = Target.GetComponents<ECS_Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].hideFlags = HideFlags.HideInInspector;
                }
                HasChanged();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            PropertyField("_name", "Name");
            PropertyField("_id", "ID");
        }
        private void GenerateID()
        {
            if (Target.ID == "")
            {
                Target.GenerateID();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to regenerate the ID？", "Yes", "No"))
                {
                    Target.GenerateID();
                }
            }
        }
    }
}