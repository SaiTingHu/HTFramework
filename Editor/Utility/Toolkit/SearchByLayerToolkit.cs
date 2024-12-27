using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 通过 Layer 搜索物体的工具
    /// </summary>
    internal sealed class SearchByLayerToolkit : HTFEditorWindow
    {
        private string _layer;
        private List<GameObject> _targets = new List<GameObject>();
        private Vector2 _scroll;

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Layer");
            if (GUILayout.Button(_layer, EditorStyles.popup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < InternalEditorUtility.layers.Length; i++)
                {
                    string layer = InternalEditorUtility.layers[i];
                    gm.AddItem(new GUIContent(layer), _layer == layer, () =>
                    {
                        _layer = layer;
                    });
                }
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Missing Layer"), _layer == "Missing Layer", () =>
                {
                    _layer = "Missing Layer";
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = !string.IsNullOrEmpty(_layer);
            if (GUILayout.Button("Search"))
            {
                _targets.Clear();
                int layer = LayerMask.NameToLayer(_layer);
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    CheckTarget(prefabStage.prefabContentsRoot, layer);
                }
                else
                {
                    List<GameObject> roots = new List<GameObject>();
                    GlobalTools.GetRootGameObjectsInAllScene(roots);
                    for (int i = 0; i < roots.Count; i++)
                    {
                        CheckTarget(roots[i], layer);
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
        }

        private void CheckTarget(GameObject target, int layer)
        {
            if (_layer == "Missing Layer")
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(target.layer))) _targets.Add(target);
            }
            else
            {
                if (target.layer == layer) _targets.Add(target);
            }

            for (int i = 0; i < target.transform.childCount; i++)
            {
                CheckTarget(target.transform.GetChild(i).gameObject, layer);
            }
        }
    }
}