using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class SettingItemTransform : SettingItemBase
    {
        private bool _onlyShowLocal = false;

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
        }
        public override void OnReset()
        {
            base.OnReset();

            _onlyShowLocal = false;
            EditorPrefs.SetBool(EditorPrefsTable.Transform_OnlyShowLocal, _onlyShowLocal);
        }
    }
}