using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DataSetManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89395574")]
    internal sealed class DataSetManagerInspector : InternalModuleInspector<DataSetManager>
    {
        private IDataSetHelper _dataSetHelper;

        protected override string Intro
        {
            get
            {
                return "DataSet Manager, manage of all dataSet, you can create, modify, delete any dataSets!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IDataSetHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _dataSetHelper = _helper as IDataSetHelper;
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_dataSetHelper.DataSets.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }

            foreach (var item in _dataSetHelper.DataSets)
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