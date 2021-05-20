using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(InputManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89001848")]
    internal sealed class InputManagerInspector : InternalModuleInspector<InputManager, IInputHelper>
    {
        protected override string Intro
        {
            get
            {
                return "Input manager, this is a cross platform input solution!";
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();
            GUILayout.Label("InputDevice ", GUILayout.Width(LabelWidth));
            if (GUILayout.Button(Target.InputDeviceType, EditorStyles.popup))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                {
                    return type.IsSubclassOf(typeof(InputDeviceBase)) && !type.IsAbstract;
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