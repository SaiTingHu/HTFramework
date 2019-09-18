using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89886124")]
    public sealed class WebRequestManagerInspector : HTFEditor<WebRequestManager>
    {
        private Dictionary<string, WebInterfaceBase> _interfaces;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _interfaces = Target.GetType().GetField("_interfaces", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<string, WebInterfaceBase>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Web request manager, it manages all Web request! you can submit forms, upload files, download files!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsOfflineState, out Target.IsOfflineState, "Is OfflineState");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.DownloadAudioType, out Target.DownloadAudioType, "Audio Type");
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Web Interfaces:" + _interfaces.Count);
            GUILayout.EndHorizontal();

            foreach (var inter in _interfaces)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(40));
                EditorGUILayout.TextField(inter.Value.Name);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Url", GUILayout.Width(40));
                EditorGUILayout.TextField(inter.Value.Url);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Is Offline", GUILayout.Width(60));
                inter.Value.IsOffline = EditorGUILayout.Toggle(inter.Value.IsOffline);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
    }
}
