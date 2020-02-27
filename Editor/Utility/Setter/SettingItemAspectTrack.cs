using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [InternalSettingItem(HTFrameworkModule.AspectTrack)]
    internal sealed class SettingItemAspectTrack : SettingItemBase
    {
        private AspectTracker _aspectTracker;

        public override string Name
        {
            get
            {
                return "AspectTrack";
            }
        }

        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            GameObject aspectTracker = GameObject.Find("HTFramework/AspectTrack");
            if (aspectTracker)
            {
                _aspectTracker = aspectTracker.GetComponent<AspectTracker>();
            }
        }

        public override void OnSettingGUI()
        {
            base.OnSettingGUI();
            
            if (_aspectTracker)
            {
                GUILayout.BeginHorizontal();
                _aspectTracker.IsEnableAspectTrack = EditorGUILayout.Toggle("Is Enable Track", _aspectTracker.IsEnableAspectTrack);
                GUILayout.EndHorizontal();

                if (_aspectTracker.IsEnableAspectTrack)
                {
                    GUILayout.BeginHorizontal();
                    _aspectTracker.IsEnableIntercept = EditorGUILayout.Toggle("Is Enable Intercept", _aspectTracker.IsEnableIntercept);
                    GUILayout.EndHorizontal();
                }

                if (GUI.changed)
                {
                    HasChanged(_aspectTracker);
                }
            }
        }
        
        public override void OnReset()
        {
            base.OnReset();

            if (_aspectTracker)
            {
                _aspectTracker.IsEnableAspectTrack = false;
                _aspectTracker.IsEnableIntercept = false;

                HasChanged(_aspectTracker);
            }
        }
    }
}