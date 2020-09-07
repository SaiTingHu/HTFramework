using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块的检视器
    /// </summary>
    public abstract class InternalModuleInspector<M> : HTFEditor<M> where M : Object
    {
        private InternalModuleBase _module;
        protected IInternalModuleHelper _helper;

        protected virtual string Intro
        {
            get
            {
                return "";
            }
        }

        protected virtual Type HelperInterface
        {
            get
            {
                return null;
            }
        }

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _module = Target as InternalModuleBase;
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            FieldInfo fieldInfo = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic);
            _helper = fieldInfo != null ? (fieldInfo.GetValue(Target) as IInternalModuleHelper) : null;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(Intro, MessageType.Info);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUI.enabled = !EditorApplication.isPlaying && HelperInterface != null;
            GUILayout.Label("Helper", GUILayout.Width(50));
            Button(ChangeHelper, _module.HelperType, EditorGlobalTools.Styles.MiniPopup);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void ChangeHelper()
        {
            GenericMenu gm = new GenericMenu();
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return HelperInterface.IsAssignableFrom(type) && HelperInterface != type;
            });
            for (int i = 0; i < types.Count; i++)
            {
                int j = i;
                gm.AddItem(new GUIContent(types[j].FullName), _module.HelperType == types[j].FullName, () =>
                {
                    _module.HelperType = types[j].FullName;
                });
            }
            gm.ShowAsContext();
        }
    }
}