using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 内置模块的检视器
    /// </summary>
    /// <typeparam name="M">内置模块</typeparam>
    /// <typeparam name="H">内置模块的助手</typeparam>
    internal abstract class InternalModuleInspector<M, H> : HTFEditor<M> where M : UObject where H : class, IInternalModuleHelper
    {
        protected H _helper;
        private InternalModuleBase<H> _module;
        private List<Type> _types;

        protected override bool IsEnableRuntimeData
        {
            get
            {
                return Main.Current != null && Main.Current.IsInitCompleted && Main.Current.IsReadyCompleted;
            }
        }

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _module = Target as InternalModuleBase<H>;
            _types = ReflectionToolkit.GetTypesInAllAssemblies(type =>
            {
                return typeof(H).IsAssignableFrom(type) && typeof(H) != type;
            }, false);
        }
        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            PropertyInfo propertyInfo = Target.GetType().GetProperty("_helper", BindingFlags.Instance | BindingFlags.NonPublic);
            _helper = propertyInfo != null ? (propertyInfo.GetValue(Target) as H) : null;
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !EditorApplication.isPlaying && _types.Count > 0;
            EditorGUILayout.LabelField("Helper", GUILayout.Width(LabelWidth));
            if (GUILayout.Button(_module.HelperType, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25)))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _types.Count; i++)
                {
                    int j = i;
                    gm.AddItem(new GUIContent(_types[j].FullName), _module.HelperType == _types[j].FullName, () =>
                    {
                        Undo.RecordObject(target, "Change Helper");
                        _module.HelperType = _types[j].FullName;
                        HasChanged();
                    });
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
    }
}