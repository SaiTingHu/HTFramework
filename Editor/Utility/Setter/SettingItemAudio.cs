using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [InternalSettingItem(HTFrameworkModule.Audio)]
    internal sealed class SettingItemAudio : SettingItemBase
    {
        private AudioManager _audio;

        public override string Name
        {
            get
            {
                return "Audio";
            }
        }
        
        public override void OnBeginSetting()
        {
            base.OnBeginSetting();

            GameObject audio = GameObject.Find("HTFramework/Audio");
            if (audio)
            {
                _audio = audio.GetComponent<AudioManager>();
            }
        }
        public override void OnSettingGUI()
        {
            base.OnSettingGUI();

            if (_audio)
            {
                GUILayout.BeginHorizontal();
                _audio.MuteDefault = EditorGUILayout.Toggle("Mute", _audio.MuteDefault);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.BackgroundPriorityDefault = EditorGUILayout.IntSlider("Background Priority", _audio.BackgroundPriorityDefault, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.BackgroundVolumeDefault = EditorGUILayout.Slider("Background Volume", _audio.BackgroundVolumeDefault, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.SinglePriorityDefault = EditorGUILayout.IntSlider("Single Priority", _audio.SinglePriorityDefault, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.SingleVolumeDefault = EditorGUILayout.Slider("Single Volume", _audio.SingleVolumeDefault, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.MultiplePriorityDefault = EditorGUILayout.IntSlider("Multiple Priority", _audio.MultiplePriorityDefault, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.MultipleVolumeDefault = EditorGUILayout.Slider("Multiple Volume", _audio.MultipleVolumeDefault, 0f, 1f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.WorldPriorityDefault = EditorGUILayout.IntSlider("WorldSound Priority", _audio.WorldPriorityDefault, 0, 256);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _audio.WorldVolumeDefault = EditorGUILayout.Slider("WorldSound Volume", _audio.WorldVolumeDefault, 0f, 1f);
                GUILayout.EndHorizontal();

                if (GUI.changed)
                {
                    HasChanged(_audio);
                }
            }
        }
        public override void OnReset()
        {
            base.OnReset();

            if (_audio)
            {
                _audio.MuteDefault = false;

                HasChanged(_audio);
            }
        }
    }
}