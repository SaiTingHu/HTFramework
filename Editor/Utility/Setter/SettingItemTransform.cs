using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class SettingItemTransform : SettingItemBase
    {
        private bool _onlyShowLocal = false;
        private bool _copyInjectPath = false;

        public override string Name
        {
            get
            {
                return "Transform";
            }
        }
        
        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            _onlyShowLocal = EditorPrefs.GetBool(EditorPrefsTable.Transform_OnlyShowLocal, false);
            _copyInjectPath = EditorPrefs.GetBool(EditorPrefsTable.Transform_CopyInjectPath, false);
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            GUILayout.BeginHorizontal();
            bool onlyShowLocal = EditorGUILayout.Toggle("Only Show Local", _onlyShowLocal);
            if (onlyShowLocal != _onlyShowLocal)
            {
                _onlyShowLocal = onlyShowLocal;
                EditorPrefs.SetBool(EditorPrefsTable.Transform_OnlyShowLocal, _onlyShowLocal);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool copyInjectPath = EditorGUILayout.Toggle("Copy Inject Path", _copyInjectPath);
            if (copyInjectPath != _copyInjectPath)
            {
                _copyInjectPath = copyInjectPath;
                EditorPrefs.SetBool(EditorPrefsTable.Transform_CopyInjectPath, _copyInjectPath);
            }
            GUILayout.EndHorizontal();
        }
        public override void OnReset()
        {
            base.OnReset();

            _onlyShowLocal = false;
            _copyInjectPath = false;
            EditorPrefs.SetBool(EditorPrefsTable.Transform_OnlyShowLocal, _onlyShowLocal);
            EditorPrefs.SetBool(EditorPrefsTable.Transform_CopyInjectPath, _copyInjectPath);
        }
    }
}