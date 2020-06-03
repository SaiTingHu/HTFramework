using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ECS_Entity))]
    internal sealed class ECS_EntityInspector : HTFEditor<ECS_Entity>
    {
        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            Button(GenerateID, "Generate ID");
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Open In Inspector"))
            {
                ECS_Inspector inspector = EditorWindow.GetWindow<ECS_Inspector>();
                inspector.titleContent.text = "ECS Inspector";
                inspector.Entity = Target;
                inspector.Show();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            PropertyField("_name", "Name");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            PropertyField("_id", "ID");
            GUILayout.EndHorizontal();
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