using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 版本号查看器
    /// </summary>
    internal sealed class VersionViewer : HTFEditorWindow, IAdminLoginWindow
    {
        public static void OpenWindow(VersionInfo info)
        {
            VersionViewer window = GetWindow<VersionViewer>();
            window.titleContent.image = EditorGUIUtility.IconContent("d_ViewToolZoom On").image;
            window.titleContent.text = "Version Viewer";
            window._versionInfo = info;
            window._version = info.PreviousVersions.Count > 0 ? info.PreviousVersions[0] : null;
            window._versionNumber = window._version != null ? window._version.GetFullNumber() : "<None>";
            window.Show();
        }

        private Color _adminModeColor = new Color(1, 0.43f, 0, 1);
        private VersionInfo _versionInfo;
        private Version _version;
        private string _versionNumber;
        private bool _isRelease = false;
        private Version _releaseVersion;
        private Vector2 _scroll;

        public bool IsAdminMode { get; set; } = false;
        public string Password
        {
            get
            {
                return "1+A/HydBW5UMiL9xsRLN2A==";
            }
        }
        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }

        private void Update()
        {
            if (_versionInfo == null)
            {
                Close();
            }
        }
        
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            TitleGUI();
            VersionGUI();
        }

        private void TitleGUI()
        {
            if (IsAdminMode)
            {
                GUI.backgroundColor = _adminModeColor;
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label("Developer Mode");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Logout", EditorStyles.toolbarPopup))
                {
                    IsAdminMode = false;
                }
                GUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Admin Login", EditorStyles.toolbarPopup))
                {
                    AdminLoginWindow.OpenWindow(this);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void VersionGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version: ", GUILayout.Width(60));
            GUI.enabled = _versionInfo.PreviousVersions.Count > 0;
            if (GUILayout.Button(_versionNumber, EditorGlobalTools.Styles.MiniPopup))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _versionInfo.PreviousVersions.Count; i++)
                {
                    Version version = _versionInfo.PreviousVersions[i];
                    if (version == _version)
                    {
                        gm.AddDisabledItem(new GUIContent(version.GetFullNumber()), true);
                    }
                    else
                    {
                        gm.AddItem(new GUIContent(version.GetFullNumber()), false, () =>
                        {
                            _version = version;
                            _versionNumber = _version.GetFullNumber();
                        });
                    }
                }
                gm.ShowAsContext();
                GUI.FocusControl(null);
            }
            if (IsAdminMode)
            {
                GUI.backgroundColor = _adminModeColor;
                if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to delete this version？", "Yes", "No"))
                    {
                        if (_version != null)
                        {
                            _versionInfo.PreviousVersions.Remove(_version);
                            _version = _versionInfo.PreviousVersions.Count > 0 ? _versionInfo.PreviousVersions[0] : null;
                            _versionNumber = _version != null ? _version.GetFullNumber() : "<None>";
                            HasChanged(_versionInfo);
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            if (_version != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Release Date: " + _version.ReleaseDate);
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Supported Unity Versions: " + _versionInfo.CurrentVersion.UnityVersions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scripting Runtime Versions: " + _versionInfo.CurrentVersion.ScriptingVersions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Api Compatibility Level: " + _versionInfo.CurrentVersion.APIVersions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Release Notes:");
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                _scroll = GUILayout.BeginScrollView(_scroll);
                GUILayout.Label(_version.ReleaseNotes);
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            if (IsAdminMode)
            {
                GUI.backgroundColor = _adminModeColor;

                GUILayout.BeginHorizontal();
                GUI.enabled = !_isRelease;
                if (GUILayout.Button("Version Release"))
                {
                    _isRelease = true;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                if (_isRelease)
                {
                    ReleaseGUI();
                }

                GUI.backgroundColor = Color.white;
            }
        }

        private void ReleaseGUI()
        {
            if (_releaseVersion == null)
            {
                _releaseVersion = new Version();
                _releaseVersion.ReleaseDate = DateTime.Now.ToString("yyyy.MM.dd");
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Version Number: ", GUILayout.Width(180));
            _releaseVersion.MajorNumber = EditorGUILayout.IntField(_releaseVersion.MajorNumber);
            _releaseVersion.MinorNumber = EditorGUILayout.IntField(_releaseVersion.MinorNumber);
            _releaseVersion.ReviseNumber = EditorGUILayout.IntField(_releaseVersion.ReviseNumber);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Supported Unity Versions: ", GUILayout.Width(180));
            _releaseVersion.UnityVersions = EditorGUILayout.TextField(_releaseVersion.UnityVersions);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scripting Runtime Versions: ", GUILayout.Width(180));
            _releaseVersion.ScriptingVersions = EditorGUILayout.TextField(_releaseVersion.ScriptingVersions);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Api Compatibility Level: ", GUILayout.Width(180));
            _releaseVersion.APIVersions = EditorGUILayout.TextField(_releaseVersion.APIVersions);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Release Date: ", GUILayout.Width(180));
            _releaseVersion.ReleaseDate = EditorGUILayout.TextField(_releaseVersion.ReleaseDate);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Release Notes:");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            _releaseVersion.ReleaseNotes = EditorGUILayout.TextArea(_releaseVersion.ReleaseNotes);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Release", EditorStyles.miniButtonLeft))
            {
                if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to release new version？Current version will be changed to [" + _releaseVersion.GetFullNumber() + "]!", "Yes", "No"))
                {
                    _versionInfo.PreviousVersions.Add(_versionInfo.CurrentVersion);
                    _versionInfo.CurrentVersion = _releaseVersion;
                    _releaseVersion = null;
                    _isRelease = false;
                    HasChanged(_versionInfo);
                }
            }
            if (GUILayout.Button("Cancel", EditorStyles.miniButtonRight))
            {
                _isRelease = false;
            }
            GUILayout.EndHorizontal();
        }

        public void AdminCheck(string password)
        {
            IsAdminMode = MathfToolkit.MD5Encrypt(password) == Password;
            _isRelease = false;
            GUI.changed = true;

            if (!IsAdminMode)
            {
                ShowNotification(new GUIContent("Password is wrong!"));
            }
        }
    }
}