﻿using System;
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
        private HTFAction _changeHelper;

        protected virtual string Intro => null;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _module = Target as InternalModuleBase<H>;
            _types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return typeof(H).IsAssignableFrom(type) && typeof(H) != type;
            });
            _changeHelper = ChangeHelper;
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
            EditorGUILayout.HelpBox(Intro, MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !EditorApplication.isPlaying && _types.Count > 0;
            EditorGUILayout.LabelField("Helper", GUILayout.Width(LabelWidth));
            Button(_changeHelper, _module.HelperType, EditorStyles.popup, GUILayout.Width(EditorGUIUtility.currentViewWidth - LabelWidth - 25));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }
        private void ChangeHelper()
        {
            GenericMenu gm = new GenericMenu();
            for (int i = 0; i < _types.Count; i++)
            {
                int j = i;
                gm.AddItem(new GUIContent(_types[j].FullName), _module.HelperType == _types[j].FullName, () =>
                {
                    _module.HelperType = _types[j].FullName;
                });
            }
            gm.ShowAsContext();
        }
    }
}