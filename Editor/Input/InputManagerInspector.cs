using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(InputManager))]
    public sealed class InputManagerInspector : HTFEditor<InputManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Input manager, managing cross platform input!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("InputDevice ");
            if (GUILayout.Button(Target.InputDeviceType, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(InputDeviceBase)))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.InputDeviceType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set InputDevice");
                            Target.InputDeviceType = types[j].FullName;
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            Target.IsEnableInputDevice = EditorGUILayout.Toggle("Enable InputDevice", Target.IsEnableInputDevice);
            GUILayout.EndHorizontal();
        }
    }
}