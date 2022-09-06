﻿using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DataSetManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89395574")]
    internal sealed class DataSetManagerInspector : InternalModuleInspector<DataSetManager, IDataSetHelper>
    {
        protected override string Intro => "DataSet Manager, manage of all dataSet, you can create, modify, delete any dataSets!";

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper.DataSets.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var item in _helper.DataSets)
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