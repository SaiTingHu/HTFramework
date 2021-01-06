using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 扩展的检视器
    /// </summary>
    internal sealed class ExtendedInspectorWindow : HTFEditorWindow
    {
        [MenuItem("CONTEXT/Component/Browse In Extended Inspector")]
        public static void BrowseInExtendedInspector(MenuCommand cmd)
        {
            if (cmd.context is Component)
            {
                ExtendedInspectorWindow window = GetWindow<ExtendedInspectorWindow>();
                window.titleContent.image = EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image;
                window.titleContent.text = "Extended Inspector";
                window.AppendComponent(cmd.context.Cast<Component>());
            }
        }

        private List<Component> _components = new List<Component>();
        private List<Editor> _editors = new List<Editor>();
        private List<bool> _foldouts = new List<bool>();
        private Vector2 _scroll;
        private GUIContent _prefabGC;
        private GUIContent _removeGC;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/102971712";

        protected override void OnEnable()
        {
            base.OnEnable();

            _editors.Clear();
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] == null)
                {
                    _components.RemoveAt(i);
                    _foldouts.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    _editors.Add(Editor.CreateEditor(_components[i]));
                }
            }

            _prefabGC = new GUIContent();
            _prefabGC.image = EditorGUIUtility.IconContent("Prefab Icon").image;
            _removeGC = new GUIContent();
            _removeGC.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _removeGC.tooltip = "Remove";
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.Label("Targets " + _components.Count.ToString(), EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fold All", EditorStyles.toolbarButton))
            {
                for (int i = 0; i < _foldouts.Count; i++)
                {
                    _foldouts[i] = false;
                }
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] == null)
                {
                    _components.RemoveAt(i);
                    _editors.RemoveAt(i);
                    _foldouts.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    _foldouts[i] = EditorGUILayout.InspectorTitlebar(_foldouts[i], _components[i]);
                    if (GUILayout.Button(_removeGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20)))
                    {
                        _components.RemoveAt(i);
                        _editors.RemoveAt(i);
                        _foldouts.RemoveAt(i);
                        i -= 1;
                        break;
                    }
                    GUILayout.EndHorizontal();

                    if (_foldouts[i])
                    {
                        GUI.color = Color.clear;
                        GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
                        GUI.color = Color.white;

                        GUI.enabled = false;

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField("GameObject", _components[i].gameObject, typeof(GameObject), true);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField("Component", _components[i], _components[i].GetType(), true);
                        GUILayout.EndHorizontal();

                        GUI.enabled = true;

                        _editors[i].OnInspectorGUI();

                        GUILayout.EndVertical();
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void AppendComponent(Component component)
        {
            if (!_components.Contains(component))
            {
                _components.Add(component);
                _editors.Add(Editor.CreateEditor(component));
                _foldouts.Add(true);
            }
        }
    }
}