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
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            GUILayout.BeginHorizontal();
            bool isDeveloperMode = EditorGUILayout.Toggle("Is Developer Mode", _isDeveloperMode);
            if (isDeveloperMode != _isDeveloperMode)
            {
                _isDeveloperMode = isDeveloperMode;
                EditorPrefs.SetBool("DeveloperMode", _isDeveloperMode);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool isEnableLnkTools = EditorGUILayout.Toggle("Is Enable LnkTools", _isEnableLnkTools);
            if (isEnableLnkTools != _isEnableLnkTools)
            {
                if (!EditorApplication.isCompiling)
                {
                    _isEnableLnkTools = isEnableLnkTools;
                    EditorPrefs.SetBool(EditorPrefsTable.LnkTools_Enable, _isEnableLnkTools);
                    EditorApplication.delayCall += EditorGlobalTools.CoerciveCompile;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}