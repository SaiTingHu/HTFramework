using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class About : HTFEditorWindow
    {
        [InitializeOnLoadMethod]
        private static void OnEditorStart()
        {
            if (EditorApplication.timeSinceStartup < 30)
            {
                if (EditorPrefs.GetBool(EditorPrefsTable.AboutIsShowOnStart, true))
                {
                    EditorApplication.delayCall += () =>
                    {
                        About about = GetWindow<About>(true, "HTFramework About", true);
                        about.position = new Rect(200, 200, 600, 350);
                        about.minSize = new Vector2(600, 350);
                        about.maxSize = new Vector2(600, 350);
                        about.Show();
                    };
                }
            }
        }

        private Texture _frameworkLogo;
        private Texture _csdnLogo;
        private Texture _githubLogo;
        private GUIContent _csdnGUIContent;
        private GUIContent _githubGUIContent;
        private GUIContent _pcGUIContent;
        private GUIContent _androidGUIContent;
        private GUIContent _webglGUIContent;
        private string _version;
        private string _supported;
        private string _scripting;
        private string _api;
        private bool _isShowOnStart;
        private string _configPath;
        
        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }

        private void OnEnable()
        {
            _frameworkLogo = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFrameworkLOGOTitle2.png");
            _csdnLogo = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/CSDN.png");
            _githubLogo = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Github.png");
            _csdnGUIContent = new GUIContent();
            _csdnGUIContent.image = _csdnLogo;
            _csdnGUIContent.text = "CSDN";
            _githubGUIContent = new GUIContent();
            _githubGUIContent.image = _githubLogo;
            _githubGUIContent.text = "Github";
            _pcGUIContent = new GUIContent();
            _pcGUIContent.image = EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image;
            _pcGUIContent.text = "PC,Mac & Linux Standalone";
            _androidGUIContent = new GUIContent();
            _androidGUIContent.image = EditorGUIUtility.IconContent("BuildSettings.Android.Small").image;
            _androidGUIContent.text = "Android";
            _webglGUIContent = new GUIContent();
            _webglGUIContent.image = EditorGUIUtility.IconContent("BuildSettings.WebGL.Small").image;
            _webglGUIContent.text = "WebGL";

            ReadConfig();
        }

        private void ReadConfig()
        {
            _configPath = Application.dataPath + "/HTFramework/Editor/Utility/Config/Config.ini";

            INIParser ini = new INIParser();
            ini.Open(_configPath);
            _version = ini.ReadValue("HTFrameworkEditor", "Version", "<None>");
            _supported = ini.ReadValue("HTFrameworkEditor", "Supported", "<None>");
            _scripting = ini.ReadValue("HTFrameworkEditor", "Scripting", "<None>");
            _api = ini.ReadValue("HTFrameworkEditor", "Api", "<None>");
            _isShowOnStart = EditorPrefs.GetBool(EditorPrefsTable.AboutIsShowOnStart, true);
            ini.Close();
        }

        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            LOGOGUI();

            AboutGUI();
        }

        private void LOGOGUI()
        {
            GUI.DrawTexture(new Rect(10, 0, 400, 100), _frameworkLogo);
            GUI.Label(new Rect(80, 100, 200, 20), "Version: " + _version);
        }

        private void AboutGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_githubGUIContent, EditorGlobalTools.Styles.Label))
            {
                Application.OpenURL("https://github.com/SaiTingHu/HTFramework");
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            if (GUILayout.Button(_csdnGUIContent, EditorGlobalTools.Styles.Label))
            {
                Application.OpenURL("https://blog.csdn.net/qq992817263/category_9283445.html");
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Unity HTFramework, A framework of client to the unity.");
            GUI.color = Color.yellow;
            if (GUILayout.Button("Check for the latest updates", EditorGlobalTools.Styles.Label))
            {
                Application.OpenURL("https://github.com/SaiTingHu/HTFramework/commits/master");
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(60);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Supported Runtime Platforms: ");
            GUILayout.Label(_pcGUIContent, EditorGlobalTools.Styles.Wordwrapminibutton);
            GUILayout.Label(_androidGUIContent, EditorGlobalTools.Styles.Wordwrapminibutton);
            GUILayout.Label(_webglGUIContent, EditorGlobalTools.Styles.Wordwrapminibutton);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Supported Unity versions: " + _supported);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scripting Runtime Versions: " + _scripting);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Api Compatibility Level: " + _api);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            if (GUILayout.Button("Copyright (c) 2019 HuTao", EditorGlobalTools.Styles.Label))
            {
                Application.OpenURL("https://github.com/SaiTingHu/HTFramework/blob/master/LICENSE");
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            bool isShowOnStart = GUILayout.Toggle(_isShowOnStart, "Show On Start");
            if (isShowOnStart != _isShowOnStart)
            {
                _isShowOnStart = isShowOnStart;
                EditorPrefs.SetBool(EditorPrefsTable.AboutIsShowOnStart, _isShowOnStart);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}