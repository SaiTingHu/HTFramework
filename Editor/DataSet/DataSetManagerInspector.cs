using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DataSetManager))]
    public sealed class DataSetManagerInspector : HTFEditor<DataSetManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("DataSet Manager, create, modify, delete all data sets!", MessageType.Info);
            GUILayout.EndHorizontal();
        }
    }
}
