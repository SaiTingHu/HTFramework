using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [InternalSettingItem(HTFrameworkModule.Main)]
    internal sealed class SettingItemMain : SettingItemBase
    {
        private Main _main;
        private bool _isDeveloperMode = false;
        private bool _isEnableLnkTools = false;
        private bool _isEnableSaveDataRuntime = false;
        private string _vscodePath;
        private string _ilspyPath;

        public override string Name
        {
            get
            {
                return "Main";
            }
        }
        
        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            GameObject main = GameObject.Find("HTFramework");
            if (main)
            {
                _main = main.GetComponent<Main>();
            }

            _isDeveloperMode = Unsupported.IsDeveloperMode();
            _isEnableLnkTools = EditorPrefs.GetBool(EditorPrefsTable.LnkTools_Enable, true);
            _isEnableSaveDataRuntime = EditorPrefs.GetBool(EditorPrefsTable.SaveDataRuntime_Enable, true);
            _vscodePath = EditorPrefs.GetString(EditorPrefsTable.VSCodePath, null);
            _ilspyPath = EditorPrefs.GetString(EditorPrefsTable.ILSpyPath, null);
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            GUILayout.BeginHorizontal();
            bool isDeveloperMode = EditorGUILayout.Toggle("Developer Mode", _isDeveloperMode);
            if (isDeveloperMode != _isDeveloperMode)
            {
                _isDeveloperMode = isDeveloperMode;
                EditorPrefs.SetBool("DeveloperMode", _isDeveloperMode);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool isEnableLnkTools = EditorGUILayout.Toggle("Enable LnkTools", _isEnableLnkTools);
            if (isEnableLnkTools != _isEnableLnkTools)
            {
                if (!EditorApplication.isCompiling)
                {
                    _isEnableLnkTools = isEnableLnkTools;
                    EditorPrefs.SetBool(EditorPrefsTable.LnkTools_Enable, _isEnableLnkTools);
                    EditorUtility.RequestScriptReload();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool isEnableSaveDataRuntime = EditorGUILayout.Toggle("Enable SaveDataRuntime", _isEnableSaveDataRuntime);
            if (isEnableSaveDataRuntime != _isEnableSaveDataRuntime)
            {
                if (!EditorApplication.isCompiling)
                {
                    _isEnableSaveDataRuntime = isEnableSaveDataRuntime;
                    EditorPrefs.SetBool(EditorPrefsTable.SaveDataRuntime_Enable, _isEnableSaveDataRuntime);
                    EditorUtility.RequestScriptReload();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string vscodePath = EditorGUILayout.TextField("VSCode Path", _vscodePath);
            if (vscodePath != _vscodePath)
            {
                _vscodePath = vscodePath;
                EditorPrefs.SetString(EditorPrefsTable.VSCodePath, _vscodePath);
            }
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select VSCode Path", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "exe");
                if (path.Length != 0)
                {
                    _vscodePath = path;
                    EditorPrefs.SetString(EditorPrefsTable.VSCodePath, _vscodePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string ilspyPath = EditorGUILayout.TextField("ILSpy Path", _ilspyPath);
            if (ilspyPath != _ilspyPath)
            {
                _ilspyPath = ilspyPath;
                EditorPrefs.SetString(EditorPrefsTable.ILSpyPath, _ilspyPath);
            }
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select ILSpy Path", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "exe");
                if (path.Length != 0)
                {
                    _ilspyPath = path;
                    EditorPrefs.SetString(EditorPrefsTable.ILSpyPath, _ilspyPath);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}