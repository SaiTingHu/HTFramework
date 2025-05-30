﻿using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89886124")]
    internal sealed class WebRequestManagerInspector : InternalModuleInspector<WebRequestManager, IWebRequestHelper>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            PropertyField(nameof(WebRequestManager.IsOfflineState), "Offline State");
            PropertyField(nameof(WebRequestManager.IsLogDetail), "Is Log Detail");
            PropertyField(nameof(WebRequestManager.DownloadAudioType), "Audio Type");
            
            GUI.enabled = true;
        }
        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (_helper == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Web Interfaces: {_helper.WebInterfaces.Count}");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Connected Internet: {_helper.IsConnectedInternet}");
            GUILayout.EndHorizontal();

            foreach (var inter in _helper.WebInterfaces)
            {
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(LabelWidth));
                EditorGUILayout.TextField(inter.Value.Name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Url", GUILayout.Width(LabelWidth));
                EditorGUILayout.TextField(inter.Value.Url);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Offline", GUILayout.Width(LabelWidth));
                inter.Value.IsOffline = EditorGUILayout.Toggle(inter.Value.IsOffline);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
    }
}