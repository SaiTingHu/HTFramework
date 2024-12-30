using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 通过 Tag 搜索物体的工具
    /// </summary>
    internal sealed class SearchByTagToolkit : HTFEditorWindow
    {
        private string _tag;
        private List<GameObject> _targets = new List<GameObject>();
        private Vector2 _scroll;

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tag");
            if (GUILayout.Button(_tag, EditorStyles.popup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < InternalEditorUtility.tags.Length; i++)
                {
                    string tag = InternalEditorUtility.tags[i];
                    gm.AddItem(new GUIContent(tag), _tag == tag, () =>
                    {
                        _tag = tag;
                    });
                }
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Undefined"), _tag == "Undefined", () =>
                {
                    _tag = "Undefined";
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = !string.IsNullOrEmpty(_tag);
            if (GUILayout.Button("Search"))
            {
                _targets.Clear();
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    CheckTarget(prefabStage.prefabContentsRoot);
                }
                else
                {
                    List<GameObject> roots = new List<GameObject>();
                    GlobalTools.GetRootGameObjectsInAllScene(roots);
                    for (int i = 0; i < roots.Count; i++)
                    {
                        CheckTarget(roots[i]);
                    }
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(_targets[i], typeof(GameObject), true);
                    if (GUILayout.Button("Select", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = _targets[i];
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUI.enabled = _targets.Count > 0;
            if (GUILayout.Button("Modify Tag Batch"))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < InternalEditorUtility.tags.Length; i++)
                {
                    string tag = InternalEditorUtility.tags[i];
                    gm.AddItem(new GUIContent(tag), false, () =>
                    {
                        ModifyTagBatch(tag);
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void CheckTarget(GameObject target)
        {
            if (_tag == "Undefined")
            {
                if (target.tag == "Undefined") _targets.Add(target);
            }
            else
            {
                if (target.CompareTag(_tag)) _targets.Add(target);
            }

            for (int i = 0; i < target.transform.childCount; i++)
            {
                CheckTarget(target.transform.GetChild(i).gameObject);
            }
        }
        private void ModifyTagBatch(string tag)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].tag = tag;
                EditorUtility.SetDirty(_targets[i]);
            }
        }
    }
}