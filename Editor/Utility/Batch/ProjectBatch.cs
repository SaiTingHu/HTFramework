using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class ProjectBatch : HTFEditorWindow
    {
        private DefaultAsset _folder;
        private string _objectTypeFilter = "";
        private Type _objectType;
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
            string prompt = string.Format("批处理[ {0} ]文件夹下的[ {1} ]对象！", _folder ? _folder.name : "Root", _objectType != null ? _objectType.FullName : "Object");
            EditorGUILayout.HelpBox(prompt, MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Folder：", GUILayout.Width(120));
            _folder = EditorGUILayout.ObjectField(_folder, typeof(DefaultAsset), false) as DefaultAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Object Filter：", GUILayout.Width(120));
            _objectTypeFilter = EditorGUILayout.TextField(_objectTypeFilter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Object：", GUILayout.Width(120));
            if (GUILayout.Button(_objectType != null ? _objectType.FullName : "<None>", EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInAllAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(UnityEngine.Object)) && !type.IsSubclassOf(typeof(Component)) && type.FullName.ToLower().Contains(_objectTypeFilter.ToLower());
                });
                gm.AddItem(new GUIContent("<None>"), _objectType == null, () =>
                {
                    _objectType = null;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    Type type = types[i];
                    gm.AddItem(new GUIContent(type.FullName.Replace(".", "/")), type == _objectType, () =>
                    {
                        _objectType = type;
                    });
                }
                gm.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = _folder && _objectType != null && _selectionAddMethod != null;

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Collect"))
            {
                Selection.activeObject = _folder;
                UnityEngine.Object[] objs = Selection.GetFiltered(_objectType, SelectionMode.DeepAssets);
                Selection.activeObject = null;

                for (int i = 0; i < objs.Length; i++)
                {
                    CollectObject(objs[i]);
                }
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private void CollectObject(UnityEngine.Object obj)
        {
            _parameter[0] = obj;
            _selectionAddMethod.Invoke(null, _parameter);
        }
    }
}