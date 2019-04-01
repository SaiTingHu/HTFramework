using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(InputManager))]
    public sealed class InputManagerInspector : Editor
    {
        private InputManager _target;

        private void OnEnable()
        {
            _target = target as InputManager;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Input manager, managing cross platform input!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("InputDevice ");
            if (GUILayout.Button(_target.InputDeviceType, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(InputDeviceBase));
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(InputDeviceBase))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), _target.InputDeviceType == types[j].FullName, () =>
                        {
                            _target.InputDeviceType = types[j].FullName;
                            //挂载此脚本的对象是预制体时，必须设置，否则重新编译后属性会被预制体还原
                            EditorUtility.SetDirty(_target);
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }
    }
}
