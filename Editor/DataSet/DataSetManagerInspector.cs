using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DataSetManager))]
    public sealed class DataSetManagerInspector : HTFEditor<DataSetManager>
    {
        private Dictionary<Type, List<DataSetBase>> _dataSets;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _dataSets = Target.GetType().GetField("_dataSets", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, List<DataSetBase>>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("DataSet Manager, create, modify, delete all data sets!", MessageType.Info);
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            foreach (var item in _dataSets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("DataSet Type: " + item.Key.Name);
                GUILayout.EndHorizontal();

                if (item.Value.Count == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label("Count 0！");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.ObjectField(item.Value[i], typeof(DataSetBase), true);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
