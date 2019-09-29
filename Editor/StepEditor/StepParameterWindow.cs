using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class StepParameterWindow : EditorWindow
    {
        public static void ShowWindow(StepEditorWindow stepEditorWindow, StepContentAsset contentAsset, StepContent content)
        {
            StepParameterWindow window = GetWindow<StepParameterWindow>();
            window.titleContent.text = "Parameters";
            window._stepEditorWindow = stepEditorWindow;
            window._contentAsset = contentAsset;
            window._content = content;
            window.minSize = new Vector2(300, 300);
            window.maxSize = new Vector2(300, 900);
            window.Show();
        }

        private StepEditorWindow _stepEditorWindow;
        private StepContentAsset _contentAsset;
        private StepContent _content;
        private Vector2 _scroll;
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal("Toolbar");
            if (GUILayout.Button(_content.Name, "Toolbarpopup"))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _contentAsset.Content.Count; i++)
                {
                    StepContent stepContent = _contentAsset.Content[i];
                    gm.AddItem(new GUIContent(i + "." + stepContent.Name), stepContent == _content, () =>
                    {
                        _content = stepContent;
                    });
                }
                gm.ShowAsContext();
            }
            if (GUILayout.Button(_content.Helper, "Toolbarbutton"))
            {
                if (_content.Helper != "<None>")
                {
                    _stepEditorWindow.OpenHelperScript(_content.Helper);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _content.Parameters.Count; i++)
            {
                StepParameter stepParameter = _content.Parameters[i];

                GUILayout.BeginVertical("Helpbox");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:", GUILayout.Width(40));
                stepParameter.Type = (StepParameter.ParameterType)EditorGUILayout.EnumPopup(stepParameter.Type, GUILayout.Width(100));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("▲", "MiniButtonleft", GUILayout.Width(20)))
                {
                    if (i > 0)
                    {
                        _content.Parameters.Remove(stepParameter);
                        _content.Parameters.Insert(i - 1, stepParameter);
                        continue;
                    }
                }
                if (GUILayout.Button("▼", "MiniButtonmid", GUILayout.Width(20)))
                {
                    if (i < _content.Parameters.Count - 1)
                    {
                        _content.Parameters.Remove(stepParameter);
                        _content.Parameters.Insert(i + 1, stepParameter);
                        continue;
                    }
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Delete", "Minibuttonright"))
                {
                    _content.Parameters.RemoveAt(i);
                    continue;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.Width(40));
                stepParameter.Name = EditorGUILayout.TextField(stepParameter.Name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Value:", GUILayout.Width(40));
                switch (_content.Parameters[i].Type)
                {
                    case StepParameter.ParameterType.String:
                        stepParameter.StringValue = EditorGUILayout.TextField(stepParameter.StringValue);
                        break;
                    case StepParameter.ParameterType.Integer:
                        stepParameter.IntegerValue = EditorGUILayout.IntField(stepParameter.IntegerValue);
                        break;
                    case StepParameter.ParameterType.Float:
                        stepParameter.FloatValue = EditorGUILayout.FloatField(stepParameter.FloatValue);
                        break;
                    case StepParameter.ParameterType.Boolean:
                        stepParameter.BooleanValue = EditorGUILayout.Toggle(stepParameter.BooleanValue);
                        break;
                    case StepParameter.ParameterType.Vector2:
                        stepParameter.Vector2Value = EditorGUILayout.Vector2Field("", stepParameter.Vector2Value);
                        break;
                    case StepParameter.ParameterType.Vector3:
                        stepParameter.Vector3Value = EditorGUILayout.Vector3Field("", stepParameter.Vector3Value);
                        break;
                    case StepParameter.ParameterType.Color:
                        stepParameter.ColorValue = EditorGUILayout.ColorField(stepParameter.ColorValue);
                        break;
                    case StepParameter.ParameterType.GameObject:
                        #region 步骤目标物体丢失，根据目标GUID重新搜寻
                        if (!stepParameter.GameObjectValue)
                        {
                            if (stepParameter.GameObjectGUID != "<None>")
                            {
                                stepParameter.GameObjectValue = GameObject.Find(stepParameter.GameObjectPath);
                                if (!stepParameter.GameObjectValue)
                                {
                                    StepTarget[] targets = FindObjectsOfType<StepTarget>();
                                    foreach (StepTarget target in targets)
                                    {
                                        if (target.GUID == stepParameter.GameObjectGUID)
                                        {
                                            stepParameter.GameObjectValue = target.gameObject;
                                            stepParameter.GameObjectPath = target.transform.FullName();
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    StepTarget target = stepParameter.GameObjectValue.GetComponent<StepTarget>();
                                    if (!target)
                                    {
                                        target = stepParameter.GameObjectValue.AddComponent<StepTarget>();
                                        target.GUID = stepParameter.GameObjectGUID;
                                    }
                                }
                            }
                        }
                        #endregion

                        GUI.color = stepParameter.GameObjectValue ? Color.white : Color.gray;
                        GameObject objValue = EditorGUILayout.ObjectField(stepParameter.GameObjectValue, typeof(GameObject), true) as GameObject;
                        GUI.color = Color.white;

                        #region 步骤目标改变
                        if (objValue != stepParameter.GameObjectValue)
                        {
                            if (objValue)
                            {
                                StepTarget target = objValue.GetComponent<StepTarget>();
                                if (!target)
                                {
                                    target = objValue.AddComponent<StepTarget>();
                                }
                                if (target.GUID == "<None>")
                                {
                                    target.GUID = Guid.NewGuid().ToString();
                                }
                                stepParameter.GameObjectValue = objValue;
                                stepParameter.GameObjectGUID = target.GUID;
                                stepParameter.GameObjectPath = objValue.transform.FullName();
                            }
                        }
                        #endregion
                        break;
                    case StepParameter.ParameterType.Texture:
                        stepParameter.TextureValue = EditorGUILayout.ObjectField(stepParameter.TextureValue, typeof(Texture), false) as Texture;
                        break;
                    case StepParameter.ParameterType.AudioClip:
                        stepParameter.AudioClipValue = EditorGUILayout.ObjectField(stepParameter.AudioClipValue, typeof(AudioClip), false) as AudioClip;
                        break;
                    case StepParameter.ParameterType.Material:
                        stepParameter.MaterialValue = EditorGUILayout.ObjectField(stepParameter.MaterialValue, typeof(Material), false) as Material;
                        break;
                }
                GUILayout.EndHorizontal();

                if (_content.Parameters[i].Type == StepParameter.ParameterType.GameObject)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("GUID:", GUILayout.Width(40));
                    EditorGUILayout.TextField(_content.Parameters[i].GameObjectGUID);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear", "Minibutton", GUILayout.Width(40)))
                    {
                        _content.Parameters[i].GameObjectValue = null;
                        _content.Parameters[i].GameObjectGUID = "<None>";
                        _content.Parameters[i].GameObjectPath = "<None>";
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", "ButtonLeft"))
            {
                _content.Parameters.Add(new StepParameter());
            }
            if (GUILayout.Button("Clear", "ButtonRight"))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure delete all parameter？", "Yes", "No"))
                {
                    _content.Parameters.Clear();
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
            {
                GUI.FocusControl(null);
                EditorUtility.SetDirty(_contentAsset);
            }
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            if (_stepEditorWindow == null || _contentAsset == null || _content == null)
            {
                Close();
            }
        }
    }
}