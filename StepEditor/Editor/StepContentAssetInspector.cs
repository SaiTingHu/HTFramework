using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepContentAsset))]
    public sealed class StepContentAssetInspector : Editor
    {
        private StepContentAsset _target;
        private Vector2 _scroll;

        private void OnEnable()
        {
            _target = target as StepContentAsset;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Step Number: " + _target.Content.Count);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Data To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, _target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < _target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _target.Content.Count, (float)i / _target.Content.Count);
                        if (_target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + _target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, (i + 1) + "、" + _target.Content[i].Name + "\r\n" + _target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Name To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, _target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < _target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _target.Content.Count, (float)i / _target.Content.Count);
                        if (_target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + _target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, (i + 1) + "、" + _target.Content[i].Name + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Prompt To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, _target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < _target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + _target.Content.Count, (float)i / _target.Content.Count);
                        File.AppendAllText(path, (i + 1) + "、" + _target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit Step Content"))
            {
                StepEditorWindow.ShowWindow(_target);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < _target.Content.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + _target.Content[i].Name);
                GUILayout.FlexibleSpace();
                GUILayout.Label("[" + _target.Content[i].Trigger + "]");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}
