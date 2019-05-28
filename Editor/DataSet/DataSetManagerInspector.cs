using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DataSetManager))]
    public sealed class DataSetManagerInspector : ModuleEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("DataSet Manager, create, modify, delete all data sets!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
