using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepContentAsset))]
    public sealed class StepContentAssetInspector : HTFEditor<StepContentAsset>
    {
        private Vector2 _scroll = Vector2.zero;
        private EditorWindow _inspectorWindow;

        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            
            if (GUI.Button(new Rect(_inspectorWindow.position.width - 45, 25, 40, 16), "Open"))
            {
                StepEditorWindow.ShowWindow(Target);
            }
        }

        protected override bool ShouldHideOpenButton()
        {
            return true;
        }

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _inspectorWindow = EditorWindow.GetWindow(EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.InspectorWindow"));
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Step Number: " + Target.Content.Count);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Data To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        if (Target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + Target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, (i + 1) + "、" + Target.Content[i].Name + "\r\n" + Target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Name To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        if (Target.Content[i].Ancillary != "")
                        {
                            File.AppendAllText(path, "【" + Target.Content[i].Ancillary + "】\r\n");
                        }
                        File.AppendAllText(path, Target.Content[i].Name + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Step Prompt To .txt"))
            {
                string path = EditorUtility.SaveFilePanel("保存数据文件", Application.dataPath, Target.name, "txt");
                if (path != "")
                {
                    for (int i = 0; i < Target.Content.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Export......", i + "/" + Target.Content.Count, (float)i / Target.Content.Count);
                        File.AppendAllText(path, Target.Content[i].Prompt + "\r\n");
                    }
                    EditorUtility.ClearProgressBar();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Edit Step Content"))
            {
                StepEditorWindow.ShowWindow(Target);
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < Target.Content.Count; i++)
            {
                if (Target.Content[i].Ancillary != "")
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("【" + Target.Content[i].Ancillary + "】");
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(i + "." + Target.Content[i].Name);
                GUILayout.FlexibleSpace();
                GUILayout.Label("[" + Target.Content[i].Trigger + "]");
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}