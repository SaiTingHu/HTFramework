using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class ProjectBatch : HTFEditorWindow
    {
        private DefaultAsset _folder;
        private string _objectTypeFilter = "";
        private Type _objectType;
        private MethodInfo _selectionAddMethod;
        private object[] _parameter = new object[1];

        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }

        private void OnEnable()
        {
            if (_selectionAddMethod == null)
            {
                MethodInfo[] methods = EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.Selection").GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
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

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("批处理[ " + (_folder ? _folder.name : "Root") + " ]文件夹下的[ " + (_objectType != null ? _objectType.FullName : "Object") + " ]对象！", MessageType.Info);
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
            if (GUILayout.Button(_objectType != null ? _objectType.FullName : "<None>", "Minipopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = EditorGlobalTools.GetTypesInAllAssemblies();
                gm.AddItem(new GUIContent("<None>"), _objectType == null, () =>
                {
                    _objectType = null;
                });
                for (int i = 0; i < types.Count; i++)
                {
                    Type type = types[i];
                    if (type.IsSubclassOf(typeof(UnityEngine.Object)) && type.FullName.ToLower().Contains(_objectTypeFilter.ToLower()))
                    {
                        gm.AddItem(new GUIContent(type.FullName), type == _objectType, () =>
                        {
                            _objectType = type;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = _folder && _objectType != null && _selectionAddMethod != null;

            EditorGUILayout.BeginHorizontal();
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
