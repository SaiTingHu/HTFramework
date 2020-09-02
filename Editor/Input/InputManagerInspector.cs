using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(InputManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89001848")]
    internal sealed class InputManagerInspector : InternalModuleInspector<InputManager>
    {
        protected override string Intro
        {
            get
            {
                return "Input manager, managing cross platform input!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IInputHelper);
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();
            GUILayout.Label("InputDevice ");
            if (GUILayout.Button(Target.InputDeviceType, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(InputDeviceBase));
                });
                for (int i = 0; i < types.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(types[j].FullName), Target.InputDeviceType == types[j].FullName, () =>
                    {
                        Undo.RecordObject(target, "Set InputDevice");
                        Target.InputDeviceType = types[j].FullName;
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUI.enabled = true;
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