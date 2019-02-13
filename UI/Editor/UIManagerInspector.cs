using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(UIManager))]
    public sealed class UIManagerInspector : Editor
    {
        private UIManager _target;

        private void OnEnable()
        {
            _target = target as UIManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("UI Manager, Add UILogic class to this interface to activate UILogic!", MessageType.Info);
            GUILayout.EndHorizontal();
            
            for (int i = 0; i < _target.UIs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + _target.UIs[i]);
                GUILayout.FlexibleSpace();
                GUI.color = _target.UIEntitys[i] ? Color.white : Color.gray;
                _target.UIEntitys[i] = EditorGUILayout.ObjectField(_target.UIEntitys[i], typeof(GameObject), false) as GameObject;
                GUI.color = Color.white;
                if (GUILayout.Button("Delete", "MiniButton"))
                {
                    _target.UIs.RemoveAt(i);
                    _target.UIEntitys.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add UILogic", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(UILogic));
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(UILogicResident))
                    {
                        Type type = types[i];
                        if (_target.UIs.Contains(type.Name))
                        {
                            gm.AddDisabledItem(new GUIContent("常驻UI/" + type.Name));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent("常驻UI/" + type.Name), false, () =>
                            {
                                _target.UIs.Add(type.Name);
                                _target.UIEntitys.Add(null);
                            });
                        }
                    }
                    else if (types[i].BaseType == typeof(UILogicTemporary))
                    {
                        Type type = types[i];
                        if (_target.UIs.Contains(type.Name))
                        {
                            gm.AddDisabledItem(new GUIContent("非常驻UI/" + type.Name));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent("非常驻UI/" + type.Name), false, () =>
                            {
                                _target.UIs.Add(type.Name);
                                _target.UIEntitys.Add(null);
                            });
                        }
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
