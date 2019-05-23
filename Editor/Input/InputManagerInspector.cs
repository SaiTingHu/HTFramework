using System;
using System.Collections.Generic;
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
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].BaseType == typeof(InputDeviceBase))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), _target.InputDeviceType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set InputDevice");
                            _target.InputDeviceType = types[j].FullName;
                            this.HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }
    }
}
