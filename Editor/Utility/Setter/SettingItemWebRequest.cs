using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [InternalSettingItem(HTFrameworkModule.WebRequest)]
    internal sealed class SettingItemWebRequest : SettingItemBase
    {
        private WebRequestManager _webRequestManager;

        public override string Name
        {
            get
            {
                return "WebRequest";
            }
        }

        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            GameObject webRequestManager = GameObject.Find("HTFramework/WebRequest");
            if (webRequestManager)
            {
                _webRequestManager = webRequestManager.GetComponent<WebRequestManager>();
            }
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            if (_webRequestManager)
            {
                GUILayout.BeginHorizontal();
                _webRequestManager.IsOfflineState = EditorGUILayout.Toggle("Is OfflineState", _webRequestManager.IsOfflineState);
                GUILayout.EndHorizontal();
                
                if (GUI.changed)
                {
                    HasChanged(_webRequestManager);
                }
            }
        }
        public override void OnReset()
        {
            base.OnReset();

            if (_webRequestManager)
            {
                _webRequestManager.IsOfflineState = true;

                HasChanged(_webRequestManager);
            }
        }
    }
}