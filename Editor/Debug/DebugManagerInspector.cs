using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(DebugManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/102570194")]
    internal sealed class DebugManagerInspector : InternalModuleInspector<DebugManager>
    {
        protected override string Intro
        {
            get
            {
                return "Debug Manager, Runtime debugger for games!";
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsEnableDebugger, out Target.IsEnableDebugger, "Is Enable Debugger");
            GUILayout.EndHorizontal();

            if (Target.IsEnableDebugger)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debugger", GUILayout.Width(100));
                if (GUILayout.Button(Target.DebuggerType, EditorGlobalTools.Styles.MiniPopup))
                {
                    GenericMenu gm = new GenericMenu();
                    List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                    {
                        return type == typeof(Debugger) || type.IsSubclassOf(typeof(Debugger));
                    });
                    for (int i = 0; i < types.Count; i++)
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.DebuggerType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set Debugger");
                            Target.DebuggerType = types[j].FullName;
                            HasChanged();
                        });
                    }
                    gm.ShowAsContext();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Debugger Skin", GUILayout.Width(100));
                ObjectField(Target.DebuggerSkin, out Target.DebuggerSkin, false, "");
                GUILayout.EndHorizontal();
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}