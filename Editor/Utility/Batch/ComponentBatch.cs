using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class ComponentBatch : HTFEditorWindow
    {
        private GameObject _root;
        private string _componentTypeFilter = "";
        private Type _componentType;
        private bool _includeInactive = true;
        private MethodInfo _selectionAddMethod;
        private object[] _parameter = new object[1];

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/102971712";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_selectionAddMethod == null)
            {
                MethodInfo[] methods = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.Selection").GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == "Add")
                    {
                        ParameterInfo[] parameters = methods[i].GetParameters();
                        if (parameters != null && parameters.Length == 1 && parameters[0].ParameterType.Name == "Object")
                        {
                            _selectionAddMethod = methods[i];
                            break;
                        }
                    }
                }
            }
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            EditorGUILayout.BeginHorizontal();
            string prompt = string.Format("批处理[ {0} ]下的[ {1} ]组件！", _root ? _root.name : "Root", _componentType != null ? _componentType.FullName : "Component");
            EditorGUILayout.HelpBox(prompt, MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root：", GUILayout.Width(120));
            _root = EditorGUILayout.ObjectField(_root, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Component Filter：", GUILayout.Width(120));
            _componentTypeFilter = EditorGUILayout.TextField(_componentTypeFilter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Component：", GUILayout.Width(120));
            if (GUILayout.Button(_componentType != null ? _componentType.FullName : "<None>", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(Component)) && type.FullName.ToLower().Contains(_componentTypeFilter.ToLower());
                });
                gm.AddItem(new GUIContent("<None>"), _componentType == null, () =>
                {
                    _componentType = null;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    Type type = types[i];
                    gm.AddItem(new GUIContent(type.FullName.Replace(".", "/")), type == _componentType, () =>
                    {
                        _componentType = type;
                    });
                }
                gm.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Include Inactive：", GUILayout.Width(120));
            _includeInactive = EditorGUILayout.Toggle(_includeInactive);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = _root && _componentType != null && _selectionAddMethod != null;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Collect"))
            {
                Selection.activeGameObject = null;

                Component[] components = _root.transform.GetComponentsInChildren(_componentType, _includeInactive);
                for (int i = 0; i < components.Length; i++)
                {
                    CollectComponent(components[i]);
                }
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private void CollectComponent(Component component)
        {
            _parameter[0] = component;
            _selectionAddMethod.Invoke(null, _parameter);
        }
    }
}