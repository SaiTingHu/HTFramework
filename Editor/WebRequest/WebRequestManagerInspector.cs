using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89886124")]
    internal sealed class WebRequestManagerInspector : InternalModuleInspector<WebRequestManager>
    {
        private IWebRequestHelper _webRequestHelper;

        protected override string Intro
        {
            get
            {
                return "Web request manager, it manages all Web request! you can submit forms, upload files, download files!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IWebRequestHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _webRequestHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IWebRequestHelper;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

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
            GUILayout.Label("Web Interfaces:" + _webRequestHelper.WebInterfaces.Count);
            GUILayout.EndHorizontal();

            foreach (var inter in _webRequestHelper.WebInterfaces)
            {
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

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
