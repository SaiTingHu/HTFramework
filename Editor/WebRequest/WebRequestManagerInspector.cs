using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(WebRequestManager))]
    [GiteeURL("https://gitee.com/SaiTingHu/HTFramework")]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/89886124")]
    internal sealed class WebRequestManagerInspector : InternalModuleInspector<WebRequestManager>
    {
        private IWebRequestHelper _webRequestHelper;

        protected override string Intro
        {
            get
            {
                return "Web request manager, it manages all web request! you can submit forms, upload files, download files!";
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

            _webRequestHelper = _helper as IWebRequestHelper;
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUI.enabled = !EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();
            Toggle(Target.IsOfflineState, out Target.IsOfflineState, "Is OfflineState");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.DownloadAudioType, out Target.DownloadAudioType, "Audio Type");
            GUILayout.EndHorizontal();

            GUI.enabled = true;
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
